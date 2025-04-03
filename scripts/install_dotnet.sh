#!/bin/bash

# Script d'installation de .NET SDK pour HeticStream
# Usage: ./install_dotnet.sh [version]
# Exemple: ./install_dotnet.sh 7.0

VERSION=${1:-"7.0"}
OS_NAME=$(uname -s)
OS_ARCH=$(uname -m)

echo "===== Installation du SDK .NET $VERSION pour $OS_NAME ($OS_ARCH) ====="

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

install_dotnet_ubuntu() {
    echo "Installation pour Ubuntu/Debian..."
    sudo apt-get update
    sudo apt-get install -y apt-transport-https
    wget https://packages.microsoft.com/config/ubuntu/$OS_VERSION/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    sudo apt-get update
    sudo apt-get install -y dotnet-sdk-$VERSION
    
    # Installer les dépendances Avalonia
    sudo apt-get install -y libx11-dev libxcursor-dev libxrandr-dev libxinerama-dev libxi-dev libgl1-mesa-dev libasound2
}

install_dotnet_fedora() {
    echo "Installation pour Fedora/CentOS/RHEL..."
    sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc
    sudo dnf install -y dnf-plugins-core
    sudo dnf copr enable @dotnet-sig/dotnet
    sudo dnf install -y dotnet-sdk-$VERSION
    
    # Installer les dépendances Avalonia
    sudo dnf install -y libX11-devel libXcursor-devel libXrandr-devel libXinerama-devel libXi-devel mesa-libGL-devel alsa-lib
}

install_dotnet_macos() {
    echo "Installation pour macOS..."
    brew install --cask dotnet-sdk
    
    # Ou avec script Microsoft
    # curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel $VERSION
}

install_dotnet_windows() {
    echo "Pour Windows, veuillez télécharger et installer le SDK .NET depuis:"
    echo "https://dotnet.microsoft.com/download/dotnet/$VERSION"
    echo "Ou utiliser winget: winget install Microsoft.DotNet.SDK.$VERSION"
}

# Installer selon l'OS
case $OS_NAME in
    Linux)
        case $OS_DISTRO in
            ubuntu|debian|linuxmint)
                install_dotnet_ubuntu
                ;;
            fedora|centos|rhel)
                install_dotnet_fedora
                ;;
            *)
                echo "Distribution non supportée: $OS_DISTRO"
                echo "Veuillez installer manuellement: https://dotnet.microsoft.com/download/dotnet/$VERSION"
                exit 1
                ;;
        esac
        ;;
    Darwin)
        install_dotnet_macos
        ;;
    MINGW*|MSYS*|CYGWIN*)
        install_dotnet_windows
        ;;
    *)
        echo "OS non supporté: $OS_NAME"
        echo "Veuillez installer manuellement: https://dotnet.microsoft.com/download/dotnet/$VERSION"
        exit 1
        ;;
esac

# Vérifier l'installation
echo -e "\nVérification de l'installation..."
dotnet --version

echo -e "\nInstallation du SDK .NET $VERSION terminée!"
echo "Vous pouvez maintenant compiler et exécuter HeticStream:"
echo "cd HeticStream"
echo "dotnet restore"
echo "dotnet build"
echo "dotnet run --project HeticStream.UI"