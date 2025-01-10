namespace TuberTreats.Models;


public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }  // Customer's name
    public string Address { get; set; }  // Customer's address
    
    public List<TuberOrder> TuberOrders { get; set; } = new List<TuberOrder>();  // List of orders placed by this customer
}