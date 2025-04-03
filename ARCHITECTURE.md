# Hetic-Stream Architecture Documentation

## Overview

Hetic-Stream is a cross-platform chat application built with Avalonia UI, C#, and .NET using the MVVM (Model-View-ViewModel) architecture. The application is designed to run on Windows, macOS, Linux, and Web platforms.

## Architecture

The application follows a layered architecture with three main projects:

1. **HeticStream.Core**: Contains models, interfaces, and configuration classes
2. **HeticStream.Services**: Contains service implementations for business logic
3. **HeticStream.UI**: Contains the Avalonia UI application with views and view models

### MVVM Pattern

The MVVM pattern is used to separate the UI from the business logic:

- **Models**: Data structures that represent the domain objects (User, Channel, Message)
- **Views**: User interface components (LoginView, MainView, etc.)
- **ViewModels**: Classes that handle the presentation logic and state of the views

### Project Structure

```
HeticStream/
├── HeticStream.Core/
│   ├── Config/
│   │   └── AppSettings.cs
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Channel.cs
│   │   └── Message.cs
│   └── Services/
│       ├── IAuthService.cs
│       ├── IChannelService.cs
│       └── IUserService.cs
├── HeticStream.Services/
│   ├── AuthService.cs
│   ├── ChannelService.cs
│   └── UserService.cs
└── HeticStream.UI/
    ├── Assets/
    ├── Services/
    │   ├── INavigationService.cs
    │   └── NavigationService.cs
    ├── Styles/
    │   ├── Colors.axaml
    │   ├── Controls.axaml
    │   └── Resources.axaml
    ├── ViewModels/
    │   ├── ViewModelBase.cs
    │   ├── LoginViewModel.cs
    │   ├── RegisterViewModel.cs
    │   ├── MainViewModel.cs
    │   ├── ChannelViewModel.cs
    │   ├── MessageViewModel.cs
    │   ├── UserListViewModel.cs
    │   └── NotificationViewModel.cs
    ├── Views/
    │   ├── LoginView.axaml
    │   ├── RegisterView.axaml
    │   ├── MainView.axaml
    │   └── NotificationView.axaml
    ├── App.axaml
    ├── App.axaml.cs
    └── Program.cs
```

## Dependency Injection

The application uses dependency injection to manage dependencies between components. Services are registered in the `App.axaml.cs` file and injected into view models.

```csharp
private void ConfigureServices()
{
    var services = new ServiceCollection();
    
    // Register services
    services.AddSingleton<IAuthService, AuthService>();
    services.AddSingleton<IChannelService, ChannelService>();
    services.AddSingleton<IUserService, UserService>();
    
    // Register view models
    services.AddTransient<LoginViewModel>();
    services.AddTransient<RegisterViewModel>();
    services.AddTransient<MainViewModel>();
    
    // Build the service provider
    _serviceProvider = services.BuildServiceProvider();
}
```

## Configuration

The application uses environment variables for configuration. The `AppSettings` class loads these variables and provides them to the application.

```csharp
private void LoadFromEnvironment()
{
    // Load API configuration
    ApiBaseUrl = Environment.GetEnvironmentVariable("ApiBaseUrl") ?? "https://api.heticstream.com";
    ApiEnabled = bool.TryParse(Environment.GetEnvironmentVariable("ApiEnabled"), out bool apiEnabled) && apiEnabled;
    
    // Load endpoints
    EndpointLogin = Environment.GetEnvironmentVariable("EndpointLogin") ?? "/auth/login";
    EndpointRegister = Environment.GetEnvironmentVariable("EndpointRegister") ?? "/auth/register";
    EndpointChannels = Environment.GetEnvironmentVariable("EndpointChannels") ?? "/channels";
    EndpointMessages = Environment.GetEnvironmentVariable("EndpointMessages") ?? "/messages";
    
    // Load assets configuration
    ImageAssetsPath = Environment.GetEnvironmentVariable("ImageAssetsPath") ?? "Assets/Images";
    
    // Load theme configuration
    DarkThemeEnabled = !bool.TryParse(Environment.GetEnvironmentVariable("LightThemeEnabled"), out bool lightThemeEnabled) || !lightThemeEnabled;
}
```

## API Integration

The application supports two modes for API integration:

