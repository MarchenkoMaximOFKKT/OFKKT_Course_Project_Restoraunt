namespace Models;
public class Dish{
    public int Id{get; set;}
    public int IdCategory{get; set;}
    public string Name{get; set;}=string.Empty;
    public decimal Price{get; set;}
    public Category Category{get; set;}=null!;
}