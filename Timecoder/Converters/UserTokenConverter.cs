using IPA.Config.Data;
using IPA.Config.Stores;
using System;
using Timecoder.Network.Models;

namespace Timecoder.Converters
{
    internal class UserTokenConverter : ValueConverter<UserToken>
    {
        public override UserToken FromValue(Value value, object parent)
        {
            try
            {
                Map valueMap = value as Map;
                return new UserToken(
                    (valueMap["TokenString"] as Text).Value,
                    (valueMap["TokenVersion"] as Text).Value);
            }
            catch (Exception)
            {
                throw new ArgumentException("Failed to parse value as Map of UserToken", nameof(value));
            }
        }

        public override Value ToValue(UserToken token, object parent)
        {
            Map valueMap = Value.Map();
            valueMap.Add("TokenString", Value.Text(token.TokenString));
            valueMap.Add("TokenVersion", Value.Text(token.Version));
            return valueMap;
        }
    }
}
