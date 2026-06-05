using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Restoraunt.Views;
using Repositories;
using Services;
using Validators;
using Input;
using Models;
using System.Collections.Generic;

namespace Restoraunt;
public partial class App : Application{
    public Customer? customer{get; set;}
    public Menu? menu{get; set;}
    public List<Order> orders{get; set;}=new List<Order>();
    public List<OrderDish> orderDishes{get; set;}=new List<OrderDish>();
    public override void Initialize()=>AvaloniaXamlLoader.Load(this);
    public override void OnFrameworkInitializationCompleted(){
        if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop){
            var inputCustomer=new InputCustomer();
            var inputOrder=new InputOrder();
            var inputOrderDish=new InputOrderDish();
            var inputPayment=new InputPayment();
            var inputPersonnel=new InputPersonnel();

            var phoneval=new PhoneValidator();
            var nameVal=new FullNameValidator();
            var dateTimeVal=new DateTimeValidator();

            var bookingRepo=new BookingRepository();
            var categoryRepo=new CategoryRepository(); 
            var customerRepo=new CustomerRepository();
            var dishRepo=new DishRepository();
            var menuRepo=new MenuRepository();
            var orderDish=new OrderDishRepository();
            var paymentRepo=new PaymentRepository();
            var personnelRepo=new PersonnelRepository();
            var tableRepo=new TableRepository();
            var tableSheduleRepo=new TableSheduleRepository();
            var orderRepo=new OrderRepository(tableRepo);
            var orderDishRepo=new OrderDishRepository();

            var customerServ=new CustomerService(customerRepo, phoneval, nameVal);
            var tableServ=new TableService(tableRepo, tableSheduleRepo);
            var paymentServ=new PaymentService(paymentRepo, orderRepo, orderDishRepo, personnelRepo, tableServ);
            var orderDishServ=new OrderDishService(orderDishRepo, orderRepo, dishRepo, paymentServ);
            var orderServ=new OrderService(orderRepo, orderDishServ, tableSheduleRepo, tableServ, personnelRepo, bookingRepo, paymentServ);

            desktop.MainWindow=new MainWindow(orderRepo, menuRepo, customerServ, orderServ, paymentServ, tableServ, dishRepo, personnelRepo);
        }
        base.OnFrameworkInitializationCompleted();
    }
}