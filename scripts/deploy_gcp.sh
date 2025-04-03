#!/bin/bash

# Script de déploiement vers Google Cloud Platform
# Usage: ./deploy_gcp.sh [project_id] [region]
# Exemple: ./deploy_gcp.sh hetic-stream-project us-central1

PROJECT_ID=${1}
REGION=${2:-"us-central1"}

if [ -z "$PROJECT_ID" ]; then
    echo "Erreur: L'ID du projet GCP est requis"
    echo "Usage: ./deploy_gcp.sh [project_id] [region]"
    exit 1
fi

echo "===== Déploiement d'Hetic-Stream sur Google Cloud (projet: $PROJECT_ID, région: $REGION) ====="

# Vérifier si Google Cloud SDK est installé
if ! command -v gcloud &> /dev/null; then
    echo "Google Cloud SDK n'est pas installé. Installation en cours..."
    curl https://sdk.cloud.google.com | bash
    exec -l $SHELL
    gcloud init
fi

# Vérifier l'authentification GCP
echo "Vérification de l'authentification GCP..."
gcloud auth list --filter=status:ACTIVE --format="value(account)" || {
    echo "Erreur: Authentification GCP échouée"
    echo "Veuillez vous connecter avec: gcloud auth login"
    gcloud auth login
}

# Configurer le projet
echo "Configuration du projet GCP..."
gcloud config set project $PROJECT_ID

# Activer les APIs nécessaires
echo "Activation des APIs nécessaires..."
gcloud services enable container.googleapis.com
gcloud services enable cloudbuild.googleapis.com
gcloud services enable run.googleapis.com

# Construction et envoi de l'image avec Cloud Build
echo "Construction et envoi de l'image avec Cloud Build..."
gcloud builds submit --tag gcr.io/$PROJECT_ID/hetic-stream

# Déploiement sur Cloud Run
echo "Déploiement sur Cloud Run..."
gcloud run deploy hetic-stream \
    --image gcr.io/$PROJECT_ID/hetic-stream \
    --platform managed \
    --region $REGION \
    --allow-unauthenticated \
    --set-env-vars="ApiBaseUrl=https://api.heticstream.com,ApiEnabled=true,EndpointLogin=/auth/login,EndpointRegister=/auth/register,EndpointChannels=/channels,EndpointMessages=/messages,ImageAssetsPath=Assets/Images,LightThemeEnabled=false"

# Obtenir l'URL du service
URL=$(gcloud run services describe hetic-stream --platform managed --region $REGION --format="value(status.url)")

echo -e "\nDéploiement vers Google Cloud terminé!"
echo "Image Docker: gcr.io/$PROJECT_ID/hetic-stream"
echo "Service Cloud Run: hetic-stream"
echo "URL d'accès: $URL"

echo -e "\nPour vérifier l'état du déploiement:"
echo "gcloud run services describe hetic-stream --platform managed --region $REGION"

echo -e "\nPour voir les journaux:"
echo "gcloud logging read \"resource.type=cloud_run_revision AND resource.labels.service_name=hetic-stream\" --limit 10"