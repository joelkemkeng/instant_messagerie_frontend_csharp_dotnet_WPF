# Guide de Dépannage pour HeticStream

Ce document contient les solutions aux problèmes courants rencontrés lors de l'exécution de HeticStream.

## Problème : Erreur avec Material.Icons.Avalonia

Si vous rencontrez cette erreur lors de la compilation :
```
Unable to resolve XAML resource "avares://Material.Icons.Avalonia/App.xaml" in the "Material.Icons.Avalonia" assembly
```

### Solution 1 : Utiliser les scripts de réparation

#### Windows
1. Exécutez le script de réparation des dépendances :
   ```
   scripts\repair_dependencies.bat
   ```

2. Si le problème persiste, exécutez le script pour utiliser une version alternative de App.axaml :
   ```
   scripts\fix_app_axaml.bat
   ```

#### Linux/macOS
1. Exécutez le script de réparation des dépendances :
   ```
   ./scripts/repair_dependencies.sh
   ```

2. Si le problème persiste, exécutez le script pour utiliser une version alternative de App.axaml :
   ```
   ./scripts/fix_app_axaml.sh
   ```

### Solution 2 : Installation manuelle des packages

1. Nettoyez les dossiers bin et obj :
   ```
   # Windows
   rmdir /s /q HeticStream.Core\bin HeticStream.Core\obj HeticStream.Services\bin HeticStream.Services\obj HeticStream.UI\bin HeticStream.UI\obj
   
   # Linux/macOS
   rm -rf HeticStream.Core/bin HeticStream.Core/obj HeticStream.Services/bin HeticStream.Services/obj HeticStream.UI/bin HeticStream.UI/obj
   ```

2. Restaurez les packages NuGet :
   ```
   dotnet restore
   ```

3. Installez explicitement Material.Icons.Avalonia :
   ```
   dotnet add HeticStream.UI package Material.Icons.Avalonia --version 2.0.1
   ```

4. Si cela ne fonctionne pas, modifiez manuellement le fichier `HeticStream.UI/App.axaml` et commentez la ligne problématique :
   ```xml
   <!-- <StyleInclude Source="avares://Material.Icons.Avalonia/App.xaml" /> -->
   ```

### Solution 3 : Utiliser Docker (recommandé)

Si vous continuez à rencontrer des problèmes avec l'installation locale, utilisez Docker qui encapsule toutes les dépendances nécessaires :

```bash
# Construction de l'image
docker-compose build --no-cache

# Démarrage du conteneur
docker-compose up -d
```

Pour l'interface graphique sur Windows avec Docker :
```
scripts\run_gui_windows.bat
```

Pour accéder via le navigateur web :
```
scripts\run_web.bat
```

## Problème : SDK .NET 7.0 requis mais non installé

Si vous rencontrez cette erreur :
```
Requested SDK version: 7.0.100
global.json file: ...
Installed SDKs:
9.0.100 [C:\Program Files\dotnet\sdk]
```

### Solution : Utiliser le SDK installé

Exécutez le script qui supprime la restriction de version du SDK :

#### Windows
```
scripts\use_installed_sdk.bat
```

#### Linux/macOS
```
./scripts/use_installed_sdk.sh
```

Cela supprimera la restriction imposée par le fichier `global.json` et vous permettra d'utiliser le SDK .NET installé sur votre système (9.0.100).

### Alternative : Installer le SDK .NET 7.0

Si vous préférez installer le SDK 7.0 :
- Téléchargez et installez le SDK .NET 7.0 depuis [la page des téléchargements .NET](https://dotnet.microsoft.com/download/dotnet/7.0)

## Problème : Avertissement sur la version obsolète de .NET 7.0

Vous pouvez ignorer cet avertissement si vous souhaitez continuer à utiliser .NET 7.0. Il n'empêche pas la compilation.