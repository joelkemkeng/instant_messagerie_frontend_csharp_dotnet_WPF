# Hetic-Stream Chat Application

Hetic-Stream is a cross-platform chat application built with Avalonia UI, C#, and .NET using the MVVM architecture. The application provides a modern user interface similar to Discord, with features for authentication, channel management, and messaging.

## Features

- Modern Discord-inspired UI with dark and light theme support
- Sign-in and registration screens
- Channel list with support for direct and group channels
- Message viewing and sending
- User online status
- Styled notifications for success, error, and info messages
- Cross-platform support for Windows, macOS, Linux, and Web

## Project Structure

The application is organized into three main projects:

1. **HeticStream.Core**: Contains models, interfaces, and configuration classes
2. **HeticStream.Services**: Contains service implementations for authentication, channels, and users
3. **HeticStream.UI**: Contains the Avalonia UI application with views and view models

## Environment Variables

The application uses the following environment variables for configuration:

- `ApiBaseUrl`: Base URL for the API (default: https://api.heticstream.com)
- `ApiEnabled`: Boolean flag to enable or disable the real API (default: false)
- `EndpointLogin`: Endpoint path for login (default: /auth/login)
- `EndpointRegister`: Endpoint path for registration (default: /auth/register)
- `EndpointChannels`: Endpoint path for channels (default: /channels)
- `EndpointMessages`: Endpoint path for messages (default: /messages)
- `ImageAssetsPath`: Path to image assets (default: Assets/Images)
- `LightThemeEnabled`: Boolean flag to enable light theme (default: false)

## API Simulation

When `ApiEnabled` is set to `false`, the application will simulate API calls and log requests and responses to the console. This is useful for development and testing without a real backend.

## Building and Running

### Prerequisites

- .NET 7.0 SDK or later
- Docker (alternative method)

### Development Mode

#### Windows, macOS, Linux

```
cd HeticStream
dotnet restore
dotnet build
dotnet run --project HeticStream.UI
```

### Docker Mode

L'application peut fonctionner en deux modes avec Docker :

#### Mode Headless (Sans Interface)

Idéal pour les serveurs ou environnements sans affichage graphique :

```bash
# Construction et démarrage en mode headless
docker-compose build
docker-compose up -d
```

Les services backend seront démarrés mais aucune interface graphique ne sera affichée.

#### Mode GUI (Avec Interface)

Pour exécuter l'application avec son interface graphique :

**Linux**
```bash
# Autoriser l'accès à X11 pour Docker
xhost +local:docker

# Démarrer avec le mode GUI
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d
```

**Windows avec WSL2**
```
# Installer un serveur X comme VcXsrv
# Le configurer pour accepter les connexions externes
# Dans WSL2, configurer la variable DISPLAY

# Puis exécuter
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d
```

**macOS**
```bash
# Installer XQuartz et activer "Autoriser les connexions de clients réseau"
xhost +localhost
docker-compose -f docker-compose.yml -f docker-compose.gui.yml up -d
```

Vous pouvez également utiliser les scripts prêts à l'emploi :
- `scripts/run_gui_linux.sh` pour Linux
- `scripts/run_gui_windows.bat` pour Windows

### Web (WebAssembly)

For web deployment, you'll need to add the WebAssembly package reference to the UI project:

```xml
<PackageReference Include="Avalonia.Browser" Version="11.0.5" />
```

Then build the WebAssembly application:

```
cd HeticStream
dotnet restore
dotnet build
dotnet publish HeticStream.UI -c Release -f net7.0 --runtime browser-wasm -o ./publish
```

The published files can be hosted on any web server.

## API Contracts

### Login API

**Endpoint**: POST `{ApiBaseUrl}{EndpointLogin}`

**Request**:
```json
{
  "email": "string",
  "password": "string"
}
```

**Response**:
```json
{
  "success": boolean,
  "token": "string",
  "userId": "string",
  "message": "string"
}
```

### Registration API

**Endpoint**: POST `{ApiBaseUrl}{EndpointRegister}`

**Request**:
```json
{
  "email": "string",
  "username": "string",
  "password": "string"
}
```

**Response**:
```json
{
  "success": boolean,
  "message": "string"
}
```

### Channels API

**Endpoint**: GET `{ApiBaseUrl}{EndpointChannels}`

**Response**:
```json
{
  "channels": [
    {
      "id": "string",
      "name": "string",
      "type": "direct|group",
      "members": []
    }
  ]
}
```

### Messages API

**Endpoint**: GET `{ApiBaseUrl}{EndpointMessages}?channelId={channelId}`

**Response**:
```json
{
  "messages": [
    {
      "id": "string",
      "content": "string",
      "authorId": "string",
      "authorName": "string",
      "timestamp": "date"
    }
  ]
}
```

## License

© 2025 Hetic-Stream