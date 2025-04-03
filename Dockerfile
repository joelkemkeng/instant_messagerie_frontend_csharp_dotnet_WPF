FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copier les fichiers projet et restaurer les dépendances
COPY ["HeticStream.Core/HeticStream.Core.csproj", "HeticStream.Core/"]
COPY ["HeticStream.Services/HeticStream.Services.csproj", "HeticStream.Services/"]
COPY ["HeticStream.UI/HeticStream.UI.csproj", "HeticStream.UI/"]
RUN dotnet restore "HeticStream.UI/HeticStream.UI.csproj"

# Copier le reste du code source
COPY . .

# Construire et publier l'application
RUN dotnet publish "HeticStream.UI/HeticStream.UI.csproj" -c Release -o /app/publish

# Image finale
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app/publish .

# Configuration des variables d'environnement
ENV ApiBaseUrl="https://api.heticstream.com"
ENV ApiEnabled="false"
ENV EndpointLogin="/auth/login"
ENV EndpointRegister="/auth/register"
ENV EndpointChannels="/channels"
ENV EndpointMessages="/messages"
ENV ImageAssetsPath="Assets/Images"
ENV LightThemeEnabled="false"
ENV HEADLESS="true"

# Installation des dépendances pour Avalonia UI sur Linux
RUN apt-get update && apt-get install -y \
    libx11-dev \
    libxcursor-dev \
    libxrandr-dev \
    libxinerama-dev \
    libxi-dev \
    libgl1-mesa-dev \
    libasound2 \
    libfontconfig1 \
    fontconfig \
    libfreetype6 \
    libpng16-16 \
    libjpeg62-turbo \
    libicu-dev \
    libharfbuzz-dev \
    && rm -rf /var/lib/apt/lists/*

# Définir le point d'entrée avec des options
# Par défaut on démarre en mode headless, désactiver avec --env HEADLESS=false
ENTRYPOINT ["dotnet", "HeticStream.UI.dll", "--headless"]