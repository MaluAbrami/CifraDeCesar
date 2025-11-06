namespace src.models
{
    public record CifrarRequest
    {
        public required string TextoClaro { get; set; }
        public required string Deslocamento { get; set; }
    }
}