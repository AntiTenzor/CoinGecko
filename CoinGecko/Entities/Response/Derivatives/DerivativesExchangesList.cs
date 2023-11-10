using Newtonsoft.Json;

namespace CoinGecko.Entities.Response.Derivatives
{
    public class DerivativesExchangesList
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public override string ToString()
        {

            string res = $"id:{Id}; Name:{Name}";
            return res;
        }
    }
}