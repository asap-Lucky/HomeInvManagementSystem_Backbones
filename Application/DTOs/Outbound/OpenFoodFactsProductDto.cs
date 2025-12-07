using Newtonsoft.Json;

namespace Application.DTOs.Outbound
{
    public class OpenFoodFactsProductDto
    {
        [JsonProperty("_id")]
        public string? ProductId { get; set; }

        [JsonProperty("_keywords")]
        public List<string>? Keywords { get; set; }

        [JsonProperty("brands")]
        public string? Brand { get; set; }

        [JsonProperty("brands_tags")]
        public List<string>? BrandTags { get; set; }

        [JsonProperty("countries")]
        public List<string>? Countries { get; set; }    

        [JsonProperty("countries_tags")]
        public List<string>? CountryTags { get; set; }

        [JsonProperty("product_name")]
        public string? ProductName { get; set; }

        [JsonProperty("product_type")]
        public string? ProductType { get; set; }
    }
}
