using System;
using System.Collections.Generic;

namespace Input;
public class InputOrder{
    public string Type{get; set;}=string.Empty;
    public DateTime StartTime{get; set;}
    public DateTime EndTime{get; set;}
    public string Comment{get; set;}=string.Empty;
    public List<int> IdTables{get; set;}=new List<int>();
    public int IdCustomer{get; set;}
}