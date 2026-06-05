using System.Collections.Generic;

namespace Models;
public class Menu{
    public int Id{get; set;}
    public string Name{get; set;}=string.Empty;
    public List<Dish> Dishes{get; set;}=new List<Dish>();
}