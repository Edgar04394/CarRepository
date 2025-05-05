namespace CarApi.Models
{
    public class Carro
    {
        public int Id { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? Anio { get; set; }
        public string? Color { get; set; }
        public decimal Precio { get; set; }
    }
}