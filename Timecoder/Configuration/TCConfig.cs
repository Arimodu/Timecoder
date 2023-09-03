using System;
using IPA.Config.Stores;
using System.Runtime.CompilerServices;
using Timecoder.Network.Models;
using IPA.Config.Stores.Attributes;
using Timecoder.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace Timecoder.Configuration
{
    public class TCConfig
    {
        public event Action<TCConfig> OnChanged;

        public virtual string BaseUrl { get; set; } = "https://timecoder.dduel.dev";
        public virtual string SessionEndpoint { get; set; } = "/api/v0/integration/me";
        public virtual string EventPush { get; set; } = "/api/v0/event/push";
        public virtual string SubEventPush { get; set; } = "/api/v0/subEvent/push";

        [UseConverter(typeof(UserTokenConverter))]
        public virtual UserToken Token { get; set; } = new UserToken("mocktoken", "IntegrationV0");

        public virtual string GetSessionURI() => $"{BaseUrl}{SessionEndpoint}";

        public virtual string GetEventPushURI() => $"{BaseUrl}{EventPush}";

        public virtual string GetSubEventPushURI() => $"{BaseUrl}{SubEventPush}";

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed(bool broadcastChange = true)
        {
            if (broadcastChange) OnChanged?.Invoke(this);
        }
    }
}
