# Extending Hetic-Stream

This guide provides information on how to extend and customize the Hetic-Stream application.

## Adding New Features

### Adding a New Page

1. Create a new view model in the `HeticStream.UI/ViewModels` directory:

```csharp
public class NewPageViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    
    public NewPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }
    
    // Add properties and commands
}
```

2. Create a new view in the `HeticStream.UI/Views` directory:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:viewModels="clr-namespace:HeticStream.UI.ViewModels"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="600"
        x:Class="HeticStream.UI.Views.NewPageView"
        x:DataType="viewModels:NewPageViewModel"
        Title="Hetic-Stream - New Page"
        Width="800" Height="600"
        WindowStartupLocation="CenterScreen"
        Background="{DynamicResource BackgroundBrush}">
    
    <!-- Content goes here -->
    
</Window>
```

3. Register the view model in the `App.axaml.cs` file:

```csharp
private void ConfigureServices()
{
    var services = new ServiceCollection();
    
    // Other registrations...
    
    // Register the new view model
    services.AddTransient<NewPageViewModel>();
    
    // Build the service provider
    _serviceProvider = services.BuildServiceProvider();
}
```

4. Update the `NavigationService.cs` file to include the new view:

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
    else if (viewModel is NewPageViewModel)
    {
        view = new NewPageView { DataContext = viewModel };
    }
    
    // Rest of the method...
}
```

### Adding a New API Endpoint

1. Update the `AppSettings.cs` file to include the new endpoint:

```csharp
public class AppSettings
{
    // Existing endpoints...
    
    public string EndpointNewFeature { get; set; } = string.Empty;
    
    private void LoadFromEnvironment()
    {
        // Existing loading logic...
        
        EndpointNewFeature = Environment.GetEnvironmentVariable("EndpointNewFeature") ?? "/new-feature";
    }
}
```

2. Create a new service interface in the `HeticStream.Core/Services` directory:

```csharp
public interface INewFeatureService
{
    Task<NewFeatureResult> GetNewFeatureDataAsync();
}

public class NewFeatureResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object Data { get; set; } = null!;
}
```

3. Implement the service in the `HeticStream.Services` directory:

```csharp
public class NewFeatureService : INewFeatureService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly AppSettings _settings;
    
    public NewFeatureService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
        _settings = AppSettings.Instance;
    }
    
    public async Task<NewFeatureResult> GetNewFeatureDataAsync()
    {
        try
        {
            // If API is disabled, simulate response
            if (!_settings.ApiEnabled)
            {
                Console.WriteLine("[SIMULATED] GetNewFeatureData request");
                
                // Simulate data
                var simulatedResult = new NewFeatureResult
                {
                    Success = true,
                    Message = "Data retrieved successfully (Simulated)",
                    Data = new { /* Simulated data */ }
                };
                
                Console.WriteLine($"[SIMULATED] GetNewFeatureData response: {JsonSerializer.Serialize(simulatedResult)}");
                
                return simulatedResult;
            }
            
            // Set the authentication token in the request header
            if (_authService.IsAuthenticated)
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _authService.CurrentToken);
            }
            
            // Make the API request
            var response = await _httpClient.GetAsync($"{_settings.ApiBaseUrl}{_settings.EndpointNewFeature}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<NewFeatureResult>();
                return result ?? new NewFeatureResult { Success = false, Message = "Failed to deserialize response" };
            }
            
            return new NewFeatureResult
            {
                Success = false,
                Message = $"Request failed with status code: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new NewFeatureResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}
```

4. Register the service in the `App.axaml.cs` file:

```csharp
private void ConfigureServices()
{
    var services = new ServiceCollection();
    
    // Other registrations...
    
    // Register the new service
    services.AddSingleton<INewFeatureService, NewFeatureService>();
    
    // Build the service provider
    _serviceProvider = services.BuildServiceProvider();
}
```

### Customizing the Theme

1. Update the `Colors.axaml` file to modify or add new colors:

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <!-- Existing colors... -->
    
    <!-- Custom colors -->
    <Color x:Key="CustomColor">#FF5733</Color>
    <SolidColorBrush x:Key="CustomBrush" Color="{DynamicResource CustomColor}" />
</ResourceDictionary>
```

2. Update the `Controls.axaml` file to add new control styles:

```xml
<Styles xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <!-- Existing styles... -->
    
    <!-- Custom control style -->
    <Style Selector="Button.Custom">
        <Setter Property="Background" Value="{DynamicResource CustomBrush}" />
        <Setter Property="Foreground" Value="White" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Padding" Value="16,8" />
        <Setter Property="CornerRadius" Value="4" />
    </Style>
    
    <Style Selector="Button.Custom:pointerover">
        <Setter Property="Background" Value="{DynamicResource CustomBrush}" />
        <Setter Property="Opacity" Value="0.9" />
    </Style>
