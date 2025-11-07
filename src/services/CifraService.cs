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

        public CifraService(LanguageSearchService languageSearchService)
        {
            _languageSearchService = languageSearchService;
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

        public DecifrarResponse DecifrarForcaBruta(DecifrarForcaBrutaRequest request)
        {
            string textoClaro = "";

            bool fimLoop = false;
            int index = 0;
            while (!fimLoop || index == 27)
            {
                index++;
                var decifrarResponse = Decifrar(new DecifrarRequest(request.TextoCifrado, index));

                var responseApiExternal = _languageSearchService.DetectLanguageAsync(decifrarResponse.TextoClaro);
                if (responseApiExternal != null)
                {
                    if (responseApiExternal.Result != "pt")
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