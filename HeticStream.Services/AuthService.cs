// HETIC - Authentication service implementation for Hetic-Stream
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using HeticStream.Core.Config;
using HeticStream.Core.Services;

namespace HeticStream.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;
        private string? _currentToken;
        private string? _currentUserId;
        
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _settings = AppSettings.Instance;
        }
        
        public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);
        
        public string? CurrentToken => _currentToken;
        
        public string? CurrentUserId => _currentUserId;
        
        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                // Create login request model
                var loginModel = new
                {
                    email,
                    password
                };
                
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
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_settings.ApiBaseUrl}{_settings.EndpointLogin}", 
                    loginModel);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResult>();
                    
                    if (result != null && result.Success)
                    {
                        // Store authentication information
                        _currentToken = result.Token;
                        _currentUserId = result.UserId;
                    }
                    
                    return result ?? new AuthResult 
                    { 
                        Success = false, 
                        Message = "Failed to deserialize response" 
                    };
                }
                
                return new AuthResult
                {
                    Success = false,
                    Message = $"Login failed with status code: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = $"Login error: {ex.Message}"
                };
            }
        }
        
        public async Task<RegisterResult> RegisterAsync(string email, string username, string password)
        {
            try
            {
                // Create register request model
                var registerModel = new
                {
                    email,
                    username,
                    password
                };
                
                // If API is disabled, simulate a successful registration
                if (!_settings.ApiEnabled)
                {
                    Console.WriteLine($"[SIMULATED] Register request: {JsonSerializer.Serialize(registerModel)}");
                    
                    // Simulate successful registration
                    var simulatedResult = new RegisterResult
                    {
                        Success = true,
                        Message = "Registration successful (Simulated)"
                    };
                    
                    Console.WriteLine($"[SIMULATED] Register response: {JsonSerializer.Serialize(simulatedResult)}");
                    
                    return simulatedResult;
                }
                
                // API is enabled, make actual request
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_settings.ApiBaseUrl}{_settings.EndpointRegister}", 
                    registerModel);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RegisterResult>();
                    
                    return result ?? new RegisterResult 
                    { 
                        Success = false, 
                        Message = "Failed to deserialize response" 
                    };
                }
                
                return new RegisterResult
                {
                    Success = false,
                    Message = $"Registration failed with status code: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = $"Registration error: {ex.Message}"
                };
            }
        }
        
        public void Logout()
        {
            _currentToken = null;
            _currentUserId = null;
        }
    }
}