1. **Real API Mode**: When `ApiEnabled` is `true`, the application makes actual HTTP requests to the API endpoints.
2. **Simulation Mode**: When `ApiEnabled` is `false`, the application simulates API calls and logs requests and responses to the console.

The service implementations handle both modes:

```csharp
// If API is disabled, simulate a successful login
if (!_settings.ApiEnabled)
{
    Console.WriteLine($"[SIMULATED] Login request: {JsonSerializer.Serialize(loginModel)}");
    
    // Simulate successful login
    var simulatedResult = new AuthResult
    {
        Success = true,
        Token = "simulated_token_" + Guid.NewGuid().ToString("N"),
        UserId = "user_" + Guid.NewGuid().ToString("N"),
        Message = "Login successful (Simulated)"
    };
    
    Console.WriteLine($"[SIMULATED] Login response: {JsonSerializer.Serialize(simulatedResult)}");
    
    // Store authentication information
    _currentToken = simulatedResult.Token;
    _currentUserId = simulatedResult.UserId;
    
    return simulatedResult;
}

// API is enabled, make actual request
// ...
```

## Navigation

The application uses a custom navigation service to navigate between views. The `NavigationService` class handles navigation and maintains a navigation stack.

```csharp
public void NavigateTo<TViewModel>(object? parameter = null)
{
    // Get service provider
    var serviceProvider = App.GetServiceProvider();
    
    // Get the view model instance
    var viewModel = serviceProvider.GetRequiredService<TViewModel>();
    
    // Create the appropriate view based on the view model type
    Window? view = null;
    
    if (viewModel is LoginViewModel)
    {
        view = new LoginView { DataContext = viewModel };
    }
    else if (viewModel is RegisterViewModel)
    {
        view = new RegisterView { DataContext = viewModel };
    }
    else if (viewModel is MainViewModel)
    {
        view = new MainView { DataContext = viewModel };
    }
    
    // ...
}
```

## UI Design

The UI is designed to be modern and responsive. It uses Avalonia UI controls and styles to achieve a consistent look and feel across platforms.

The application supports both light and dark themes. Theme resources are defined in `Colors.axaml` and `Resources.axaml`.

## Notifications

The application uses a custom notification system to show success, error, and warning messages. The `NotificationView` class displays a toast-like notification at the bottom-right corner of the screen.

```csharp
public void ShowNotification(string title, string message, NotificationType type)
{
    // Close any existing notification
    _currentNotification?.Close();
    
    // Create a new notification view model
    var viewModel = new NotificationViewModel
    {
        Title = title,
        Message = message,
        Type = type
    };
    
    // Create the notification view
    _currentNotification = new NotificationView
    {
        DataContext = viewModel
    };
    
    // Show the notification
    _currentNotification.Show();
    
    // Auto-close after 5 seconds for success messages
    if (type == NotificationType.Success)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await Task.Delay(5000);
            _currentNotification?.Close();
            _currentNotification = null;
        });
    }
}
```

## Cross-Platform Support

The application is designed to run on multiple platforms:

- **Windows, macOS, Linux**: Using Avalonia's native platform support
- **Web**: Using Avalonia's WebAssembly support

The application uses platform-specific application lifetimes to handle different platforms:

```csharp
public override void OnFrameworkInitializationCompleted()
{
    // Configure services
    ConfigureServices();
    
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        var loginViewModel = _serviceProvider!.GetRequiredService<LoginViewModel>();
        desktop.MainWindow = new LoginView
        {
            DataContext = loginViewModel
        };
    }
    else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
    {
        var loginViewModel = _serviceProvider!.GetRequiredService<LoginViewModel>();
        singleViewPlatform.MainView = new LoginView
        {
            DataContext = loginViewModel
        };
    }

    base.OnFrameworkInitializationCompleted();
}
```

## Security

The application uses token-based authentication for API requests. The token is stored in memory and used for subsequent requests:

```csharp
// Set the authentication token in the request header
if (_authService.IsAuthenticated)
{
    _httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", _authService.CurrentToken);
}
```

No sensitive information like passwords or tokens is persisted to disk.

## Conclusion

Hetic-Stream follows a well-structured, modular, and maintainable architecture. It separates concerns using the MVVM pattern and supports cross-platform development. The application is designed to be flexible, allowing it to work with both real and simulated APIs.