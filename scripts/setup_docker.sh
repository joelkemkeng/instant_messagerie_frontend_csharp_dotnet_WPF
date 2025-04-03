#!/bin/bash

# Script d'installation de Docker et Docker Compose pour HeticStream
# Usage: ./setup_docker.sh

OS_NAME=$(uname -s)
OS_ARCH=$(uname -m)

echo "===== Installation de Docker pour $OS_NAME ($OS_ARCH) ====="

# Détecter la distribution Linux
if [ "$OS_NAME" = "Linux" ]; then
    if [ -f /etc/os-release ]; then
        . /etc/os-release
        OS_DISTRO=$ID
        OS_VERSION=$VERSION_ID
    else
        echo "Impossible de détecter la distribution Linux"
        exit 1
    fi
fi

install_docker_ubuntu() {
    echo "Installation de Docker pour Ubuntu/Debian..."
    sudo apt-get update
    sudo apt-get install -y apt-transport-https ca-certificates curl software-properties-common

    # Supprimer les anciennes versions si présentes
    sudo apt-get remove -y docker docker-engine docker.io containerd runc || true

    # Ajouter la clé GPG Docker
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -

    # Ajouter le dépôt Docker
    sudo add-apt-repository "deb [arch=$(dpkg --print-architecture)] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"

    # Installer Docker
    sudo apt-get update
    sudo apt-get install -y docker-ce docker-ce-cli containerd.io

    # Installer Docker Compose
    sudo apt-get install -y docker-compose-plugin || {
        sudo curl -L "https://github.com/docker/compose/releases/download/v2.21.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
        sudo chmod +x /usr/local/bin/docker-compose
    }

    # Démarrer et activer Docker
    sudo systemctl start docker
    sudo systemctl enable docker

    # Ajouter l'utilisateur au groupe docker
    sudo usermod -aG docker $USER
    
    echo "🔔 IMPORTANT: Vous devez vous déconnecter et vous reconnecter pour que les changements de groupe prennent effet."
    echo "Ou exécutez: newgrp docker"
}

install_docker_fedora() {
    echo "Installation de Docker pour Fedora/CentOS/RHEL..."
    sudo dnf -y install dnf-plugins-core

    # Ajouter le dépôt Docker
    sudo dnf config-manager --add-repo https://download.docker.com/linux/fedora/docker-ce.repo

    # Installer Docker
    sudo dnf install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

    # Démarrer et activer Docker
    sudo systemctl start docker
    sudo systemctl enable docker

    # Ajouter l'utilisateur au groupe docker
    sudo usermod -aG docker $USER
    
    echo "🔔 IMPORTANT: Vous devez vous déconnecter et vous reconnecter pour que les changements de groupe prennent effet."
    echo "Ou exécutez: newgrp docker"
}

install_docker_macos() {
    echo "Pour macOS, veuillez télécharger et installer Docker Desktop depuis:"
    echo "https://www.docker.com/products/docker-desktop"
    
    if command -v brew &> /dev/null; then
        echo "Ou installez avec Homebrew: brew install --cask docker"
    fi
}

install_docker_windows() {
    echo "Pour Windows, veuillez télécharger et installer Docker Desktop depuis:"
    echo "https://www.docker.com/products/docker-desktop"
    echo "Ou utilisez winget: winget install Docker.DockerDesktop"
}

# Installer selon l'OS
case $OS_NAME in
    Linux)
        case $OS_DISTRO in
            ubuntu|debian|linuxmint)
                install_docker_ubuntu
                ;;
            fedora|centos|rhel)
                install_docker_fedora
                ;;
            *)
                echo "Distribution non supportée: $OS_DISTRO"
                echo "Veuillez installer Docker manuellement: https://docs.docker.com/engine/install/"
                exit 1
                ;;
        esac
        ;;
    Darwin)
        install_docker_macos
        ;;
    MINGW*|MSYS*|CYGWIN*)
        install_docker_windows
        ;;
    *)
        echo "OS non supporté: $OS_NAME"
        echo "Veuillez installer Docker manuellement: https://docs.docker.com/engine/install/"
        exit 1
        ;;
esac

# Vérifier l'installation (sauf pour macOS et Windows qui nécessitent une installation manuelle)
if [ "$OS_NAME" = "Linux" ]; then
    echo -e "\nVérification de l'installation..."
    docker --version
    docker compose version || docker-compose --version

    echo -e "\nTest de l'installation avec un conteneur Hello World..."
    docker run --rm hello-world
fi

echo -e "\nInstallation de Docker terminée!"
echo "Vous pouvez maintenant exécuter HeticStream avec Docker:"
echo "cd HeticStream"
echo "docker-compose up -d"