namespace TuberTreats.Models;

public class TuberDriver
{
    public int Id { get; set; }
    public string Name { get; set; }  // Driver's name
    
    public List<TuberOrder> TuberDeliveries { get; set; } = new List<TuberOrder>();  // List of orders assigned to this driver
}