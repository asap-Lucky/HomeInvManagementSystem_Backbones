using HomeInvManagementAPI.Interfaces.OpenFoodFacts;

namespace HomeInvManagementAPI.Services.OpenFoodFacts
{
    public class OpenFoodFactsService : IOpenFoodFactsService
    {
        // Injections
        private readonly ILogger<OpenFoodFactsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient = new HttpClient();

        public OpenFoodFactsService(ILogger<OpenFoodFactsService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
    }
}
