using Application.DTOs.Outbound;
using HomeInvManagementAPI.Interfaces.Repositories;
using HomeInvManagementAPI.Services;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HomeInvManagementAPI.Repositories
{
    public class OpenFoodFactsRepository : IOpenFoodFactsRepository
    {
        // Injections
        private readonly ILogger<OpenFoodFactsRepository> _logger;
        private readonly HttpClient _httpClient;
        private readonly HomeInvOptions _options;

        public OpenFoodFactsRepository(ILogger<OpenFoodFactsRepository> logger, HttpClient httpClient, HomeInvOptions options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<OpenFoodFactsResponseApiDto>? GetByEanAsync(string ean)
        {
            try
            {
                string urlPath = $"{_options.OFFApiUrl}{ean}.json";
                    
                var request = new HttpRequestMessage(HttpMethod.Get, urlPath);

                request.Headers.Add("Authorization", $"Basic {_options.OFFAuthToken}");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var productResponse = JsonConvert.DeserializeObject<OpenFoodFactsResponseApiDto>(content);

                    try
                    {
                        if (productResponse != null)
                            return productResponse;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize product response from OpenFoodFacts.");
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
