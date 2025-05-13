using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Project.Interfaces;
using Project.Models;
using Project.Repositories;

namespace Project.Services
{
    public class AuthService(UserRepository repository, IConfiguration configuration) : IAuthService
    {
        private readonly UserRepository _repository = repository;
        private readonly IConfiguration _configuration = configuration;
        private readonly string TokenEndpoint = "https://oauth2.googleapis.com/token";
        private readonly string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
        private readonly string AccountEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private readonly string RedirectUriBase = "http://localhost:5085/auth/google/callback";

        public async Task<bool> SignUp(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _repository.Add(user);
            return await _repository.SaveAsync();
        }

        public async Task<User> SignIn(string email, string password)
        {
            var user = await _repository.SelectAll().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }
            return user;
        }

        public string GetGoogleUrl()
        {
            var clientId = _configuration["GoogleAPIKey:ClientId"];
            var scopes = "openid email profile";
            return $"{AccountEndpoint}?client_id={clientId}&redirect_uri={RedirectUriBase}&response_type=code&scope={scopes}";
        }

        public async Task<string> GetUserInfoAsync(string accessToken)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken
            );
            HttpResponseMessage response = await client.GetAsync(UserInfoEndpoint);
            var userInfo = await response.Content.ReadAsStringAsync();
            return userInfo;
        }

        public async Task<string> ExchangeCodeForTokenAsync(string code, string type)
        {
            using var client = new HttpClient();
            var clientId = _configuration["GoogleAPIKey:ClientId"];
            var clientSecret = _configuration["GoogleAPIKey:ClientSecret"];

            var response = await client.PostAsync(
                TokenEndpoint,
                new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { "code", code },
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "redirect_uri", RedirectUriBase },
                        { "grant_type", "authorization_code" },
                    }
                )
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(responseContent);

            return tokenData.GetProperty("access_token").GetString();
        }

        public async Task<User> GoogleAuthenticatedUser(string email)
        {
            User _user = await _repository.SelectAll().FirstOrDefaultAsync(u => u.Email == email);
            return _user;
        }

        public async Task<bool> CreateGoogleUser(User _item)
        {
            _repository.Add(_item);
            return _repository.Save();
        }

        public async Task<bool> CheckExistEmail(string email)
        {
            return await _repository.SelectAll().AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ChangePassword(string email, string password)
        {
            var user = await _repository.SelectAll().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            _repository.Update(user);
            return await _repository.SaveAsync();
        }
    }
}
