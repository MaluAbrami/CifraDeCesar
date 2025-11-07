using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace src.services
{
    public class LanguageSearchService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public LanguageSearchService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<string> DetectLanguageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Texto não pode ser vazio.", nameof(text));

            string url = $"http://api.languagelayer.com/detect?access_key={_configuration["languageApiKey"]}&query={Uri.EscapeDataString(text)}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            if (root.TryGetProperty("results", out JsonElement results) && results.GetArrayLength() > 0)
            {
                var first = results[0];
                return first.GetProperty("language_code").GetString()!;
            }

            return null!;
        }
    }
}