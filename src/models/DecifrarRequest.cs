namespace src.models
{
    public record DecifrarRequest
    {
        public string TextoCifrado { get; set; }
        public string Deslocamento { get; set; }
    }
}