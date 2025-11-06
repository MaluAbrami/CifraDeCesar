namespace src.models
{
    public record CifrarRequest
    {
        public string TextoClaro { get; set; }
        public int Deslocamento { get; set; }
    }
}