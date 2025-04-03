#!/bin/bash
# Script pour lancer HeticStream en mode GUI sur Linux

# Autoriser l'accès à X11 pour Docker
xhost +local:docker

# Arrêter les conteneurs existants si nécessaire
docker-compose down

# Démarrer avec le mode GUI
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d

# Afficher les logs
echo "Application démarrée en mode GUI, affichage des logs :"
docker logs -f hetic-stream