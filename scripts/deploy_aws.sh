#!/bin/bash

# Script de déploiement vers AWS
# Usage: ./deploy_aws.sh [region] [account_id]
# Exemple: ./deploy_aws.sh eu-west-3 123456789012

REGION=${1:-"eu-west-3"}
ACCOUNT_ID=${2}

if [ -z "$ACCOUNT_ID" ]; then
    echo "Erreur: L'ID de compte AWS est requis"
    echo "Usage: ./deploy_aws.sh [region] [account_id]"
    exit 1
fi

echo "===== Déploiement d'Hetic-Stream sur AWS (région: $REGION) ====="

# Vérifier si AWS CLI est installé
if ! command -v aws &> /dev/null; then
    echo "AWS CLI n'est pas installé. Installation en cours..."
    curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
    unzip awscliv2.zip
    sudo ./aws/install
    rm -rf aws awscliv2.zip
fi

# Vérifier l'authentification AWS
echo "Vérification de l'authentification AWS..."
aws sts get-caller-identity || {
    echo "Erreur: Authentification AWS échouée"
    echo "Veuillez configurer vos identifiants AWS avec: aws configure"
    exit 1
}

# Création du dépôt ECR
echo "Création du dépôt ECR..."
aws ecr describe-repositories --repository-names hetic-stream --region $REGION &> /dev/null || {
    aws ecr create-repository --repository-name hetic-stream --region $REGION
}

# Authentification à ECR
echo "Authentification à ECR..."
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com

# Construction et envoi de l'image
echo "Construction de l'image Docker..."
docker build -t hetic-stream .

echo "Étiquetage de l'image..."
docker tag hetic-stream:latest $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/hetic-stream:latest

echo "Envoi de l'image vers ECR..."
docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/hetic-stream:latest

# Création du cluster ECS
echo "Création du cluster ECS..."
aws ecs describe-clusters --clusters hetic-stream-cluster --region $REGION | grep -q "hetic-stream-cluster" || {
    aws ecs create-cluster --cluster-name hetic-stream-cluster --region $REGION
}

# Préparation de la définition de tâche
echo "Préparation de la définition de tâche..."
sed -i "s/ACCOUNT_ID/$ACCOUNT_ID/g" task-definition.json
sed -i "s/REGION/$REGION/g" task-definition.json

# Enregistrement de la définition de tâche
echo "Enregistrement de la définition de tâche..."
aws ecs register-task-definition --cli-input-json file://task-definition.json --region $REGION

# Création du service ECS
echo "Création du service ECS..."
# Note: Cette partie nécessite un sous-réseau et un groupe de sécurité configurés pour votre VPC
# Remplacez 'subnet-12345678' et 'sg-12345678' par vos propres valeurs
echo "⚠️ ATTENTION: Pour créer le service ECS, vous devez spécifier votre sous-réseau et groupe de sécurité"
echo "Commande à exécuter manuellement après avoir configuré votre réseau:"
echo "aws ecs create-service --cluster hetic-stream-cluster --service-name hetic-stream-service --task-definition hetic-stream --desired-count 1 --launch-type FARGATE --network-configuration \"awsvpcConfiguration={subnets=[subnet-12345678],securityGroups=[sg-12345678],assignPublicIp=ENABLED}\" --region $REGION"

echo -e "\nDéploiement vers AWS terminé!"
echo "Image Docker: $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/hetic-stream:latest"
echo "Cluster ECS: hetic-stream-cluster"
echo "Définition de tâche: hetic-stream"

echo -e "\nPour vérifier l'état du déploiement:"
echo "aws ecs describe-services --cluster hetic-stream-cluster --services hetic-stream-service --region $REGION"