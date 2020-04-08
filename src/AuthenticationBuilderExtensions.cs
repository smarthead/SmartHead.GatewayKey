using System;
using Microsoft.AspNetCore.Authentication;

namespace SmartHead.GatewayKey
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddGatewayKey(this AuthenticationBuilder authenticationBuilder,
            Action<GatewayKeyAuthenticationOptions> options)
            => authenticationBuilder.AddScheme<GatewayKeyAuthenticationOptions, GatewayKeyAuthenticationHandler>(
                GatewayKeyAuthenticationOptions.DefaultScheme, options);
    }
}