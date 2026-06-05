namespace Models;
public class OrderDish{
    public int Id{get; set;}
    public int IdOrder{get; set;}
    public int IdDish{get; set;}
    public int Quantity{get; set;}
    public Order Order{get; set;}=null!;
    public Dish Dish{get; set;}=null!;
}