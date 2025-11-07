using System.ComponentModel.DataAnnotations;
using System.Text;
using src.models;
namespace src.services
{
    public class CifraService
    {
        private const string Alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string AlfabetoMinusculo = "abcdefghijklmnopqrstuvwxyz";
        private readonly LanguageSearchService _languageSearchService;
        private readonly ILogger<CifraService> _logger;

        public CifraService(LanguageSearchService languageSearchService, ILogger<CifraService> logger)
        {
            _languageSearchService = languageSearchService;
            _logger = logger;
        }

        public CifrarResponse Cifrar(CifrarRequest request)
        {
            var textoCifrado = new StringBuilder();

            foreach (var ch in request.TextoClaro)
            {
                var proxChar = ch switch
                {
                    _ when Alfabeto.Contains(ch) =>
                        Alfabeto[(Alfabeto.IndexOf(ch) + request.Deslocamento) % Alfabeto.Length],

                    _ when AlfabetoMinusculo.Contains(ch) =>
                        AlfabetoMinusculo[(AlfabetoMinusculo.IndexOf(ch) + request.Deslocamento) % AlfabetoMinusculo.Length],

                    ' ' => ' ',

                    _ => throw new ValidationException("Caractere inválido encontrado")
                };

                textoCifrado.Append(proxChar);
            }

            return new CifrarResponse(textoCifrado.ToString());
        }

        public DecifrarResponse Decifrar(DecifrarRequest request)
        {
            CifrarResponse cifradoInverso = this.Cifrar(new CifrarRequest(request.TextoCifrado, -request.Deslocamento));
            return new DecifrarResponse(cifradoInverso.TextoCifrado);
        }

        public async Task<DecifrarResponse> DecifrarForcaBruta(DecifrarForcaBrutaRequest request)
        {
            string textoClaro = "";

            bool fimLoop = false;
            int index = 0;
            while (!fimLoop || index == 26)
            {
                index++;
                var decifrarResponse = Decifrar(new DecifrarRequest(request.TextoCifrado, index));
                _logger.LogInformation($"Retorno do decifrar: {decifrarResponse.TextoClaro}");

                var responseApiExternal = await _languageSearchService.DetectLanguageAsync(decifrarResponse.TextoClaro);
                _logger.LogInformation($"Retorno da api externa: {responseApiExternal}");
                if (responseApiExternal != null)
                {
                    if (responseApiExternal != "pt")
                        continue;

                    textoClaro = decifrarResponse.TextoClaro;
                    fimLoop = true;
                }
            }

            if (textoClaro == "")
                throw new Exception("Não foi possível encontrar texto válido");

            return new DecifrarResponse(textoClaro);
        }
    }
}