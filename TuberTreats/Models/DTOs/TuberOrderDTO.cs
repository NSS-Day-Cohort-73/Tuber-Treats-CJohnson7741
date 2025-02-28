namespace TuberTreats.Models.DTOs;

    public class TuberOrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderPlacedOnDate { get; set; }
        public DateTime? DeliveredOnDate { get; set; }
        public int CustomerId { get; set; }
        public int? TuberDriverId { get; set; }
        
        // List of toppings for this order
        public List<ToppingDTO> Toppings { get; set; } = new List<ToppingDTO>();
    }




