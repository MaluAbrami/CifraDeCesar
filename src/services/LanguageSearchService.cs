using System.Text;
using System.Text.Json;
using System.Globalization;
using System.Text.RegularExpressions;

public class DicionarioAbertoValidator : IDisposable
{
    private readonly HttpClient _httpClient;

    public DicionarioAbertoValidator(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<bool> FraseExisteEmPortuguesAsync(string frase)
    {
        if (string.IsNullOrWhiteSpace(frase))
            return false;

        // Quebra em palavras (somente letras)
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
        string url = $"https://api.dicionario-aberto.net/near/{Uri.EscapeDataString(palavra)}";

        HttpResponseMessage resposta;

        try
        {
            resposta = await _httpClient.GetAsync(url);
        }
        catch
        {
            return false;
        }

        if (!resposta.IsSuccessStatusCode)
            return false;

        string json = await resposta.Content.ReadAsStringAsync();

        try
        {
            var resultados = JsonSerializer.Deserialize<string[]>(json);

            if (resultados is null || resultados.Length == 0)
                return false;

            foreach (var palavraRetornada in resultados)
            {
                string normalizadaRetornada = RemoverAcentos(palavraRetornada).ToLowerInvariant();
                if (normalizadaRetornada == normalizada)
                    return true;
            }

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
