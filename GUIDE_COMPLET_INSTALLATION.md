# Guide Complet d'Installation et Déploiement d'Hetic-Stream

Ce document fournit des instructions détaillées pour installer et déployer l'application Hetic-Stream sur différentes plateformes.

## Table des matières

1. [Prérequis](#prérequis)
2. [Installation avec Docker (Recommandé)](#installation-avec-docker-recommandé)
   - [Linux](#docker-sur-linux)
   - [Windows](#docker-sur-windows)
   - [macOS](#docker-sur-macos)
3. [Installation Native](#installation-native)
   - [Linux](#installation-native-sur-linux)
   - [Windows](#installation-native-sur-windows)
   - [macOS](#installation-native-sur-macos)
4. [Déploiement en Production](#déploiement-en-production)
   - [Serveur Linux](#serveur-linux)
   - [Cloud (AWS, Azure, GCP)](#cloud)
   - [Kubernetes](#kubernetes)
5. [Configuration](#configuration)
6. [Résolution des problèmes](#résolution-des-problèmes)

## Prérequis

### Matériel minimum recommandé
- CPU: 2 cœurs
- RAM: 2 Go
- Espace disque: 1 Go

### Réseaux et Ports
- Port 5000 ouvert pour l'accès à l'application
- Connexion Internet pour les installations et mises à jour

## Installation avec Docker (Recommandé)

L'installation avec Docker est la méthode la plus simple et portable pour tous les systèmes d'exploitation.

### Docker sur Linux

#### 1. Installation de Docker et Docker Compose

**Ubuntu/Debian**:
```bash
# Mettre à jour les paquets
sudo apt-get update

# Installer les prérequis
sudo apt-get install -y apt-transport-https ca-certificates curl software-properties-common

# Ajouter la clé GPG Docker
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -

# Ajouter le dépôt Docker
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"

# Mettre à jour la liste des paquets
sudo apt-get update

# Installer Docker et Docker Compose
sudo apt-get install -y docker-ce docker-compose

# Démarrer et activer Docker
sudo systemctl start docker
sudo systemctl enable docker

# Ajouter votre utilisateur au groupe docker (pour éviter d'utiliser sudo)
sudo usermod -aG docker $USER
```

**Redémarrez votre session pour que les changements de groupe prennent effet.**

**Fedora/CentOS/RHEL**:
```bash
# Installer les outils nécessaires
sudo dnf install -y dnf-plugins-core

# Ajouter le dépôt Docker
sudo dnf config-manager --add-repo https://download.docker.com/linux/fedora/docker-ce.repo

# Installer Docker
sudo dnf install -y docker-ce docker-compose-plugin

# Démarrer et activer Docker
sudo systemctl start docker
sudo systemctl enable docker

# Ajouter votre utilisateur au groupe docker
sudo usermod -aG docker $USER
```

**Redémarrez votre session pour que les changements de groupe prennent effet.**

#### 2. Cloner le dépôt Hetic-Stream
```bash
git clone https://github.com/yourusername/hetic-stream.git
cd hetic-stream
```

#### 3. Démarrer l'application
```bash
docker-compose up -d
```

Pour l'interface graphique:
```bash
xhost +local:docker
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d
```

#### 4. Accéder à l'application
- Interface graphique: L'application s'ouvre automatiquement
- Si vous utilisez un serveur distant, configurez l'accès X11 forwarding dans SSH

### Docker sur Windows

#### 1. Installation de Docker Desktop

1. Téléchargez Docker Desktop depuis [le site officiel](https://www.docker.com/products/docker-desktop/)
2. Exécutez le programme d'installation
3. Suivez les instructions d'installation (activation de WSL2 si demandé)
4. Lancez Docker Desktop après l'installation

#### 2. Cloner le dépôt Hetic-Stream
Ouvrez PowerShell:
```powershell
git clone https://github.com/yourusername/hetic-stream.git
cd hetic-stream
```

#### 3. Démarrer l'application
```powershell
docker-compose up -d
```

#### 4. Accéder à l'application
- Interface graphique: L'application s'ouvre automatiquement
- Si vous avez des problèmes, consultez la section [Résolution des problèmes](#résolution-des-problèmes)

### Docker sur macOS

#### 1. Installation de Docker Desktop

1. Téléchargez Docker Desktop depuis [le site officiel](https://www.docker.com/products/docker-desktop/)
2. Installez l'application en la déplaçant dans le dossier Applications
3. Lancez Docker Desktop et suivez les instructions

#### 2. Cloner le dépôt Hetic-Stream
Ouvrez Terminal:
```bash
git clone https://github.com/yourusername/hetic-stream.git
cd hetic-stream
```

#### 3. Démarrer l'application
```bash
docker-compose up -d
```

Pour l'interface graphique:
```bash
# Installation de XQuartz (nécessaire pour X11)
brew install --cask xquartz

# Configurer XQuartz
open -a XQuartz
# Dans les préférences XQuartz, activez "Allow connections from network clients"
# Redémarrer XQuartz

# Configurer l'accès
xhost +localhost

# Démarrer l'application
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d
```

#### 4. Accéder à l'application
- Interface graphique: L'application s'ouvre via XQuartz
- Si vous avez des problèmes, consultez la section [Résolution des problèmes](#résolution-des-problèmes)

## Installation Native

Si vous préférez une installation native sans Docker, suivez ces instructions.

### Installation Native sur Linux

#### 1. Installation de .NET SDK

**Ubuntu/Debian**:
```bash
# Installer les prérequis
sudo apt-get update
sudo apt-get install -y apt-transport-https

# Télécharger et installer le package Microsoft
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Installer le SDK .NET
sudo apt-get update
sudo apt-get install -y dotnet-sdk-7.0

# Installer les dépendances pour Avalonia UI
sudo apt-get install -y libx11-dev libxcursor-dev libxrandr-dev libxinerama-dev libxi-dev libgl1-mesa-dev libasound2
```

**Fedora/CentOS/RHEL**:
```bash
# Importer la clé Microsoft
sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc

# Ajouter le dépôt Microsoft
sudo dnf install -y dnf-plugins-core
sudo dnf copr enable @dotnet-sig/dotnet
sudo dnf install -y dotnet-sdk-7.0

# Installer les dépendances pour Avalonia UI
sudo dnf install -y libX11-devel libXcursor-devel libXrandr-devel libXinerama-devel libXi-devel mesa-libGL-devel alsa-lib
```

#### 2. Cloner et configurer le projet
```bash
git clone https://github.com/yourusername/hetic-stream.git
cd hetic-stream
```

#### 3. Compiler et exécuter l'application
```bash
# Restaurer les dépendances
dotnet restore

# Compiler le projet
dotnet build

# Exécuter l'application
dotnet run --project HeticStream.UI
```

### Installation Native sur Windows

#### 1. Installation de .NET SDK

1. Téléchargez le .NET SDK 7.0 depuis [le site officiel de .NET](https://dotnet.microsoft.com/download/dotnet/7.0)
2. Exécutez le programme d'installation et suivez les instructions
3. Vérifiez l'installation en ouvrant une invite de commande et en tapant:
   ```
   dotnet --version
   ```

#### 2. Cloner et configurer le projet
```powershell
git clone https://github.com/yourusername/hetic-stream.git
cd hetic-stream
```

#### 3. Compiler et exécuter l'application
```powershell
# Restaurer les dépendances
dotnet restore

# Compiler le projet
dotnet build

# Exécuter l'application
dotnet run --project HeticStream.UI
```

### Installation Native sur macOS

#### 1. Installation de .NET SDK

1. Téléchargez le .NET SDK 7.0 depuis [le site officiel de .NET](https://dotnet.microsoft.com/download/dotnet/7.0)
2. Installez le package téléchargé
3. Vérifiez l'installation en ouvrant Terminal et en tapant:
   ```
   dotnet --version
   ```

#### 2. Cloner et configurer le projet
```bash
git clone https://github.com/yourusername/hetic-stream.git
cd hetic-stream
```

#### 3. Compiler et exécuter l'application
```bash
# Restaurer les dépendances
dotnet restore

# Compiler le projet
dotnet build

# Exécuter l'application
dotnet run --project HeticStream.UI
```

## Déploiement en Production

### Serveur Linux

#### 1. Installation des prérequis
Suivez les instructions d'installation pour [Docker sur Linux](#docker-sur-linux) ou [Installation Native sur Linux](#installation-native-sur-linux).

#### 2. Configuration d'un service systemd (installation native)
```bash
# Créer un fichier de service
sudo nano /etc/systemd/system/hetic-stream.service
```

Contenu du fichier:
```ini
[Unit]
Description=Hetic-Stream Application
After=network.target

[Service]
WorkingDirectory=/chemin/vers/hetic-stream/HeticStream.UI/bin/Release/net7.0
ExecStart=/usr/bin/dotnet /chemin/vers/hetic-stream/HeticStream.UI/bin/Release/net7.0/HeticStream.UI.dll
Restart=always
RestartSec=10
SyslogIdentifier=hetic-stream
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ApiBaseUrl=https://api.heticstream.com
Environment=ApiEnabled=true
# Autres variables d'environnement...

[Install]
WantedBy=multi-user.target
```

Activer et démarrer le service:
```bash
sudo systemctl enable hetic-stream
sudo systemctl start hetic-stream
```

#### 3. Configuration avec Nginx (installation native)
```bash
sudo apt-get install -y nginx

# Configurer Nginx
sudo nano /etc/nginx/sites-available/hetic-stream
```

Contenu du fichier:
```nginx
server {
    listen 80;
    server_name heticstream.example.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Activer le site et redémarrer Nginx:
```bash
sudo ln -s /etc/nginx/sites-available/hetic-stream /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

#### 4. Configuration avec Docker Compose (installation Docker)

Créez un fichier docker-compose.prod.yml:
```yaml
version: '3.8'

services:
  hetic-stream:
    build:
      context: .
      dockerfile: Dockerfile
    restart: always
    environment:
      - ApiBaseUrl=https://api.heticstream.com
      - ApiEnabled=true
      - EndpointLogin=/auth/login
      - EndpointRegister=/auth/register
      - EndpointChannels=/channels
      - EndpointMessages=/messages
      - ImageAssetsPath=Assets/Images
      - LightThemeEnabled=false
    ports:
      - "5000:5000"
    volumes:
      - ./logs:/app/logs

  nginx:
    image: nginx:latest
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/conf.d/default.conf
      - ./certbot/conf:/etc/letsencrypt
      - ./certbot/www:/var/www/certbot
    depends_on:
      - hetic-stream
    restart: always

  certbot:
    image: certbot/certbot
    volumes:
      - ./certbot/conf:/etc/letsencrypt
      - ./certbot/www:/var/www/certbot
    entrypoint: "/bin/sh -c 'trap exit TERM; while :; do certbot renew; sleep 12h & wait $!; done;'"
```

Créez un fichier nginx.conf:
```nginx
server {
    listen 80;
    server_name heticstream.example.com;

    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
    }

    location / {
        return 301 https://$host$request_uri;
    }
}

server {
    listen 443 ssl;
    server_name heticstream.example.com;

    ssl_certificate /etc/letsencrypt/live/heticstream.example.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/heticstream.example.com/privkey.pem;

    location / {
        proxy_pass http://hetic-stream:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Démarrer les services:
```bash
docker-compose -f docker-compose.prod.yml up -d
```

### Cloud

#### AWS Elastic Container Service (ECS)

1. Créez un dépôt ECR (Elastic Container Registry):
```bash
aws ecr create-repository --repository-name hetic-stream
```

2. Authentifiez Docker à ECR:
```bash
aws ecr get-login-password --region <votre-region> | docker login --username AWS --password-stdin <votre-id-compte>.dkr.ecr.<votre-region>.amazonaws.com
```

3. Construisez et poussez l'image:
```bash
docker build -t hetic-stream .
docker tag hetic-stream:latest <votre-id-compte>.dkr.ecr.<votre-region>.amazonaws.com/hetic-stream:latest
docker push <votre-id-compte>.dkr.ecr.<votre-region>.amazonaws.com/hetic-stream:latest
```

4. Créez un cluster ECS:
```bash
aws ecs create-cluster --cluster-name hetic-stream-cluster
```

5. Créez une définition de tâche (task-definition.json):
```json
{
  "family": "hetic-stream",
  "executionRoleArn": "arn:aws:iam::<votre-id-compte>:role/ecsTaskExecutionRole",
  "networkMode": "awsvpc",
  "containerDefinitions": [
    {
      "name": "hetic-stream",
      "image": "<votre-id-compte>.dkr.ecr.<votre-region>.amazonaws.com/hetic-stream:latest",
      "essential": true,
      "portMappings": [
        {
          "containerPort": 5000,
          "hostPort": 5000,
          "protocol": "tcp"
        }
      ],
      "environment": [
        { "name": "ApiBaseUrl", "value": "https://api.heticstream.com" },
        { "name": "ApiEnabled", "value": "true" },
        { "name": "EndpointLogin", "value": "/auth/login" },
        { "name": "EndpointRegister", "value": "/auth/register" },
        { "name": "EndpointChannels", "value": "/channels" },
        { "name": "EndpointMessages", "value": "/messages" },
        { "name": "ImageAssetsPath", "value": "Assets/Images" },
        { "name": "LightThemeEnabled", "value": "false" }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/hetic-stream",
          "awslogs-region": "<votre-region>",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ],
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512"
}
```

6. Enregistrez la définition de tâche:
```bash
aws ecs register-task-definition --cli-input-json file://task-definition.json
```

7. Créez un service:
```bash
aws ecs create-service --cluster hetic-stream-cluster --service-name hetic-stream-service --task-definition hetic-stream --desired-count 1 --launch-type FARGATE --network-configuration "awsvpcConfiguration={subnets=[<votre-subnet>],securityGroups=[<votre-groupe-securite>],assignPublicIp=ENABLED}"
```

#### Microsoft Azure Container Instances

1. Créez un groupe de ressources:
```bash
az group create --name hetic-stream-rg --location westeurope
```

2. Créez un registre de conteneurs:
```bash
az acr create --resource-group hetic-stream-rg --name heticstreamacr --sku Basic
```

3. Authentifiez-vous au registre:
```bash
az acr login --name heticstreamacr
```

4. Construisez et poussez l'image:
```bash
docker build -t heticstreamacr.azurecr.io/hetic-stream:latest .
docker push heticstreamacr.azurecr.io/hetic-stream:latest
```

5. Créez une instance de conteneur:
```bash
az container create --resource-group hetic-stream-rg --name hetic-stream --image heticstreamacr.azurecr.io/hetic-stream:latest --cpu 1 --memory 1.5 --registry-login-server heticstreamacr.azurecr.io --registry-username <votre-nom-utilisateur> --registry-password <votre-mot-de-passe> --dns-name-label hetic-stream --ports 5000 --environment-variables ApiBaseUrl=https://api.heticstream.com ApiEnabled=true EndpointLogin=/auth/login EndpointRegister=/auth/register EndpointChannels=/channels EndpointMessages=/messages ImageAssetsPath=Assets/Images LightThemeEnabled=false
```

#### Google Cloud Run

1. Construisez et poussez l'image:
```bash
gcloud builds submit --tag gcr.io/<votre-projet>/hetic-stream
```

2. Déployez l'image:
```bash
gcloud run deploy hetic-stream --image gcr.io/<votre-projet>/hetic-stream --platform managed --set-env-vars="ApiBaseUrl=https://api.heticstream.com,ApiEnabled=true,EndpointLogin=/auth/login,EndpointRegister=/auth/register,EndpointChannels=/channels,EndpointMessages=/messages,ImageAssetsPath=Assets/Images,LightThemeEnabled=false"
```

### Kubernetes

1. Créez un fichier de déploiement (deployment.yaml):
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: hetic-stream
spec:
  replicas: 2
  selector:
    matchLabels:
      app: hetic-stream
  template:
    metadata:
      labels:
        app: hetic-stream
    spec:
      containers:
      - name: hetic-stream
        image: <votre-registry>/hetic-stream:latest
        ports:
        - containerPort: 5000
        env:
        - name: ApiBaseUrl
          value: "https://api.heticstream.com"
        - name: ApiEnabled
          value: "true"
        - name: EndpointLogin
          value: "/auth/login"
        - name: EndpointRegister
          value: "/auth/register"
        - name: EndpointChannels
          value: "/channels"
        - name: EndpointMessages
          value: "/messages"
        - name: ImageAssetsPath
          value: "Assets/Images"
        - name: LightThemeEnabled
          value: "false"
        resources:
          limits:
            cpu: "0.5"
            memory: "512Mi"
          requests:
            cpu: "0.2"
            memory: "256Mi"
```

2. Créez un fichier de service (service.yaml):
```yaml
apiVersion: v1
kind: Service
metadata:
  name: hetic-stream
spec:
  selector:
    app: hetic-stream
  ports:
  - port: 80
    targetPort: 5000
  type: LoadBalancer
```

3. Appliquez les fichiers:
```bash
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml
```

## Configuration

### Variables d'environnement

Voici les variables d'environnement disponibles pour configurer l'application:

| Variable | Description | Valeur par défaut |
|----------|-------------|------------------|
| ApiBaseUrl | URL de base de l'API | https://api.heticstream.com |
| ApiEnabled | Activer/désactiver l'API réelle | false |
| EndpointLogin | Chemin de l'endpoint de connexion | /auth/login |
| EndpointRegister | Chemin de l'endpoint d'inscription | /auth/register |
| EndpointChannels | Chemin de l'endpoint des canaux | /channels |
| EndpointMessages | Chemin de l'endpoint des messages | /messages |
| ImageAssetsPath | Chemin des ressources images | Assets/Images |
| LightThemeEnabled | Activer le thème clair | false |

### Configuration de l'API

Pour configurer l'API:

1. Mode simulation (par défaut):
```
ApiEnabled=false
```

2. Mode réel (API active):
```
ApiEnabled=true
ApiBaseUrl=https://votre-api.exemple.com
```

## Résolution des problèmes

### Problèmes courants

#### L'interface graphique ne s'affiche pas avec Docker

**Linux**:
- Vérifiez que X11 est configuré correctement: `xhost +local:docker`
- Vérifiez que le volume X11 est monté correctement

**macOS**:
- Vérifiez que XQuartz est installé et configuré: `brew install --cask xquartz`
- Assurez-vous que "Allow connections from network clients" est activé dans les préférences XQuartz

**Windows**:
- Utilisez WSLg pour Windows 11 ou un serveur X11 comme VcXsrv pour Windows 10

#### Erreurs de compilation (installation native)

1. Vérifiez que vous avez la bonne version de .NET SDK:
```bash
dotnet --version
```

2. Restaurez les dépendances:
```bash
dotnet restore
```

3. Vérifiez les erreurs de compilation:
```bash
dotnet build -v detailed
```

#### Problèmes de connexion à l'API

1. Vérifiez la configuration de l'API:
```bash
# Docker
docker-compose config

# Native
echo $ApiBaseUrl
echo $ApiEnabled
```

2. Vérifiez que l'API est accessible:
```bash
curl -v $ApiBaseUrl
```

3. Utilisez le mode simulation pour tester sans API:
```bash
export ApiEnabled=false
```

#### Problèmes de déploiement Docker

1. Vérifiez les logs Docker:
```bash
docker-compose logs
```

2. Vérifiez l'état des conteneurs:
```bash
docker-compose ps
```

3. Entrez dans le conteneur pour déboguer:
```bash
docker-compose exec hetic-stream bash
```

### Journaux d'application

Les journaux sont disponibles:

- **Docker**: `docker-compose logs -f`
- **Systemd**: `sudo journalctl -u hetic-stream`
- **Logs applicatifs**: Consultez le dossier `logs` dans le répertoire de l'application

### Obtenir de l'aide

Si vous rencontrez des problèmes:
- Consultez la documentation
- Ouvrez une issue sur GitHub
- Contactez l'équipe de support à support@heticstream.com