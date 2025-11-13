using System.ComponentModel.DataAnnotations;
using System.Text;
using src.models;
namespace src.services
{
    public class CifraService
    {
        private const string Alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string AlfabetoMinusculo = "abcdefghijklmnopqrstuvwxyz";
        private readonly DicionarioAbertoValidator _languageSearchService;
        private readonly ILogger<CifraService> _logger;

        public CifraService(DicionarioAbertoValidator languageSearchService, ILogger<CifraService> logger)
        {
            _languageSearchService = languageSearchService;
            _logger = logger;
        }

        public CifrarResponse Cifrar(CifrarRequest request)
        {
            var textoCifrado = new StringBuilder();
            int desloc = ((request.Deslocamento % Alfabeto.Length) + Alfabeto.Length) % Alfabeto.Length;

            foreach (var ch in request.TextoClaro)
            {
                var proxChar = ch switch
                {
                    _ when Alfabeto.Contains(ch) =>
                        Alfabeto[(Alfabeto.IndexOf(ch) + desloc) % Alfabeto.Length],

                    _ when AlfabetoMinusculo.Contains(ch) =>
                        AlfabetoMinusculo[(AlfabetoMinusculo.IndexOf(ch) + desloc) % AlfabetoMinusculo.Length],

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
            var biggestScoreResponse;

            int index = 0;
            while (index < 26)
            {
                index++;
                var decifrarResponse = Decifrar(new DecifrarRequest(request.TextoCifrado, index));
                _logger.LogInformation($"Retorno do decifrar: {decifrarResponse.TextoClaro}");

                var responseApiExternal = await _languageSearchService.FraseExisteEmPortuguesAsync(decifrarResponse.TextoClaro);
                _logger.LogInformation($"Retorno da api externa: {responseApiExternal}");

                if(biggestScoreResponse == null || responseApiExternal > biggestScoreResponse)
                {
                    biggestScoreResponse = responseApiExternal;
                    textoClaro = decifrarResponse.TextoClaro;
                }
            }

            if (textoClaro == "")
                throw new Exception("Não foi possível encontrar texto válido");

            return new DecifrarResponse(textoClaro);
        }
    }
}