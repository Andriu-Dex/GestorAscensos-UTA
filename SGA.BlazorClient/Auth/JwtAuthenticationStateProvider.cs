using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using SGA.Application.DTOs;

namespace SGA.BlazorApp.Client.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly JwtTokenHandler _tokenHandler;

        public JwtAuthenticationStateProvider(JwtTokenHandler tokenHandler)
        {
            _tokenHandler = tokenHandler;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _tokenHandler.GetTokenAsync();
            
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }            var claims = _tokenHandler.ParseClaimsFromJwt(savedToken);
            var expiry = claims.FirstOrDefault(c => c.Type.Equals("exp"))?.Value;
            
            if (string.IsNullOrEmpty(expiry) || 
                !long.TryParse(expiry, out var expiryUnixTimeSeconds) || 
                DateTimeOffset.FromUnixTimeSeconds(expiryUnixTimeSeconds) <= DateTimeOffset.UtcNow)
            {
                await _tokenHandler.RemoveTokenAsync();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var claims = _tokenHandler.ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
