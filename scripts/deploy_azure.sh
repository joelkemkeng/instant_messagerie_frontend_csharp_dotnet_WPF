#!/bin/bash

# Script de déploiement vers Azure
# Usage: ./deploy_azure.sh [location] [resource_group]
# Exemple: ./deploy_azure.sh westeurope hetic-stream-rg

LOCATION=${1:-"westeurope"}
RESOURCE_GROUP=${2:-"hetic-stream-rg"}
ACR_NAME=${3:-"heticstreamacr"}

echo "===== Déploiement d'Hetic-Stream sur Azure (location: $LOCATION) ====="

# Vérifier si Azure CLI est installé
if ! command -v az &> /dev/null; then
    echo "Azure CLI n'est pas installé. Installation en cours..."
    curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
fi

# Vérifier l'authentification Azure
echo "Vérification de l'authentification Azure..."
az account show &> /dev/null || {
    echo "Erreur: Authentification Azure échouée"
    echo "Veuillez vous connecter avec: az login"
    az login
}

# Création du groupe de ressources
echo "Création du groupe de ressources..."
az group show --name $RESOURCE_GROUP &> /dev/null || {
    az group create --name $RESOURCE_GROUP --location $LOCATION
}

# Création du registre de conteneurs
echo "Création du registre de conteneurs Azure..."
az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP &> /dev/null || {
    az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic
}

# Authentification au registre
echo "Authentification au registre de conteneurs..."
az acr login --name $ACR_NAME

# Construction et envoi de l'image
echo "Construction de l'image Docker..."
docker build -t ${ACR_NAME}.azurecr.io/hetic-stream:latest .

echo "Envoi de l'image vers ACR..."
docker push ${ACR_NAME}.azurecr.io/hetic-stream:latest

# Création de l'Azure Container Instance
echo "Création de l'instance de conteneur Azure..."
echo "⚠️ ATTENTION: Cette commande vous demandera d'entrer le nom d'utilisateur et le mot de passe du registre"
echo "Utilisez les commandes suivantes pour obtenir les identifiants:"
echo "Nom d'utilisateur: az acr credential show --name $ACR_NAME --query \"username\" -o tsv"
echo "Mot de passe: az acr credential show --name $ACR_NAME --query \"passwords[0].value\" -o tsv"

# Obtenir les identifiants automatiquement
ACR_USERNAME=$(az acr credential show --name $ACR_NAME --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name $ACR_NAME --query "passwords[0].value" -o tsv)

echo "Création de l'instance de conteneur..."
az container create \
    --resource-group $RESOURCE_GROUP \
    --name hetic-stream \
    --image ${ACR_NAME}.azurecr.io/hetic-stream:latest \
    --cpu 1 \
    --memory 1.5 \
    --registry-login-server ${ACR_NAME}.azurecr.io \
    --registry-username $ACR_USERNAME \
    --registry-password $ACR_PASSWORD \
    --dns-name-label hetic-stream-$RANDOM \
    --ports 5000 \
    --environment-variables \
        ApiBaseUrl=https://api.heticstream.com \
        ApiEnabled=true \
        EndpointLogin=/auth/login \
        EndpointRegister=/auth/register \
        EndpointChannels=/channels \
        EndpointMessages=/messages \
        ImageAssetsPath=Assets/Images \
        LightThemeEnabled=false

# Obtenir l'URL de l'application
FQDN=$(az container show --resource-group $RESOURCE_GROUP --name hetic-stream --query ipAddress.fqdn -o tsv)
IP=$(az container show --resource-group $RESOURCE_GROUP --name hetic-stream --query ipAddress.ip -o tsv)

echo -e "\nDéploiement vers Azure terminé!"
echo "Image Docker: ${ACR_NAME}.azurecr.io/hetic-stream:latest"
echo "Groupe de ressources: $RESOURCE_GROUP"
echo "Instance de conteneur: hetic-stream"
echo "URL d'accès: http://$FQDN:5000"
echo "Adresse IP: $IP:5000"

echo -e "\nPour vérifier l'état du déploiement:"
echo "az container show --resource-group $RESOURCE_GROUP --name hetic-stream --query instanceView.state"

echo -e "\nPour voir les journaux:"
echo "az container logs --resource-group $RESOURCE_GROUP --name hetic-stream"