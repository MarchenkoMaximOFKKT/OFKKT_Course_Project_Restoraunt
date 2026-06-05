using System;
using System.Collections.Generic;
using System.Linq;
using Repositories;
using Input;
using Models;
 
namespace Services;
public class OrderService{
    private readonly OrderRepository _orderRepo;
    private readonly OrderDishService _orderDishServ;
    private readonly TableSheduleRepository _tableSheduleRepo;
    private readonly TableService _tableServ;
    private readonly PersonnelRepository _personnelRepo;
    private readonly BookingRepository _bookingRepo;
    private readonly PaymentService _paymentServ;
    public OrderService(OrderRepository orderRepo, OrderDishService orderDishServ, TableSheduleRepository tableSheduleRepo, TableService tableServ, PersonnelRepository personnelRepo, BookingRepository bookingRepo, PaymentService paymentServ){
        this._orderRepo=orderRepo;
        this._orderDishServ=orderDishServ;
        this._tableSheduleRepo=tableSheduleRepo;
        this._tableServ=tableServ;
        this._personnelRepo=personnelRepo;
        this._bookingRepo=bookingRepo;
        this._paymentServ=paymentServ;
    }
    public (bool Success, Order? order, string Message) CreateOrder(InputOrder input){
        var person=this._personnelRepo.GetFirstAvailible();
        if(person==null || !person.IsActive || person.Priority>=5) return (false, null, "Офіціанти зайняті в даний момент");
        if(input.Type=="Бронювання"){
            if(!this._bookingRepo.Add(input.IdCustomer, input.StartTime, input.EndTime)) return (false, null, "Не вдалося створити бронювання");
        }
        if(!this._orderRepo.Add(input.IdCustomer, person.Id, input.StartTime, input.EndTime, input.Type, input.Comment)) return (false, null, "Не вдалося зробити замовлення");
        var order=this._orderRepo.GetActive(input.IdCustomer);
        if(order==null) return (false, null, "Помилка отримання створеного замовлення");
        if(!this._personnelRepo.ChangeStatus(person.Id, person.IsActive, person.Priority+1)) return (false, null, "Помилка оновлення даних персоналу");
        var (ok, payment, msg)=this._paymentServ.Add(order.Id, order.TotalPrice);
        if(!ok) return (false, null, msg);
        return (true, order, "Замовлення створено");
    }
    public (bool Success, string Message) AddTablesToOrder(int idOrder, InputOrder input){
        if(input.IdTables.Count()==0) return (false, "Оберіть хочаб один стіл");
        var conflicts=this._tableSheduleRepo.GetOverlaps(input.IdTables, input.StartTime, input.EndTime);
        if(conflicts.Any()) return (false, input.IdTables.Count>1 ? "Столи зайняті" : "Стіл зайнятий");
        var booking=this._bookingRepo.GetByCustomer(input.IdCustomer, input.StartTime);
        var (success, message)=this._tableServ.OcuppateTable(input.IdTables, input.StartTime, input.EndTime, input.Type=="Бронювання" ? "Бронювання" : "Замовлення", idOrder, input.Type=="Бронювання" ? booking?.Id : default);
        if(!success) return (false, message);
        return (true, input.IdTables.Count>1 ? "Столи" : "Стіл"+" додан"+(input.IdTables.Count>1 ? "і" : "")+" до замовлення");
    }
    public Order? GetActive(int idCustomer)=>this._orderRepo.GetActive(idCustomer);
    public List<Order> GetHistory(int idCustomer)=>this._orderRepo.GetAll(idCustomer);
    public List<OrderDish> GetDishes(int idOrder)=>this._orderDishServ.GetDishes(idOrder);
    public (bool Success, string Message) AddDish(InputOrderDish input){
        return this._orderDishServ.AddDish(input.IdOrder, input.IdDish, input.Quantity);
    }
    public (bool Success, string Message) RemoveDish(InputOrderDish input){
        var orderDishes=this._orderDishServ.GetDishes(input.IdOrder);
        if(orderDishes==null || orderDishes.Count==0) return (false, "Нема страв для видалення");
        var orderDish=orderDishes.FirstOrDefault(o=>o.IdDish==input.IdDish);
        if(orderDish==null) return (false, "В замовленні поки немає такої страви");
        return this._orderDishServ.RemoveDish(orderDish.Id);
    }
    public (bool Success, string Message) UpdateQuantity(InputOrderDish input){
        return this._orderDishServ.UpdateQuantity(input.IdOrder, input.IdDish, input.Quantity);
    }
    public (bool Success, string Message) CancelOrder(int idOrder, int idCustomer){
        var order=this._orderRepo.Get(idOrder);
        if(order==null) return (false, "Замовлення не знайдено");
        if(order.IdCustomer!=idCustomer) return (false, "Немає доступу до цього замовлення");
        if(order.Status=="Оплачено") return (false, "Оплачене замовлення скасувати неможливо");
        if(order.StartTime<=DateTime.Now.AddMinutes(-10)) return (false, "Замовлення вже розпочато — скасування недоступне");
        var (freed, message)=this._tableServ.FreeTable(idOrder: idOrder);
        if(!freed) return (false, message);
        if(order.IdPersonnel>0){
            var person=this._personnelRepo.Get(order.IdPersonnel);
            if(person!=null){
                int newPriority=Math.Max(0, person.Priority-1);
                this._personnelRepo.ChangeStatus(person.Id, person.IsActive, newPriority);
            }
        }
        if(!this._orderRepo.UpdateStatus(idOrder, "Скасовано")) return (false, "Помилка при скасуванні замовлення");
        return (true, "Замовлення скасовано");
    }
}