namespace src.models
{
    public record CifrarRequest
    {
        public string TextoClaro { get; set; }
        public string Deslocamento { get; set; }
    }
}