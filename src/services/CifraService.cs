using System.Text;
using src.models;
namespace src.services
{
    public class CifraService
    {
        private const string Alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string AlfabetoMinusculo = "abcdefghijklmnopqrstuvwxyz";


        public CifrarResponse Cifrar(CifrarRequest request)
        {
            var textoCifrado = new StringBuilder();

            foreach (var ch in request.TextoClaro)
            {
                if (Alfabeto.Contains(ch))
                {
                    int index = Alfabeto.IndexOf(ch);

                    int proxIndex = (index + request.Deslocamento) % Alfabeto.Length;
                    textoCifrado.Append(Alfabeto[proxIndex]);
                }
                else if (AlfabetoMinusculo.Contains(ch))
                {
                    int index = AlfabetoMinusculo.IndexOf(ch);

                    int proxIndex = (index + request.Deslocamento) % AlfabetoMinusculo.Length;
                    textoCifrado.Append(AlfabetoMinusculo[proxIndex]);
                }
                else
                {
                    textoCifrado.Append(ch);
                }
            }

            return new CifrarResponse(textoCifrado.ToString());
        }

        public DecifrarResponse Decifrar(DecifrarRequest request)
        {
            CifrarResponse cifradoInverso = this.Cifrar(new CifrarRequest(request.TextoCifrado, -request.Deslocamento));
            return new DecifrarResponse(cifradoInverso.TextoCifrado);
        }
    }
}