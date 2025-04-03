# Hetic-Stream Deployment Guide

This guide provides instructions for building and deploying the Hetic-Stream application on different platforms.

## Prerequisites

- .NET 7.0 SDK or later
- Git (optional, for cloning the repository)

## Building from Source

### Cloning the Repository

```bash
git clone https://github.com/yourusername/hetic-stream.git
cd hetic-stream
```

### Restoring Dependencies

```bash
dotnet restore
```

### Building the Solution

```bash
dotnet build
```

## Running the Application

### Running in Development Mode

```bash
dotnet run --project HeticStream.UI
```

### Setting Environment Variables

You can set environment variables to configure the application:

#### Windows (PowerShell)

```powershell
$env:ApiBaseUrl = "https://api.example.com"
$env:ApiEnabled = "true"
$env:EndpointLogin = "/auth/login"
$env:EndpointRegister = "/auth/register"
$env:EndpointChannels = "/channels"
$env:EndpointMessages = "/messages"
$env:ImageAssetsPath = "Assets/Images"
$env:LightThemeEnabled = "false"
```

#### macOS/Linux (Bash)

```bash
export ApiBaseUrl="https://api.example.com"
export ApiEnabled="true"
export EndpointLogin="/auth/login"
export EndpointRegister="/auth/register"
export EndpointChannels="/channels"
export EndpointMessages="/messages"
export ImageAssetsPath="Assets/Images"
export LightThemeEnabled="false"
```

## Deploying to Different Platforms

### Windows

#### Building a Windows Executable

```bash
dotnet publish HeticStream.UI -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

The executable will be in the `HeticStream.UI/bin/Release/net7.0/win-x64/publish` directory.

#### Creating a Windows Installer (with NSIS)

1. Install NSIS (Nullsoft Scriptable Install System)
2. Create an NSIS script (`installer.nsi`):

```nsi
!include "MUI2.nsh"

Name "Hetic-Stream"
OutFile "HeticStreamSetup.exe"
InstallDir "$PROGRAMFILES\Hetic-Stream"
RequestExecutionLevel admin

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Section "Install"
  SetOutPath "$INSTDIR"
  File /r "HeticStream.UI\bin\Release\net7.0\win-x64\publish\*.*"
  
  CreateDirectory "$SMPROGRAMS\Hetic-Stream"
  CreateShortcut "$SMPROGRAMS\Hetic-Stream\Hetic-Stream.lnk" "$INSTDIR\HeticStream.UI.exe"
  CreateShortcut "$DESKTOP\Hetic-Stream.lnk" "$INSTDIR\HeticStream.UI.exe"
  
  WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd

Section "Uninstall"
  Delete "$INSTDIR\Uninstall.exe"
  Delete "$SMPROGRAMS\Hetic-Stream\Hetic-Stream.lnk"
  Delete "$DESKTOP\Hetic-Stream.lnk"
  RMDir "$SMPROGRAMS\Hetic-Stream"
  RMDir /r "$INSTDIR"
