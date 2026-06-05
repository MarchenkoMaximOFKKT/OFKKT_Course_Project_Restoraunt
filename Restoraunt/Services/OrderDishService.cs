using System;
using System.Collections.Generic;
using System.Linq;
using Repositories;
using Models;

namespace Services;
public class OrderDishService{
    private readonly OrderDishRepository _orderDishRepo;
    private readonly OrderRepository _orderRepo;
    private readonly DishRepository _dishRepo;
    private readonly PaymentService _paymentServ;
    public OrderDishService(OrderDishRepository orderDishRepo, OrderRepository orderRepo, DishRepository dishRepo, PaymentService paymentServ){
        this._orderDishRepo=orderDishRepo;
        this._orderRepo=orderRepo;
        this._dishRepo=dishRepo;
        this._paymentServ=paymentServ;
    }
    private void RecalculateTotal(int idOrder){
        var positions=this._orderDishRepo.GetAll(idOrder);
        decimal total=positions.Sum(od=>od.Dish.Price*od.Quantity);
        this._orderRepo.UpdateTotal(idOrder, total);
        this._paymentServ.UpdateTotal(idOrder, total);
    }
    public List<OrderDish> GetDishes(int idOrder)=>this._orderDishRepo.GetAll(idOrder);
    public (bool Success, string Message) AddDish(int idOrder, int idDish, int quantity){
        if(quantity<=0) return (false, "Кількість має бути більше нуля");
        var order=this._orderRepo.Get(idOrder);
        if(order==null) return (false, "Замовлення не знайдено");
        if(order.Status=="Оплачено") return (false, "Неможливо змінити оплачене замовлення");
        var dish=this._dishRepo.Get(idDish);
        if(dish==null) return (false, "Страву не знайдено");
        if(!this._orderDishRepo.Add(idOrder, idDish, quantity)) return (false, $"Не вдалося додати «{dish.Name}» до замовлення");
        RecalculateTotal(idOrder);
        return (true, $"«{dish.Name}» x{quantity} додано до замовлення");
    }
    public (bool Success, string Message) RemoveDish(int idOrderDish){
        var orderDish=this._orderDishRepo.Get(idOrderDish);
        if(orderDish==null) return (false, "Позицію не знайдено");
        var order=this._orderRepo.Get(orderDish.IdOrder);
        if(order?.Status=="Оплачено") return (false, "Неможливо змінити оплачене замовлення");
        if(!this._orderDishRepo.Remove(idOrderDish)) return (false, "Не вдалося видалити страву з замовлення");
        RecalculateTotal(orderDish.IdOrder);
        return (true, "Страву видалено з замовлення");
    }
    public (bool Success, string Message) UpdateQuantity(int idOrder, int idDish, int quantity){
        if(quantity<=0) return (false, "Кількість має бути більше нуля");
        var order=this._orderRepo.Get(idOrder);
        if(order==null) return (false, "Замовлення не знайдено");
        if(order.Status=="Оплачено") return (false, "Неможливо змінити оплачене замовлення");
        if(!this._orderDishRepo.UpdateQuantity(idOrder, idDish, quantity)) return (false, "Не вдалося оновити кількість");
        RecalculateTotal(idOrder);
        return (true, "Кількість оновлено");
    }
}