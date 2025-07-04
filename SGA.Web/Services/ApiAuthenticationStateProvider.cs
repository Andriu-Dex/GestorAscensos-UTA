using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using SGA.Web.Services;

namespace SGA.Web.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly SGA.Web.Services.ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public ApiAuthenticationStateProvider(SGA.Web.Services.ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt")));
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            if (string.IsNullOrEmpty(jwt)) return claims;

            var payloadSegment = jwt.Split('.')[1];
            if (string.IsNullOrEmpty(payloadSegment)) return claims;

            var jsonBytes = ParseBase64WithoutPadding(payloadSegment);
            if (jsonBytes == null) return claims;

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null) return claims;

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles); // Allow null for roles

            if (roles != null)
            {
                var rolesString = roles.ToString();
                if (!string.IsNullOrEmpty(rolesString)) 
                {
                    if (rolesString.Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);
                        if (parsedRoles != null)
                        {
                            foreach (var parsedRole in parsedRoles)
                            {
                                if (!string.IsNullOrEmpty(parsedRole)) 
                                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                            }
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesString));
                    }
                }
                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs
                .Where(kvp => kvp.Value != null && !string.IsNullOrEmpty(kvp.Value.ToString())) // Ensure value is not null
                .Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!))); // Use null-forgiving operator for kvp.Value

            return claims;
        }

        private byte[]? ParseBase64WithoutPadding(string base64) // Allow null return
        {
            if (string.IsNullOrEmpty(base64)) return null;
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            try
            {
                return Convert.FromBase64String(base64);
            }
            catch (FormatException)
            {
                // Log error or handle as appropriate
                return null;
            }
        }

        public async Task<bool> ValidateToken()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return false;
            }

            try
            {
                // Validar que el token sea un JWT válido
                var parts = savedToken.Split('.');
                if (parts.Length != 3)
                {
                    return false;
                }

                // Validar que podamos parsear el payload
                var payload = ParseBase64WithoutPadding(parts[1]);
                if (payload == null)
                {
                    return false;
                }

                // Verificar si el token ha expirado
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
                if (keyValuePairs == null)
                {
                    return false;
                }

                // Verificar si el token tiene un claim "exp" (tiempo de expiración)
                if (keyValuePairs.TryGetValue("exp", out object? exp))
                {
                    if (exp != null)
                    {
                        // Convertir el valor de exp a un DateTimeOffset
                        var expValue = Convert.ToInt64(exp);
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expValue).DateTime;

                        // Verificar si el token ha expirado
                        if (expirationTime <= DateTime.UtcNow)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Método para verificar si el token actual es válido
        public async Task<bool> IsTokenValidAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return false;
            }
            
            try
            {
                // Verificar si podemos parsear el token y si contiene los claims esperados
                var claims = ParseClaimsFromJwt(savedToken);
                
                if (claims == null || !claims.Any())
                {
                    return false;
                }
                
                // Verificar si hay algún claim de expiración
                var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
                
                if (expClaim != null)
                {
                    // Verificar si el token ha expirado
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                    
                    if (expirationTime <= DateTimeOffset.UtcNow)
                    {
                        return false;
                    }
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
