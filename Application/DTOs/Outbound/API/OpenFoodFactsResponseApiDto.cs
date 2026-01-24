using Newtonsoft.Json;

namespace Application.DTOs.Outbound.API
{
    public class OpenFoodFactsResponseApiDto
    {
        [JsonProperty("code")]
        public string? EanCode { get; set; }

        [JsonProperty("product")]
        public OpenFoodFactsProductDto? Product { get; set; }

        [JsonProperty("status")]
        public int StatusCode { get; set; }

        [JsonProperty("status_verbose")]
        public string? StatusMessage { get; set; }
    }
}
