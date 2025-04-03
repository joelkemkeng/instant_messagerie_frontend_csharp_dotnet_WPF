#!/bin/bash

# Script pour exécuter l'application Hetic-Stream
# Usage: ./run_app.sh [mode]
# Modes: docker, native, gui
# Exemple: ./run_app.sh docker

MODE=${1:-"docker"}

echo "===== Exécution d'Hetic-Stream en mode $MODE ====="

case $MODE in
    docker)
        echo "Exécution avec Docker..."
        docker-compose up -d
        echo "Hetic-Stream démarré en arrière-plan."
        echo "Pour voir les logs: docker-compose logs -f"
        echo "Pour arrêter: docker-compose down"
        ;;
        
    docker-gui)
        echo "Exécution avec Docker (interface graphique)..."
        xhost +local:docker 2>/dev/null || {
            echo "⚠️ Impossible d'exécuter xhost. Si vous êtes sur Linux, installez xhost:"
            echo "sudo apt-get install -y x11-xserver-utils"
            exit 1
        }
        docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d
        echo "Hetic-Stream démarré avec interface graphique."
        echo "Pour voir les logs: docker-compose logs -f"
        echo "Pour arrêter: docker-compose down"
        ;;
        
    native)
        echo "Exécution native..."
        # Vérifier si dotnet est installé
        if ! command -v dotnet &> /dev/null; then
            echo "❌ .NET SDK n'est pas installé."
            echo "Veuillez l'installer avec: ./scripts/install_dotnet.sh"
            exit 1
        fi
        
        cd ..
        dotnet restore
        dotnet build
        dotnet run --project HeticStream.UI
        ;;
        
    prod)
        echo "Exécution en mode production..."
        docker-compose -f docker-compose.prod.yml up -d
        echo "Hetic-Stream démarré en mode production."
        echo "Pour voir les logs: docker-compose -f docker-compose.prod.yml logs -f"
        echo "Pour arrêter: docker-compose -f docker-compose.prod.yml down"
        ;;
        
    *)
        echo "Mode non reconnu: $MODE"
        echo "Modes disponibles: docker, docker-gui, native, prod"
        exit 1
        ;;
esac

echo -e "\n👍 Hetic-Stream est en cours d'exécution!"
echo "Reportez-vous à la documentation pour plus d'informations."