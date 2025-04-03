# Guide Docker pour Hetic-Stream

Ce guide explique comment utiliser Docker pour exécuter l'application Hetic-Stream sans avoir besoin d'installer .NET localement.

## Prérequis

- Docker
- Docker Compose

## Dépendances incluses

Le Dockerfile inclut automatiquement toutes les dépendances nécessaires pour exécuter Avalonia UI dans un conteneur Linux, notamment :
- Bibliothèques X11 pour l'affichage graphique
- Fontconfig et libfreetype pour la gestion des polices
- SkiaSharp et ses dépendances (libicu, libharfbuzz, etc.) pour le rendu graphique

## Installation de Docker

### Ubuntu
```bash
sudo apt-get update
sudo apt-get install docker.io docker-compose
sudo systemctl enable --now docker
sudo usermod -aG docker $USER
```
(Déconnectez-vous et reconnectez-vous pour que les changements de groupe prennent effet)

### Windows
Téléchargez et installez Docker Desktop depuis [le site officiel](https://www.docker.com/products/docker-desktop)

### macOS
Téléchargez et installez Docker Desktop depuis [le site officiel](https://www.docker.com/products/docker-desktop)

## Construction et démarrage de l'application

1. Accédez au répertoire racine du projet (où se trouve le fichier `docker-compose.yml`)

2. Construisez et démarrez l'application:
```bash
docker-compose up -d
```

3. Pour voir les logs:
```bash
docker-compose logs -f
```

4. Pour arrêter l'application:
```bash
docker-compose down
```

## Configuration

Vous pouvez modifier les variables d'environnement dans le fichier `docker-compose.yml`:

```yaml
environment:
  - ApiBaseUrl=https://api.heticstream.com
  - ApiEnabled=false
  - EndpointLogin=/auth/login
  - EndpointRegister=/auth/register
  - EndpointChannels=/channels
  - EndpointMessages=/messages
  - ImageAssetsPath=Assets/Images
  - LightThemeEnabled=false
```

## Accès à l'application

### Mode GUI

Si vous exécutez Docker sur un système avec un affichage graphique, vous devrez ajouter le support pour afficher l'interface utilisateur:

#### Linux

```bash
# Autoriser l'accès à l'affichage X11 pour Docker
xhost +local:docker

# Démarrer l'application avec le support GUI
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d

# Si vous rencontrez des problèmes de rendu graphique, essayez ces commandes :
# Pour les problèmes de dépendances manquantes
docker exec -it hetic-stream apt-get update && apt-get install -y libfontconfig1 fontconfig libharfbuzz0b

# Pour vérifier les erreurs spécifiques
docker logs hetic-stream
```

#### Windows avec WSL2 et Docker Desktop

1. Installez un serveur X comme VcXsrv ou X410
2. Configurez-le pour accepter les connexions externes
3. Dans WSL2, configurez la variable DISPLAY :
```bash
export DISPLAY=$(ip route | grep default | awk '{print $3}'):0
```
4. Puis exécutez :
```bash
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d
```

#### macOS avec Docker Desktop

1. Installez XQuartz
2. Ouvrez XQuartz et dans les préférences, activez "Autoriser les connexions de clients réseau"
3. Redémarrez XQuartz
4. Exécutez :
```bash
xhost +localhost
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d
```

## Déploiement

### Déploiement sur un serveur

1. Copiez les fichiers `Dockerfile`, `docker-compose.yml` et `.dockerignore` sur votre serveur

2. Copiez le code source de l'application ou clonez le dépôt Git

3. Exécutez:
```bash
docker-compose up -d
```

### Déploiement sur un service cloud

#### AWS Elastic Container Service (ECS)

1. Créez un dépôt ECR (Elastic Container Registry)
2. Construisez et poussez l'image:
```bash
aws ecr get-login-password --region <votre-region> | docker login --username AWS --password-stdin <votre-id-compte>.dkr.ecr.<votre-region>.amazonaws.com
docker build -t hetic-stream .
docker tag hetic-stream:latest <votre-id-compte>.dkr.ecr.<votre-region>.amazonaws.com/hetic-stream:latest
docker push <votre-id-compte>.dkr.ecr.<votre-region>.amazonaws.com/hetic-stream:latest
```
3. Créez un cluster ECS et un service utilisant cette image

#### Google Cloud Run

```bash
gcloud builds submit --tag gcr.io/<votre-projet>/hetic-stream
gcloud run deploy hetic-stream --image gcr.io/<votre-projet>/hetic-stream --platform managed
```

#### Azure Container Instances

```bash
az acr build --registry <votre-registre> --image hetic-stream:latest .
az container create --resource-group <votre-groupe-ressources> --name hetic-stream --image <votre-registre>.azurecr.io/hetic-stream:latest --dns-name-label hetic-stream --ports 80
```

## Résolution des problèmes courants

### Erreurs liées aux bibliothèques natives

Si vous rencontrez des erreurs comme `libfontconfig.so.1: cannot open shared object file` ou `libSkiaSharp.so: cannot open shared object file`, c'est que des dépendances native sont manquantes. Solutions :

1. Reconstruisez l'image Docker avec les dépendances ajoutées :
```bash
docker-compose build --no-cache
docker-compose up -d
```

2. Installez manuellement les dépendances manquantes dans le conteneur existant :
```bash
docker exec -it hetic-stream bash
apt-get update && apt-get install -y libfontconfig1 fontconfig libicu-dev libharfbuzz-dev
```

### Problèmes d'affichage GUI

1. **Erreur "Cannot open display"** :
   - Vérifiez que vous avez bien exécuté `xhost +local:docker` sur Linux
   - Sur WSL2/Windows, vérifiez que votre serveur X est en cours d'exécution et configuré correctement
   - Sur macOS, vérifiez que XQuartz est en cours d'exécution avec les bonnes permissions

2. **Écran noir ou interface qui ne s'affiche pas** :
   - Vérifiez les logs : `docker-compose logs -f`
   - Assurez-vous que les volumes X11 sont correctement montés

3. **Performances graphiques lentes** :
   - Ajoutez `--privileged` à votre commande docker-compose pour un accès complet au matériel graphique
   - Utilisez l'option de rendu logiciel : `export LIBGL_ALWAYS_SOFTWARE=1` avant de lancer le conteneur

## Avantages de l'utilisation de Docker

1. **Portabilité**: L'application fonctionne de manière identique sur n'importe quel système pouvant exécuter Docker
2. **Isolation**: Pas besoin d'installer .NET localement
3. **Reproductibilité**: Environnement cohérent de développement à production
4. **Facilité de déploiement**: Déployez facilement sur n'importe quelle plateforme cloud
5. **Scalabilité**: Facile à mettre à l'échelle horizontalement