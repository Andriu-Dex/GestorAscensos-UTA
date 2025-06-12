using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SGA.BlazorApp.Client.Auth
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly JwtTokenHandler _tokenHandler;

        public AuthorizationMessageHandler(JwtTokenHandler tokenHandler)
        {
            _tokenHandler = tokenHandler;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _tokenHandler.GetTokenAsync();
            
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
