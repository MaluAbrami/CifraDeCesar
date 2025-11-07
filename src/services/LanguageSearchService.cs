using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace src.services
{
    public class LanguageSearchService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<LanguageSearchService> _logger;

        public LanguageSearchService(IConfiguration configuration, ILogger<LanguageSearchService> logger)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<string> DetectLanguageAsync(string text)
        {
            _logger.LogInformation($"Iniciando chamada da API Detect Language com o texto: {text}");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Texto não pode ser vazio.", nameof(text));

            string apiKey = _configuration["languageApiKey"]!;
            string url = "https://ws.detectlanguage.com/v3/detect";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            request.Content = new StringContent($"q={Uri.EscapeDataString(text)}", Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Retorno real da api: {json}");

            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.GetArrayLength() > 0)
            {
                // Prioridade para 'pt'
                foreach (var item in root.EnumerateArray())
                {
                    if (item.GetProperty("language").GetString() == "pt")
                    {
                        return "pt";
                    }
                }

                // Se não tiver 'pt', retorna o primeiro da lista
                return root[0].GetProperty("language").GetString()!;
            }

            return null!;
        }

    }
}
