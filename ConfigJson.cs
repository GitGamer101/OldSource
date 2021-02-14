// Copyright [2021] [NotSoNitro]

using Newtonsoft.Json;

namespace Lunar_Bot
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
