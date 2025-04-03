// HETIC - Authentication service interface for Hetic-Stream
using System.Threading.Tasks;

namespace HeticStream.Core.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticate a user with email and password
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password</param>
        /// <returns>Authentication result with token if successful</returns>
        Task<AuthResult> LoginAsync(string email, string password);
        
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="username">User username</param>
        /// <param name="password">User password</param>
        /// <returns>Registration result</returns>
        Task<RegisterResult> RegisterAsync(string email, string username, string password);
        
        /// <summary>
        /// Check if the user is authenticated
        /// </summary>
        bool IsAuthenticated { get; }
        
        /// <summary>
        /// Get the current authentication token
        /// </summary>
        string? CurrentToken { get; }
        
        /// <summary>
        /// Get the current user ID
        /// </summary>
        string? CurrentUserId { get; }
        
        /// <summary>
        /// Logout the current user
        /// </summary>
        void Logout();
    }
    
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    
    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}