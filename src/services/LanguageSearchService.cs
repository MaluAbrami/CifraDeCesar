using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;

public class DicionarioAbertoValidator : IDisposable
{
    private readonly HttpClient _httpClient;

    public DicionarioAbertoValidator(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; PalavraChecker/1.0)");
    }

    public async Task<bool> FraseExisteEmPortuguesAsync(string frase)
    {
        if (string.IsNullOrWhiteSpace(frase))
            return false;

        // Extrai apenas palavras (letras, ignora pontuação e números)
        var palavras = Regex.Matches(frase, @"\p{L}+")
                            .Select(m => m.Value)
                            .ToList();

        foreach (var palavra in palavras)
        {
            if (!await PalavraExisteAsync(palavra))
                return false;
        }

        return true;
    }

    private async Task<bool> PalavraExisteAsync(string palavra)
    {
        string normalizada = RemoverAcentos(palavra).ToLowerInvariant();
        string url = $"https://www.dicio.com.br/{Uri.EscapeDataString(normalizada)}/";

        try
        {
            HttpResponseMessage resposta = await _httpClient.GetAsync(url);

            if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            if (resposta.IsSuccessStatusCode)
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static string RemoverAcentos(string texto)
    {
        var formD = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (char ch in formD)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (categoria != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
