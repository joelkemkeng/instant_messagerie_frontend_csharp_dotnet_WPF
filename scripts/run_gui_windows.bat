@echo off
:: Script pour lancer HeticStream en mode GUI sur Windows avec WSL2

echo Veuillez vous assurer qu'un serveur X comme VcXsrv est en cours d'exécution
echo Et que vous avez configuré la variable DISPLAY dans WSL si nécessaire.
pause

:: Arrêter les conteneurs existants si nécessaire
docker-compose down

:: Démarrer avec le mode GUI
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d

:: Afficher les logs
echo Application démarrée en mode GUI, affichage des logs :
docker logs -f hetic-stream