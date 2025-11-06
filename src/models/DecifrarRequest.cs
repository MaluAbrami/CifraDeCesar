namespace src.models
{
    public record DecifrarRequest
    {
        public string TextoCifrado { get; set; }
        public int Deslocamento { get; set; }
    }
}