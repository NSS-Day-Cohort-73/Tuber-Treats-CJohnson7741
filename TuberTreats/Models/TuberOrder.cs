namespace TuberTreats.Models;

public class TuberOrder
{
    public int Id { get; set; }
    public DateTime OrderPlacedOnDate { get; set; }
    public DateTime? DeliveredOnDate { get; set; }
    public int CustomerId { get; set; }
    public int? TuberDriverId { get; set; }
    
    // This is where we define the relationship to TuberTopping
    public List<Topping> Toppings { get; set; } = new List<Topping>();

    // Navigation properties for Customer and TuberDriver
    public Customer Customer { get; set; }
    public TuberDriver TuberDriver { get; set; }
}
