using System;

namespace Models;
public class TableShedule{
    public int Id{get; set;}
    public int IdTable{get; set;}
    public int? IdOrder{get; set;}
    public int? IdBooking{get; set;}
    public DateTime StartTime{get; set;}
    public DateTime EndTime{get; set;}
    public string EventType{get; set;}=string.Empty;
}