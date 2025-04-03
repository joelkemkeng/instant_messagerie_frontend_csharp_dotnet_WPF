#!/bin/bash
echo "Suppression de la restriction de version du SDK .NET..."

# Sauvegarder le fichier global.json au cas où
mv global.json global.json.bak
echo "Le fichier global.json a été renommé en global.json.bak"

echo ""
echo "La restriction de version du SDK a été supprimée."
echo "Vous pouvez maintenant utiliser le SDK .NET installé sur votre système."
echo ""
echo "Essayez maintenant:"
echo "dotnet build"
echo "dotnet run --project HeticStream.UI"
echo ""