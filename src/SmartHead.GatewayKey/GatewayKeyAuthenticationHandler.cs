using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartHead.Essentials.Application.Formatter;

namespace SmartHead.GatewayKey
{
    public class GatewayKeyAuthenticationHandler<T> : AuthenticationHandler<GatewayKeyAuthenticationOptions>
        where T : GatewayKeyBase
    {
        private readonly DbContext _context;
        private GatewayKeyBase _key;
        
        public GatewayKeyAuthenticationHandler(
            IOptionsMonitor<GatewayKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            DbContext dbContext) : base(options, logger, encoder, clock)
        {
            _context = dbContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(Options.GatewayKeyHeaderName, out var gatewayKeyHeaderValues))
                return AuthenticateResult.NoResult();

            var header = gatewayKeyHeaderValues.FirstOrDefault();

            if (gatewayKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(header))
                return AuthenticateResult.NoResult();

            var keys = _context.Set<T>().ToArray();
            
            _key = await _context
                .Set<T>()
                .FirstOrDefaultAsync(x => x.Key == header);

            if (_key == null || !_key.IsActive)
            {
                return AuthenticateResult.Fail("Invalid Gateway Key.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, _key.Name)
            };

            switch (_key.GatewayType)
            {
                case GatewayKeyType.Microservice:
                    claims.Add(new Claim(ClaimTypes.Spn, nameof(GatewayKeyType.Microservice)));
                    break;
                case GatewayKeyType.Device:
                    claims.Add(new Claim(ClaimTypes.Spn, nameof(GatewayKeyType.Device)));
                    break;
                case GatewayKeyType.ThirdParty:
                    claims.Add(new Claim(ClaimTypes.Spn, nameof(GatewayKeyType.ThirdParty)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(
                    new[] {new ClaimsIdentity(claims, Options.AuthenticationType)}), Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }
        
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            await base.HandleChallengeAsync(properties);

            if (_key != null)
            {
                var errorResponse = new ErrorApiResponse("Deactivated");
                
                Context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                await Context.CreateResponse(errorResponse);
            }
        }
    }
}