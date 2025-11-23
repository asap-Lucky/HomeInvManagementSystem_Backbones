using Newtonsoft.Json;

namespace HomeInvManagementAPI.DTOs.OpenFoodFactsProduct.Outbound
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
