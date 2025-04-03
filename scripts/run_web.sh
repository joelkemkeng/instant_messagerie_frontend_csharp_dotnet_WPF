#!/bin/bash
# Script pour lancer HeticStream en mode web

# Arrêter les conteneurs existants si nécessaire
docker-compose down

# Démarrer avec le mode web
docker-compose -f docker-compose.yml -f docker-compose.web.yml up -d

# Afficher les logs
echo "Application démarrée en mode web, accessible sur http://localhost"
echo "Affichage des logs :"
docker logs -f hetic-stream