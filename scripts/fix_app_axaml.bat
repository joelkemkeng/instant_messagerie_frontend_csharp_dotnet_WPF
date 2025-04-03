@echo off
echo Mise à jour du fichier App.axaml...

REM Sauvegarder le fichier original
copy HeticStream.UI\App.axaml HeticStream.UI\App.axaml.original
echo Sauvegarde créée dans HeticStream.UI\App.axaml.original

REM Utiliser la version alternative
copy HeticStream.UI\App.axaml.alternative HeticStream.UI\App.axaml
echo App.axaml mis à jour avec la version alternative

echo.
echo Modification terminée! Essayez maintenant:
echo dotnet build
echo dotnet run --project HeticStream.UI
echo.