</Styles>
```

## Adding a New Model

1. Create a new model class in the `HeticStream.Core/Models` directory:

```csharp
public class NewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

2. Create a corresponding view model in the `HeticStream.UI/ViewModels` directory:

```csharp
public class NewModelViewModel : ViewModelBase
{
    private NewModel _model;
    
    public NewModelViewModel(NewModel model)
    {
        _model = model;
    }
    
    public string Id => _model.Id;
    
    public string Name => _model.Name;
    
    public string Description => _model.Description;
    
    public DateTime CreatedAt => _model.CreatedAt;
    
    public string FormattedDate => _model.CreatedAt.ToString("MMM dd, yyyy");
}
```

## Adding Unit Tests

1. Create a new test project:

```bash
dotnet new xunit -n HeticStream.Tests
```

2. Add references to the projects you want to test:

```bash
dotnet add HeticStream.Tests reference HeticStream.Core
dotnet add HeticStream.Tests reference HeticStream.Services
```

3. Create test classes for your services and view models:

```csharp
public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsSuccessResult()
    {
        // Arrange
        var httpClient = new HttpClient();
        var authService = new AuthService(httpClient);
        
        // Act
        var result = await authService.LoginAsync("valid@example.com", "validpassword");
        
        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.Token);
        Assert.NotEmpty(result.UserId);
    }
    
    [Fact]
    public async Task LoginAsync_WhenCredentialsAreInvalid_ReturnsFailureResult()
    {
        // Arrange
        var httpClient = new HttpClient();
        var authService = new AuthService(httpClient);
        
        // Act
        var result = await authService.LoginAsync("invalid@example.com", "invalidpassword");
        
        // Assert
        Assert.False(result.Success);
        Assert.Empty(result.Token);
        Assert.Empty(result.UserId);
        Assert.NotEmpty(result.Message);
    }
}
```

## Adding Localization

1. Create a resource file for each language:

```csharp
// Resources/Strings.en.resx
// Resources/Strings.fr.resx
// Resources/Strings.es.resx
```

2. Create a localization service:

```csharp
public interface ILocalizationService
{
    string GetString(string key);
    void SetLanguage(string language);
    string CurrentLanguage { get; }
}

public class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _resources;
    private string _currentLanguage = "en";
    
    public LocalizationService()
    {
        _resources = new Dictionary<string, Dictionary<string, string>>();
        
        // Load resources for each language
        LoadResources();
    }
    
    public string GetString(string key)
    {
        if (_resources.TryGetValue(_currentLanguage, out var strings) && strings.TryGetValue(key, out var value))
        {
            return value;
        }
        
        // Fallback to English
        if (_currentLanguage != "en" && _resources.TryGetValue("en", out var englishStrings) && englishStrings.TryGetValue(key, out var englishValue))
        {
            return englishValue;
        }
        
        // Return the key as a last resort
        return key;
    }
    
    public void SetLanguage(string language)
    {
        if (_resources.ContainsKey(language))
        {
            _currentLanguage = language;
        }
    }
    
    public string CurrentLanguage => _currentLanguage;
    
    private void LoadResources()
    {
        // Load resources for each language
        // This could be from embedded resources, files, or a database
    }
}
```

3. Register the localization service in the `App.axaml.cs` file:

```csharp
private void ConfigureServices()
{
    var services = new ServiceCollection();
    
    // Other registrations...
    
    // Register the localization service
    services.AddSingleton<ILocalizationService, LocalizationService>();
    
    // Build the service provider
    _serviceProvider = services.BuildServiceProvider();
}
```

4. Use the localization service in your view models:

```csharp
public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly ILocalizationService _localizationService;
    
    public LoginViewModel(
        IAuthService authService, 
        INavigationService navigationService,
        ILocalizationService localizationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _localizationService = localizationService;
    }
    
    public string LoginButtonText => _localizationService.GetString("LoginButton");
    public string EmailPlaceholder => _localizationService.GetString("EmailPlaceholder");
    public string PasswordPlaceholder => _localizationService.GetString("PasswordPlaceholder");
}
```

## Conclusion

This guide provides a starting point for extending the Hetic-Stream application. You can add new features, customize the theme, add new models, and more. The application's modular architecture makes it easy to extend and maintain.