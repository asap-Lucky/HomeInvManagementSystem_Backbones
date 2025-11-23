using HomeInvManagementAPI.DTOs.OpenFoodFactsProduct;
using HomeInvManagementAPI.DTOs.OpenFoodFactsProduct.Outbound;
using HomeInvManagementAPI.Interfaces.OpenFoodFacts;
using System.Text.Json;

namespace HomeInvManagementAPI.Services.OpenFoodFacts
{
    public class OpenFoodFactsService : IOpenFoodFactsService
    {
        // Injections
        private readonly ILogger<OpenFoodFactsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        // Fields
        private readonly string _apiBaseUrl;

        public OpenFoodFactsService(ILogger<OpenFoodFactsService> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;

            _apiBaseUrl = _configuration.GetValue<string>("ExternalApis:OpenFoodFactsUrl", "https://world.openfoodfacts.org/api/v0/product/");
        }

        public async Task<OpenFoodFactsResponseApiDto>? GetProductByEanCodeAsync(string eanCode)
        {
            try
            {
                string urlPath = $"{_apiBaseUrl}{eanCode}.json";

                var request = new HttpRequestMessage(HttpMethod.Get, urlPath);

                var authenticationString = $"off:off";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));

                request.Headers.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var productResponse = JsonSerializer.Deserialize<OpenFoodFactsResponseApiDto>(content);

                    try
                    {
                        if (productResponse != null)
                        {
                            return productResponse;
                        }
                        
                        throw new Exception("Failed to deserialize product response from OpenFoodFacts.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Deserialization Failure.");
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product details from OpenFoodFacts.");
                throw;
            }
        }
    }
}
