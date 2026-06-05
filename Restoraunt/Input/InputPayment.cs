namespace Input;
public class InputPayment{
    public int IdOrder{get; set;}
    public decimal Total{get; set;}
    public string PaymentMethod{get; set;}=string.Empty;
}