SectionEnd
```

3. Compile the NSIS script:

```bash
makensis installer.nsi
```

### macOS

#### Building a macOS Application

```bash
dotnet publish HeticStream.UI -c Release -r osx-x64 --self-contained true
```

The application will be in the `HeticStream.UI/bin/Release/net7.0/osx-x64/publish` directory.

#### Creating a macOS DMG

1. Create a directory for the DMG contents:

```bash
mkdir -p dmg/Hetic-Stream.app/Contents/MacOS
mkdir -p dmg/Hetic-Stream.app/Contents/Resources
```

2. Copy the published files:

```bash
cp -r HeticStream.UI/bin/Release/net7.0/osx-x64/publish/* dmg/Hetic-Stream.app/Contents/MacOS/
```

3. Create the Info.plist file:

```bash
cat > dmg/Hetic-Stream.app/Contents/Info.plist << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>HeticStream.UI</string>
    <key>CFBundleIconFile</key>
    <string>AppIcon</string>
    <key>CFBundleIdentifier</key>
    <string>com.heticstream.app</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundleName</key>
    <string>Hetic-Stream</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>CFBundleVersion</key>
    <string>1</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.12</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF
```

4. Create the DMG:

```bash
hdiutil create -volname "Hetic-Stream" -srcfolder dmg -ov -format UDZO Hetic-Stream.dmg
```

### Linux

#### Building a Linux AppImage

1. Install the AppImage tools:

```bash
sudo apt-get install appstream-util
```

2. Publish the application:

```bash
dotnet publish HeticStream.UI -c Release -r linux-x64 --self-contained true
```

3. Create the AppDir structure:

```bash
mkdir -p AppDir/usr/bin
mkdir -p AppDir/usr/share/applications
mkdir -p AppDir/usr/share/icons/hicolor/256x256/apps
```

4. Copy the published files:

```bash
cp -r HeticStream.UI/bin/Release/net7.0/linux-x64/publish/* AppDir/usr/bin/
```

5. Create the desktop entry:

```bash
cat > AppDir/usr/share/applications/hetic-stream.desktop << EOF
[Desktop Entry]
Name=Hetic-Stream
Comment=Cross-platform chat application
Exec=HeticStream.UI
Icon=hetic-stream
Type=Application
Categories=Network;InstantMessaging;
EOF
```

6. Copy the icon:

```bash
cp Assets/icon.png AppDir/usr/share/icons/hicolor/256x256/apps/hetic-stream.png
```

7. Create the AppImage:

```bash
wget -c "https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage"
chmod +x appimagetool-x86_64.AppImage
./appimagetool-x86_64.AppImage AppDir Hetic-Stream.AppImage
```

### Web (WebAssembly)

#### Building for Web

1. Add the WebAssembly package reference to the UI project (HeticStream.UI.csproj):

```xml
<PackageReference Include="Avalonia.Browser" Version="11.0.5" />
```

2. Publish the WebAssembly application:

```bash
dotnet publish HeticStream.UI -c Release -f net7.0 --runtime browser-wasm -o ./publish-wasm
```

3. The published files will be in the `./publish-wasm` directory.

#### Deploying to a Web Server

1. Copy the published files to your web server:

```bash
scp -r ./publish-wasm/* user@server:/var/www/html/hetic-stream/
```

2. Configure your web server:

Apache:

```apache
<VirtualHost *:80>
    ServerName hetic-stream.example.com
    DocumentRoot /var/www/html/hetic-stream
    
    <Directory /var/www/html/hetic-stream>
        Options -Indexes +FollowSymLinks
        AllowOverride All
        Require all granted
        
        # Set correct MIME types
        AddType application/wasm .wasm
        AddType application/octet-stream .clr
        AddType application/octet-stream .dat
        AddType application/javascript .js
        AddType text/html .html
    </Directory>
    
    ErrorLog ${APACHE_LOG_DIR}/hetic-stream-error.log
    CustomLog ${APACHE_LOG_DIR}/hetic-stream-access.log combined
</VirtualHost>
```

Nginx:

```nginx
server {
    listen 80;
    server_name hetic-stream.example.com;
    root /var/www/html/hetic-stream;
    index index.html;
    
    location / {
        try_files $uri $uri/ =404;
    }
    
    # Set correct MIME types
    types {
        application/wasm wasm;
        application/octet-stream clr;
        application/octet-stream dat;
        application/javascript js;
        text/html html;
    }
}
```

## Docker Deployment

### Building a Docker Image

1. Create a Dockerfile:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["HeticStream.Core/HeticStream.Core.csproj", "HeticStream.Core/"]
COPY ["HeticStream.Services/HeticStream.Services.csproj", "HeticStream.Services/"]
COPY ["HeticStream.UI/HeticStream.UI.csproj", "HeticStream.UI/"]
RUN dotnet restore "HeticStream.UI/HeticStream.UI.csproj"

# Copy the rest of the source code
COPY . .

# Build and publish the application
RUN dotnet publish "HeticStream.UI/HeticStream.UI.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app/publish .

# Configure environment variables
ENV ApiBaseUrl="https://api.example.com"
ENV ApiEnabled="false"
ENV EndpointLogin="/auth/login"
ENV EndpointRegister="/auth/register"
ENV EndpointChannels="/channels"
ENV EndpointMessages="/messages"
ENV ImageAssetsPath="Assets/Images"
ENV LightThemeEnabled="false"

# Expose the port (if needed)
EXPOSE 8080

ENTRYPOINT ["dotnet", "HeticStream.UI.dll"]
```

2. Build the Docker image:

```bash
docker build -t hetic-stream:latest .
```

3. Run the Docker container:

```bash
docker run -p 8080:8080 hetic-stream:latest
```

## Continuous Integration/Continuous Deployment (CI/CD)

### GitHub Actions

Create a GitHub Actions workflow file (`.github/workflows/ci-cd.yml`):

```yaml
name: CI/CD

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 7.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Publish Windows
      run: dotnet publish HeticStream.UI -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish-win
    
    - name: Publish macOS
      run: dotnet publish HeticStream.UI -c Release -r osx-x64 --self-contained true -o ./publish-mac
    
    - name: Publish Linux
      run: dotnet publish HeticStream.UI -c Release -r linux-x64 --self-contained true -o ./publish-linux
    
    - name: Publish Web
      run: dotnet publish HeticStream.UI -c Release -f net7.0 --runtime browser-wasm -o ./publish-web
    
    - name: Upload Windows Artifact
      uses: actions/upload-artifact@v2
      with:
        name: windows-build
        path: ./publish-win
    
    - name: Upload macOS Artifact
      uses: actions/upload-artifact@v2
      with:
        name: macos-build
        path: ./publish-mac
    
    - name: Upload Linux Artifact
      uses: actions/upload-artifact@v2
      with:
        name: linux-build
        path: ./publish-linux
    
    - name: Upload Web Artifact
      uses: actions/upload-artifact@v2
      with:
        name: web-build
        path: ./publish-web
```

## Environment Configuration

### Production Environment

For production environments, set the following environment variables:

```
ApiBaseUrl=https://api.hetic-stream.com
ApiEnabled=true
EndpointLogin=/auth/login
EndpointRegister=/auth/register
EndpointChannels=/channels
EndpointMessages=/messages
ImageAssetsPath=Assets/Images
LightThemeEnabled=false
```

### Development Environment

For development environments, set the following environment variables:

```
ApiBaseUrl=https://dev-api.hetic-stream.com
ApiEnabled=false
EndpointLogin=/auth/login
EndpointRegister=/auth/register
EndpointChannels=/channels
EndpointMessages=/messages
ImageAssetsPath=Assets/Images
LightThemeEnabled=false
```

## Troubleshooting

### Common Issues

#### Application fails to start

1. Check that .NET 7.0 SDK is installed:

```bash
dotnet --version
```

2. Check that all dependencies are restored:

```bash
dotnet restore
```

3. Check the environment variables:

```bash
env | grep Api
```

#### API connection issues

1. Check the `ApiBaseUrl` environment variable:

```bash
echo $ApiBaseUrl
```

2. Check the `ApiEnabled` environment variable:

```bash
echo $ApiEnabled
```

3. Use simulation mode for testing:

```bash
export ApiEnabled=false
```

#### UI rendering issues

1. Check that Avalonia dependencies are installed:

```bash
dotnet add HeticStream.UI package Avalonia --version 11.0.5
dotnet add HeticStream.UI package Avalonia.Desktop --version 11.0.5
dotnet add HeticStream.UI package Avalonia.Themes.Fluent --version 11.0.5
```

2. Check that assets paths are correct:

```bash
export ImageAssetsPath=Assets/Images
```

## Support

For support, please contact the Hetic-Stream team or open an issue on GitHub.