namespace Models;
public class Personnel{
    public int Id{get; set;}
    public string FullName{get; set;}=string.Empty;
    public string Position{get; set;}=string.Empty;
    public string Phone{get; set;}=string.Empty;
    public bool IsActive{get; set;}
    public int Priority{get; set;}
    public decimal Salary{get; set;}
}