namespace Models;
public class Payment{
    public int Id{get; set;}
    public int IdOrder{get; set;}
    public decimal Total{get; set;}
    public string PaymentMethod{get; set;}=string.Empty;
    public Order Order{get; set;}=null!;
}