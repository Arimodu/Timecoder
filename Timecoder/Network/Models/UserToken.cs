using System;

namespace Timecoder.Network.Models
{
    [Serializable]
    public class UserToken
    {
        public string TokenString { get; }
        public string Version { get; }

        public UserToken(string tokenString, string version)
        {
            TokenString = tokenString;
            Version = version;
        }

        public override string ToString() => $"{Version} {TokenString}";
    }
}
