using ImpersonationDemo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using System.Web;

namespace ImpersonationDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationData _authenticationData;
        public List<WeatherForecast> WeatherForecastData { get; set; } = new List<WeatherForecast>();

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, AuthenticationData authenticationData) 
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _authenticationData = authenticationData;
        }

        private HttpClient GetClient()
        {
            // retrieve the client we initialized in Program.cs that supports Windows Authentication
            return _httpClientFactory.CreateClient("WindowsAuthenticationClient");
        }

        public async Task OnGet() 
        {
            try
            { 
                if (_authenticationData.WindowsIdentity != null)
                {
                    var user = (WindowsIdentity)_authenticationData.WindowsIdentity;

                    if (user != null)
                    {
#pragma warning disable CA1416 // Validate platform compatibility
                        await WindowsIdentity.RunImpersonated(user.AccessToken, async () =>
                        {
                            var httpClient = GetClient();

                            var result = await httpClient.GetAsync("https://localhost/ImpersonationDemoAPI/WeatherForecast");

                            if (result.IsSuccessStatusCode)
                            {
                                var returnedWeatherForecastData = await result.Content.ReadFromJsonAsync<List<WeatherForecast>>();

                                if (returnedWeatherForecastData != null)
                                {
                                    WeatherForecastData = returnedWeatherForecastData;
                                }
                            }
                            else
                            {
                                _logger.LogError(HttpUtility.HtmlDecode(await result.Content.ReadAsStringAsync()));
                            }
                        });
#pragma warning restore CA1416 // Validate platform compatibility
                    }
                    else
                    {
                        WeatherForecastData = new List<WeatherForecast>();
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
