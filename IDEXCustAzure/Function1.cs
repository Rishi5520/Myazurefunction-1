using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IDEXCustAzure
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _httpClient = new HttpClient();
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

                if (myTimer.ScheduleStatus is not null)
                {
                    _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
                }

                // Your target URL
                string url = "https://myidexhubprod.azurewebsites.net/api/v1/dummy/encode_request";

                // Create payload
                var payload = new
                {
                    email = "admin@test.com",
                    password = "12345678"
                };

                // Serialize payload to JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send POST request
                var response = await _httpClient.PostAsync(url, content);

                // Read response content
                var responseContent = await response.Content.ReadAsStringAsync();

                // Log the status code and response content
                _logger.LogInformation($"Status Code: {response.StatusCode}");
                _logger.LogInformation($"Response: {responseContent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the function execution.");
            }
        }
    }
}
