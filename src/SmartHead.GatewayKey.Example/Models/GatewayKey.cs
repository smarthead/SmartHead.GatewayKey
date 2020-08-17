namespace SmartHead.GatewayKey.Example.Models
{
    public class GatewayKey : GatewayKeyBase
    {
        public GatewayKey(string key, string name, GatewayKeyType gatewayType, bool isActive = false)
            :base(key, name, gatewayType)
        {
            IsActive = isActive;
        }
    }
}