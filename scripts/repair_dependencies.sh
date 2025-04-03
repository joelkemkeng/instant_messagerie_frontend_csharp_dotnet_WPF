#!/bin/bash
echo "Réparation des dépendances pour HeticStream..."

# Supprimer les dossiers obj et bin pour une compilation propre
echo "Nettoyage des dossiers bin et obj..."
rm -rf HeticStream.Core/bin
rm -rf HeticStream.Core/obj
rm -rf HeticStream.Services/bin
rm -rf HeticStream.Services/obj
rm -rf HeticStream.UI/bin
rm -rf HeticStream.UI/obj

# Restaurer les packages
echo "Restauration des packages NuGet..."
dotnet restore

# Installer explicitement Material.Icons.Avalonia
echo "Installation de Material.Icons.Avalonia..."
dotnet add HeticStream.UI package Material.Icons.Avalonia --version 2.0.1

# Utiliser l'App.axaml alternatif si nécessaire
echo "Si vous rencontrez toujours des problèmes, lancez le script fix_app_axaml.sh"

echo ""
echo "Réparation terminée! Essayez maintenant:"
echo "dotnet build"
echo "dotnet run --project HeticStream.UI"
echo ""