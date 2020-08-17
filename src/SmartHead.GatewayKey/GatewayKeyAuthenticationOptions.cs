using Microsoft.AspNetCore.Authentication;

namespace SmartHead.GatewayKey
{
    public class GatewayKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "GatewayKey";
        public string AuthenticationType = DefaultScheme;
        public string GatewayKeyHeaderName { get; set; }
        public string Scheme => DefaultScheme;
    }
}