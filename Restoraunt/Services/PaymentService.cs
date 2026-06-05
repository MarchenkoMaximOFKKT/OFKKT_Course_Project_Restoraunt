using System;
using System.Linq;
using Repositories;
using Models;
using System.Data.Common;
using Avalonia.Controls;

namespace Services;
public class PaymentService{
    private readonly PaymentRepository _paymentRepo;
    private readonly OrderRepository _orderRepo;
    private readonly OrderDishRepository _orderDishRepo;
    private readonly PersonnelRepository _personnelRepo;
    private readonly TableService _tableServ;
    public PaymentService(PaymentRepository paymentRepo, OrderRepository orderRepo, OrderDishRepository orderDishRepo, PersonnelRepository personnelRepo, TableService tableServ){
        this._paymentRepo=paymentRepo;
        this._orderRepo=orderRepo;
        this._orderDishRepo=orderDishRepo;
        this._personnelRepo=personnelRepo;
        this._tableServ=tableServ;
    }
    public (bool Success, Payment? payment, string Message) Add(int idOrder, decimal total){
        if(total<0) return (false, null, "Замовлення повинно мати додатню суму");
        var added=this._paymentRepo.Add(idOrder, total);
        if(!added) return (false, null, "Не вдалося створити платіж");
        var payment=GetReceipt(idOrder);
        if(payment==null) return (false, null, "Платіж не знайдено");
        return (true, payment, "Платіж успішно додано");
    }
    public (bool Success, string Message) Pay(int idOrder, string method="Готівка"){
        var order=this._orderRepo.Get(idOrder);
        if(order==null) return (false, "Замовлення не знайдено");
        if(order.Status=="Оплачено") return (false, "Замовлення вже оплачено");
        var dishes=this._orderDishRepo.GetAll(idOrder);
        if(!dishes.Any()) return (false, "Неможливо оплатити порожнє замовлення — спочатку додайте страви");
        decimal total=order.TotalPrice;
        if(total<=0) return (false, "Сума замовлення некоректна");
        if(!this._orderRepo.UpdateStatus(idOrder, "Оплачено")) return (false, "Помилка при оновленні статусу замовлення");
        this._tableServ.FreeTable(idOrder: idOrder);
        if(order.IdPersonnel>0){
            var person=this._personnelRepo.Get(order.IdPersonnel);
            if(person!=null){
                int newPriority=Math.Max(0, person.Priority-1);
                this._personnelRepo.ChangeStatus(person.Id, person.IsActive, newPriority);
            }
        }
        return (true, $"Оплату на суму {total}₴ прийнято. Метод: {method}");
    }
    public (bool Success, string Message) UpdateTotal(int idOrder, decimal total){
        if(total<0) return (false, "Неможливо змінити суму замовлення");        
        var payment=GetReceipt(idOrder);
        if(payment==null) return (false, "Платіж не знайдено для оновлення");
        if(!this._paymentRepo.UpdateTotal(payment.Id, total)) return (false, "Невдалося оновити суму замовлення");
        return (true, "Cуму замовлення оновлено");
    }
    public (bool Success, string Message) ChangeMethod(int idOrder, string method){
        var order=this._orderRepo.Get(idOrder);
        if(order==null) return (false, "Замовлення не знайдено");
        if(order.Status=="Оплачено" || order.Status=="Скасовано") return (false, "Неможливо змінити метод замовлення");
        var payment=this._paymentRepo.GetByOrder(idOrder);
        if(payment==null) return (false, "Платіж ще не створено — оплатіть замовлення першою дією");
        if(!this._paymentRepo.UpdateMethod(payment.Id, method)) return (false, "Не вдалося змінити метод оплати");
        return (true, $"Метод оплати успішно змінено на «{method}»");
    }
    public Payment? GetReceipt(int idOrder)=>this._paymentRepo.GetByOrder(idOrder);
}