using Newtonsoft.Json;

namespace Timecoder.Network.Models
{
    internal class Session
    {
        public string Id { get; }

        public Session(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
