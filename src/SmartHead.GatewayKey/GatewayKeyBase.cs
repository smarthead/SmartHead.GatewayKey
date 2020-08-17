using System;
using EF.DataProtection.Services.Aes256;
using EF.DataProtection.Services.Sha512;
using SmartHead.Essentials.Abstractions.Ddd;

namespace SmartHead.GatewayKey
{
    public abstract class GatewayKeyBase : Entity, IEquatable<GatewayKeyBase>
    {
        protected GatewayKeyBase()
        {
        }

        public bool IsRevokeRequired { get; protected set; }

        protected GatewayKeyBase(string key, string name, GatewayKeyType gatewayType)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Key = key;
            Name = name;
            IsActive = true;
            GatewayType = gatewayType;
        }

        public GatewayKeyBase Invalidate()
        {
            IsActive = false;
            return this;
        }

        public GatewayKeyBase RevokeRequest()
        {
            IsRevokeRequired = !IsRevokeRequired;
            return this;
        }

        [Sha512]
        public string Key { get; protected set; }

        [Aes256]
        public string Name { get; protected set; }

        [Aes256]
        public string Description { get; protected set; }

        public bool IsActive { get; protected set; }

        public GatewayKeyType GatewayType { get; set; }

        public bool Equals(GatewayKeyBase other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(Name, other.Name) && GatewayType == other.GatewayType;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((GatewayKeyBase)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (int)GatewayType;
            }
        }
    }
}
