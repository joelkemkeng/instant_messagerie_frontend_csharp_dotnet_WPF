@echo off
echo Réparation des dépendances pour HeticStream...

REM Supprimer les dossiers obj et bin pour une compilation propre
echo Nettoyage des dossiers bin et obj...
rmdir /s /q HeticStream.Core\bin 2>nul
rmdir /s /q HeticStream.Core\obj 2>nul
rmdir /s /q HeticStream.Services\bin 2>nul
rmdir /s /q HeticStream.Services\obj 2>nul
rmdir /s /q HeticStream.UI\bin 2>nul
rmdir /s /q HeticStream.UI\obj 2>nul

REM Restaurer les packages
echo Restauration des packages NuGet...
dotnet restore

REM Installer explicitement Material.Icons.Avalonia
echo Installation de Material.Icons.Avalonia...
dotnet add HeticStream.UI package Material.Icons.Avalonia --version 2.0.1

REM Utiliser l'App.axaml alternatif si nécessaire
echo Si vous rencontrez toujours des problèmes, lancez le script fix_app_axaml.bat

echo.
echo Réparation terminée! Essayez maintenant:
echo dotnet build
echo dotnet run --project HeticStream.UI
echo.