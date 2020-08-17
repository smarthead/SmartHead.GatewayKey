using System;
using Microsoft.AspNetCore.Authentication;

namespace SmartHead.GatewayKey
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddGatewayKey<T>(this AuthenticationBuilder authenticationBuilder,
            Action<GatewayKeyAuthenticationOptions> options) where T : GatewayKeyBase
            => authenticationBuilder.AddScheme<GatewayKeyAuthenticationOptions, GatewayKeyAuthenticationHandler<T>>(
                GatewayKeyAuthenticationOptions.DefaultScheme, options);
    }
}