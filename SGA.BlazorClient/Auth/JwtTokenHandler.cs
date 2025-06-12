using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.JSInterop;

namespace SGA.BlazorApp.Client.Auth
{
    public class JwtTokenHandler
    {
        private const string TOKEN_KEY = "jwt_token";
        private const string EXPIRY_KEY = "jwt_expiry";
        private const string USER_INFO_KEY = "user_info";
        
        private readonly IJSRuntime _jsRuntime;

        public JwtTokenHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetTokenAsync()
        {
            return await GetFromLocalStorageAsync(TOKEN_KEY);
        }

        public async Task SaveTokenAsync(string token)
        {
            await SaveToLocalStorageAsync(TOKEN_KEY, token);
        }

        public async Task SaveUserInfoAsync(string userInfo)
        {
            await SaveToLocalStorageAsync(USER_INFO_KEY, userInfo);
        }

        public async Task<string> GetUserInfoAsync()
        {
            return await GetFromLocalStorageAsync(USER_INFO_KEY);
        }

        public async Task RemoveTokenAsync()
        {
            await RemoveFromLocalStorageAsync(TOKEN_KEY);
            await RemoveFromLocalStorageAsync(EXPIRY_KEY);
            await RemoveFromLocalStorageAsync(USER_INFO_KEY);
        }

        public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
                return new List<Claim>();

            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(jwt))
            {
                var token = handler.ReadJwtToken(jwt);
                return token.Claims;
            }
            
            // Fallback to manual parsing if token is not properly formatted
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private async Task<string> GetFromLocalStorageAsync(string key)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            }
            catch
            {
                return null;
            }
        }

        private async Task SaveToLocalStorageAsync(string key, string value)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
            }
            catch
            {
                // Log error
            }
        }

        private async Task RemoveFromLocalStorageAsync(string key)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            }
            catch
            {
                // Log error
            }
        }
    }
}
