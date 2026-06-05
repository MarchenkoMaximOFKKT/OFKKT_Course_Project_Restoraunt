using System;
using System.Collections.Generic;

namespace Models;
public class Order{
    public int Id{get; set;}
    public int IdCustomer{get; set;}
    public int IdPersonnel{get; set;}
    public string Type{get; set;}=string.Empty;
    public DateTime StartTime{get; set;}
    public DateTime EndTime{get; set;}
    public string Comment{get; set;}=string.Empty;
    public string Status{get; set;}=string.Empty;
    public decimal TotalPrice{get; set;}
    public Customer Customer{get; set;}=null!;
    public List<Table> Tables{get; set;}=new List<Table>();
    public Personnel Personnel{get; set;}=null!;
}