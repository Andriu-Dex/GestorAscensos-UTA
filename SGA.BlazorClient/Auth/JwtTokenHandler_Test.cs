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
    public class JwtTokenHandler_Test
    {
        private readonly IJSRuntime _jsRuntime;

        public JwtTokenHandler_Test(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetTokenAsync()
        {
            return await Task.FromResult("test");
        }
    }
}
