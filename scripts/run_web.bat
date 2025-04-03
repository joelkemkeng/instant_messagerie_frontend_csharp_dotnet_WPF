@echo off
:: Script pour lancer HeticStream en mode web sur Windows

echo Démarrage de HeticStream en mode web...

:: Arrêter les conteneurs existants si nécessaire
docker-compose down

:: Démarrer avec le mode web
docker-compose -f docker-compose.yml -f docker-compose.web.yml up -d

:: Afficher les instructions
echo.
echo Application démarrée en mode web
echo Accédez à http://localhost dans votre navigateur
echo.
echo Appuyez sur une touche pour voir les logs...
pause > nul

:: Afficher les logs
docker logs -f hetic-stream