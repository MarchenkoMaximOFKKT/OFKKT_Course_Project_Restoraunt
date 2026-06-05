using System;

namespace Models;
public class Booking{
    public int Id{get; set;}
    public int IdCustomer{get; set;}
    public DateTime BookingStartTime{get; set;}
    public DateTime BookingEndTime{get; set;}
    public Customer Customer{get; set;}=null!;
}