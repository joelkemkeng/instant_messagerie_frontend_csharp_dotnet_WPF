# Hetic-Stream - Guide de Démarrage Rapide

Ce guide vous permettra de démarrer rapidement avec Hetic-Stream sur n'importe quelle plateforme.

## Prérequis

### Installation avec Docker (Recommandé)

1. **Installer Docker et Docker Compose**

   Exécutez notre script d'installation automatique:
   ```bash
   ./scripts/setup_docker.sh
   ```
   
   Ou suivez la méthode d'installation manuelle pour votre système d'exploitation dans le document `GUIDE_COMPLET_INSTALLATION.md`.

2. **Cloner le dépôt Hetic-Stream**
   ```bash
   git clone https://github.com/yourusername/hetic-stream.git
   cd hetic-stream
   ```

## Démarrage Rapide avec Docker

1. **Lancer l'application en mode conteneur**
   ```bash
   ./scripts/run_app.sh docker
   ```
   
   L'application sera accessible sur le port 5000.

2. **Mode GUI (pour interface graphique)**
   
   Sur Linux avec affichage X11:
   ```bash
   ./scripts/run_app.sh docker-gui
   ```

## Démarrage Natif (Sans Docker)

1. **Installer .NET SDK 7.0**
   ```bash
   ./scripts/install_dotnet.sh
   ```

2. **Compiler et exécuter l'application**
   ```bash
   ./scripts/run_app.sh native
   ```

## Configuration

Toutes les options de configuration sont disponibles via des variables d'environnement:

| Variable | Description | Valeur par défaut |
|----------|-------------|------------------|
| ApiBaseUrl | URL de base de l'API | https://api.heticstream.com |
| ApiEnabled | Activer l'API réelle | false |
| EndpointLogin | Endpoint de connexion | /auth/login |
| EndpointRegister | Endpoint d'inscription | /auth/register |
| EndpointChannels | Endpoint des canaux | /channels |
| EndpointMessages | Endpoint des messages | /messages |
| ImageAssetsPath | Chemin des images | Assets/Images |
| LightThemeEnabled | Activer le thème clair | false |

## Déploiement en Production

### AWS
```bash
./scripts/deploy_aws.sh [region] [account_id]
```

### Azure
```bash
./scripts/deploy_azure.sh [location] [resource_group]
```

### Google Cloud
```bash
./scripts/deploy_gcp.sh [project_id] [region]
```

## Mode Production Local
```bash
./scripts/run_app.sh prod
```

## Documentation Complète

Pour plus d'informations, consultez:
- `GUIDE_COMPLET_INSTALLATION.md` - Guide d'installation détaillé
- `DOCKER_README.md` - Guide d'utilisation de Docker
- `DEPLOYMENT.md` - Instructions de déploiement complètes
- `API_DOCUMENTATION.md` - Documentation de l'API

## Résolution des Problèmes

### Docker: L'application ne démarre pas
```bash
docker-compose logs
```

### Native: Erreur de compilation
```bash
dotnet restore
dotnet build -v detailed
```

### L'interface graphique ne s'affiche pas
Sur Linux:
```bash
xhost +local:docker
```

Sur macOS, installer XQuartz:
```bash
brew install --cask xquartz
```

## Accès à l'Interface Web
Ouvrez votre navigateur à l'adresse:
- http://localhost:5000