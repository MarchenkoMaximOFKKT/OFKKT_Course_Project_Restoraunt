using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using HarfBuzzSharp;
using Input;
using Microsoft.VisualBasic;
using Models;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Validators;

namespace Restoraunt.Views;
public class OrderSettings : UserControl{
    private readonly OrderRepository _orderRepo;
    private readonly OrderService _orderServ;
    private readonly PaymentService _paymentServ;
    private readonly TableService _tableServ;
    private readonly DishRepository _dishRepo;
    private readonly PersonnelRepository _personnelRepo;
    private readonly Action<UserControl> _navigateTo;
    private readonly Action<UserControl> _successNavigate;
    private void LoadData(){
        var orders=this._orderServ.GetHistory(((App)Application.Current!).customer!.Id);
        foreach(var order in orders) order.Personnel=this._personnelRepo.Get(order.IdPersonnel)!;
        ((App)Application.Current!).orders=orders;
    }
    private void LoadData(int idOrder){
        var dishes=this._dishRepo.GetAll();
        var orderDishes=this._orderServ.GetDishes(idOrder);
        foreach(var orderDish in orderDishes) foreach(var dish in dishes) if(orderDish.IdDish==dish.Id) orderDish.Dish=dish;
        ((App)Application.Current!).orderDishes=orderDishes;
    }
    private Button MakeButton(string name, int sizeX=256, int sizeY=52, string posY="Center", string posX="Center", int margin=4, int fontSize=18, byte bg=42)=>new Button{
        Content=name,
        Width=sizeX,
        Height=sizeY,
        FontSize=fontSize,
        Foreground=Brushes.White,
        Background=new SolidColorBrush(Color.FromRgb(bg, bg, bg)),
        CornerRadius=new CornerRadius(9),
        HorizontalContentAlignment=HorizontalAlignment.Center,
        VerticalContentAlignment=VerticalAlignment.Center,
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
        Margin=new Thickness(margin),
    };
    private UserControl MakePage(Control control, bool scrolleble=true, string posY="Center", string posX="Center"){
        var page=new UserControl{
            HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
            VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
            Content=scrolleble ? MakeScrolleble(control) : control,
        };
        return page;
    }
    private TextBlock MakeTextBlock(string text, string posY="Center", string posX="Center", int margin=0, int fontSize=22, string color="White", bool visible=true)=>new TextBlock{
        Text=text,
        Foreground=Brush.Parse(color),
        FontSize=fontSize,
        IsVisible=visible,
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
        Margin=new Thickness(margin),
    };
    private ScrollViewer MakeScrolleble(Control control)=>new ScrollViewer{
        VerticalScrollBarVisibility=Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
        Content=control,
    };
    private UserControl NoLoginPage(){
        return MakePage(MakeTextBlock("Для створення замовлень потрібен користувач", fontSize: 22), false);
    }
    private StackPanel MakeStack(int spacing=0, int margin=0, string posX="Center", string posY="Center")=>new StackPanel{
        Spacing=spacing,
        Margin=new Thickness(margin),
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
    };
    private Grid MakeGrid(string posX="Stretch", string posY="Stretch")=>new Grid{
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
    };
    private RadioButton MakeRadioButton(string text, string group, int margin=0, string posX="Center", string posY="Center", int fontSize=18, string color="White", bool check=false)=>new RadioButton{
        Content=text,
        GroupName=group,
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
        Margin=new Thickness(margin),
        FontSize=fontSize,
        Foreground=Brush.Parse(color),
        IsChecked=check,
    };
    private TextBox MakeTextBox(string waterMark, int sizeX=256, int maxSizeX=52, string wrap="Wrap", string posX="Center", int padding=8, bool returning=true)=>new TextBox{
        PlaceholderText=waterMark,
        Width=sizeX,
        MaxWidth=maxSizeX,
        MaxLength=300,
        Padding=new Thickness(padding),
        TextWrapping=Enum.Parse<TextWrapping>(wrap),
        AcceptsReturn=returning,
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
    };
    private CheckBox MakeCheckBox(int sizeX=22, int sizeY=18, string posY="Center", string posX="Center", int margin=0, int padding=0, bool visible=false, bool enable=true)=>new CheckBox{
        Width=sizeX,
        Height=sizeY,
        IsChecked=false,
        IsVisible=visible,
        IsEnabled=enable,
        CornerRadius=new CornerRadius(8),
        Background=new SolidColorBrush(Color.FromRgb(80, 80, 80)),
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
        HorizontalContentAlignment=HorizontalAlignment.Center,
        VerticalContentAlignment=VerticalAlignment.Center,
        Padding=new Thickness(padding),
        Margin=new Thickness(margin),
    };
    private Border MakeBorder(Control child, int sizeX=480, int sizeY=64, string posX="Center", string posY="Center", int margin=5, int padding=10)=>new Border{
        Width=sizeX,
        Height=sizeY,
        
        CornerRadius=new CornerRadius(12),
        Background=new SolidColorBrush(Color.FromRgb(42, 42, 42)),
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
        Padding=new Thickness(padding),
        Margin=new Thickness(margin),
        Child=child,
    };
    private Slider MakeSlider(int sizeX=50, int sizeY=20, int minV=1, int maxV=1000, string posX="Center", string posY="Center", bool enable=false, int margin=0)=>new Slider{
        Width=sizeX,
        Height=sizeY,
        Value=minV,
        Minimum=minV,
        Maximum=maxV,
        IsEnabled=enable,
        Margin=new Thickness(margin),
        SmallChange=1,
        LargeChange=5,
        TickFrequency=1,
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
    };
    private Control MakeDatePicker(DateTime now, int sizeX=128, int sizeY=64, string posX="Center", string posY="Center", int margin=0, bool visible=true)=>new DatePicker{
        Width=sizeX,
        Height=sizeY,
        SelectedDate=now,
        MinYear=new DateTimeOffset(now.AddDays(2)),
        MaxYear=new DateTimeOffset(now.AddDays(2).AddYears(3)),
        YearFormat="yyyy",
        MonthFormat="MMMM",
        DayFormat="ddd dd",
        YearVisible=true,
        MonthVisible=true,
        DayVisible=true,
        IsVisible=visible,
        Margin=new Thickness(margin),
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
    };
    private Control MakeTimePicker(DateTime now, int sizeX=128, int sizeY=64, string posX="Center", string posY="Center", int margin=0, bool visible=true, int hour=3)=>new TimePicker{
        Width=sizeX,
        Height=sizeY,
        SelectedTime=new TimeSpan(now.AddHours(hour).Hour, now.Minute, 0),
        ClockIdentifier="24HourClock",
        IsVisible=visible,
        Margin=new Thickness(margin),
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
    };
    private UserControl ChangeMethod(Order order){
        var stack=MakeStack(5, 10);
        var methods=new[]{"Готівка", "Картка", "Онлайн",};
        var radioBtns=new List<RadioButton>();
        var error=MakeTextBlock("", margin: 6, fontSize: 14, color: "DarkRed", visible: false);
        var payment=this._paymentServ.GetReceipt(order.Id);
        var changeBtn=MakeButton("Змінити", margin: 3);
        var methodStack=MakeStack(5, 5);
        stack.Children.Add(MakeTextBlock("Змінити метод оплати", margin: 6, fontSize: 30));
        foreach(var m in methods){
            var radioBtn=MakeRadioButton(m, "PaymentMethod", 8, posX: "Left", check: m=="Готівка" ? true : false);
            radioBtns.Add(radioBtn);
            methodStack.Children.Add(radioBtn);
        }
        stack.Children.Add(methodStack);
        stack.Children.Add(error);
        stack.Children.Add(changeBtn);
        changeBtn.Click+=async(s, e)=>{
            var newMethod=radioBtns.FirstOrDefault(rb=>rb.IsChecked==true)!.Content!.ToString();
            if(payment!.PaymentMethod==newMethod){this._successNavigate(OrdersDetails()); return;}
            var (success, message)=this._paymentServ.ChangeMethod(order.Id, newMethod!);
            if(!success){error.Text=message; error.IsVisible=true; return;}
            this._successNavigate(MakePage(MakeTextBlock(message, fontSize: 24)));
            await Task.Delay(1000);
            this._successNavigate(OrdersDetails());
        };
        return MakePage(stack, false);
    }
    private UserControl PayOrder(Order order){
        var stack=MakeStack(5, 10);
        var error=MakeTextBlock("", margin: 6, fontSize: 14, color: "DarkRed", visible: false);
        var changeMethod=MakeButton("Змінити метод", margin: 3);
        var payBtn=MakeButton("Оплатити", margin: 3);
        var payment=this._paymentServ.GetReceipt(order.Id);
        changeMethod.Click+=(s, e)=>this._navigateTo(ChangeMethod(order));
        payBtn.Click+=async(s, e)=>{
            var (success, message)=this._paymentServ.Pay(order.Id);
            if(!success){error.Text=message; error.IsVisible=true; return;}
            this._successNavigate(MakePage(MakeTextBlock(message, fontSize: 24), false));
            await Task.Delay(1000);
            LoadData();
            this._successNavigate(OrdersDetails());
        };
        stack.Children.Add(MakeTextBlock("Оплата", margin: 5, fontSize: 30));
        stack.Children.Add(MakeTextBlock($"Замовлення №{order.Id}", fontSize: 16));
        stack.Children.Add(MakeTextBlock($"Тип замовлення: {order.Type}", fontSize: 16));
        stack.Children.Add(MakeTextBlock($"Метод оплати: {payment!.PaymentMethod}", fontSize: 16));
        stack.Children.Add(MakeTextBlock($"До сплати: {order.TotalPrice}", fontSize: 16));
        stack.Children.Add(error);
        stack.Children.Add(changeMethod);
        stack.Children.Add(payBtn);
        return MakePage(stack, false);
    }
    private Control MakeCardForEditDish(OrderDish orderDish, bool edit, int fontSize=11){
        var card=MakeGrid();
        card.ColumnDefinitions=new ColumnDefinitions("4*, 2*, 1*, 1*, 2*, 1*");
        var dishName=MakeTextBlock(orderDish.Dish.Name, fontSize: fontSize, posX: "Left");
        var dishCat=MakeTextBlock(orderDish.Dish.Category.Name, fontSize: fontSize);
        var dishPrice=MakeTextBlock($"{orderDish.Dish.Price}₴", fontSize: fontSize);
        var dishCount=MakeTextBlock($"x{orderDish.Quantity}", fontSize: fontSize);
        var dishTotalPrice=MakeTextBlock($"{orderDish.Dish.Price*orderDish.Quantity}₴", fontSize: fontSize);
        var cancelBtn=MakeButton("✕", 26, 26, fontSize: fontSize, bg: 80, posX: "Right");
        cancelBtn.IsVisible=edit;
        Grid.SetColumn(dishName, 0);
        Grid.SetColumn(dishCat, 1);
        Grid.SetColumn(dishPrice, 2);
        Grid.SetColumn(dishCount, 3);
        Grid.SetColumn(dishTotalPrice, 4);
        Grid.SetColumn(cancelBtn, 5);
        card.Children.Add(dishName);
        card.Children.Add(dishCat);
        card.Children.Add(dishPrice);
        card.Children.Add(dishCount);
        card.Children.Add(dishTotalPrice);
        card.Children.Add(cancelBtn);
        var cardBorder=MakeBorder(card,  sizeX: 620, sizeY: 38, posX: "Stretch", "Stretch", padding: 8, margin: 4);
        cancelBtn.Click+=(s, e)=>{
            cardBorder.IsVisible=false; 
            this._orderServ.RemoveDish(new InputOrderDish{IdOrder=orderDish.IdOrder, IdDish=orderDish.IdDish});
            var order=this._orderRepo.Get(orderDish.IdOrder);
            order!.Personnel=this._personnelRepo.Get(order.IdPersonnel)!;
            LoadData();
            LoadData(orderDish.IdOrder);
            this._successNavigate(ShowDetails(order!, edit));
        };
        return cardBorder;
    }
    private UserControl ShowDetails(Order order, bool editDiseh=false){
        var grid=MakeGrid();
        var stack=MakeStack(6, 18, "Left", "Stretch");
        var stackBtn=MakeStack(2, 5, "Right", "Bottom");
        stackBtn.IsVisible=editDiseh;
        var card=MakeStack(4, 0, "Left", "Stretch");
        card.Children.Add(MakeTextBlock($"Замовленя №{order.Id}", fontSize: 30, posX: "Left", posY: "Top"));
        card.Children.Add(MakeTextBlock($"Клієнт:  {((App)Application.Current!).customer!.FullName}", fontSize: 14, posX: "Left", margin: 1));
        card.Children.Add(MakeTextBlock($"Офіціант:  {order.Personnel.FullName}", fontSize: 14, posX: "Left", margin: 1));
        card.Children.Add(MakeTextBlock($"Тип:  {order.Type}", fontSize: 14, posX: "Left", margin: 1));
        card.Children.Add(MakeTextBlock($"Статус:  {order.Status}", fontSize: 14, posX: "Left", margin: 1));
        card.Children.Add(MakeTextBlock($"Сума:  {order.TotalPrice}₴", fontSize: 14, posX: "Left"));
        card.Children.Add(MakeTextBlock($"Дата:  {order.StartTime.ToString("HH:mm dd.MM.yyyy")} - {order.EndTime.ToString("HH:mm dd.MM.yyyy")}", fontSize: 14, posX: "Left", margin: 1));
        card.Children.Add(MakeTextBlock("Страви:", fontSize: 14, posX: "Left", posY: "Bottom", margin: 1));
        stack.Children.Add(MakeBorder(card, 620, 220, "Left", "Stretch", 5, 8));
        if(!((App)Application.Current!).orderDishes.Any()) LoadData(order.Id);
        if(((App)Application.Current!).orderDishes.Any()){
            var stackDishesEdit=MakeStack(3, 0, "Left");
            foreach(var orderDish in ((App)Application.Current!).orderDishes) stackDishesEdit.Children.Add(MakeCardForEditDish(orderDish, editDiseh));
            stack.Children.Add(stackDishesEdit);
        }
        else stack.Children.Add(MakeTextBlock("Страв поки нема", fontSize: 26));
        var commentStack=MakeStack(6, 2);
        commentStack.Children.Add(MakeTextBlock("Коментар:   "+(string.IsNullOrWhiteSpace(order.Comment) ? "Коментаря нема" : ""), fontSize: 14, posX: "Left", posY: "Top", margin: 1));
        var comment=MakeTextBlock(order.Comment, fontSize: 16, posY: "Top");
        comment.TextWrapping=TextWrapping.Wrap;
        commentStack.Children.Add(comment);
        stack.Children.Add(MakeBorder(commentStack, 620, string.IsNullOrWhiteSpace(order.Comment) ? 40 : 200));
        grid.Children.Add(MakeScrolleble(stack));
        var payBtn=MakeButton("Оплатити", sizeX: 200, sizeY: 42, margin: 1);
        var addDishesBtn=MakeButton("Додати стриви", sizeX: 200, sizeY: 42, margin: 1);
        var cancelOrderBtn=MakeButton("Скасувати", sizeX: 200, sizeY: 42, margin: 1);
        stackBtn.Children.Add(payBtn);
        stackBtn.Children.Add(addDishesBtn);
        stackBtn.Children.Add(cancelOrderBtn);
        payBtn.Click+=(s, e)=>this._navigateTo(PayOrder(order));
        addDishesBtn.Click+=(s, e)=>this._navigateTo(AddDishesToOrder(order));
        cancelOrderBtn.Click+=async(s, e)=>{
            var (success, message)=this._orderServ.CancelOrder(order.Id, ((App)Application.Current!).customer!.Id);
            this._successNavigate(MakePage(MakeTextBlock(message, fontSize: 24, color: success ? "White" : "DarkRed")));
            await Task.Delay(700);
            LoadData();
            this._successNavigate(OrdersDetails());
        };
        grid.Children.Add(stackBtn);
        return MakePage(grid, false, "Stretch", "Stretch");
    }
    private Control MakeOrderCard(Order order){
        var card=MakeGrid();
        card.ColumnDefinitions=new ColumnDefinitions("3*, 2*, 1*");
        var name=MakeTextBlock($"Замовлення №{order.Id}", posX: "Left", fontSize: 16, margin: 1); 
        var timeInterval=MakeTextBlock($"{order.StartTime.ToString("HH:mm dd.MM.yyyy")} - {order.EndTime.ToString("HH:mm dd.MM.yyyy")}", fontSize: 12, margin: 1);
        var typeAndStatus=MakeTextBlock($"{order.Type}    {order.Status}", fontSize: 12, posX: "Left", margin: 1);
        var totalOrder=MakeTextBlock($"{order.TotalPrice.ToString()}₴", fontSize: 18, margin: 6);
        var detailBtn=MakeButton("Деталі", sizeX: 86, sizeY: 36, posX: "Right", margin: 1, bg: 80);
        var payBtn=MakeButton("Оплати", sizeX: 86, sizeY: 36, posX: "Right", margin: 1, bg: 80);
        var detailsStack=MakeStack(3, posX: "Left");
        var stackBtn=MakeStack(posX: "Right");
        stackBtn.Children.Add(detailBtn);
        stackBtn.Children.Add(payBtn);
        payBtn.IsVisible=order.Status=="Нове";
        detailsStack.Children.Add(name);
        detailsStack.Children.Add(typeAndStatus);
        detailsStack.Children.Add(timeInterval);
        Grid.SetColumn(detailsStack, 0);
        Grid.SetColumn(totalOrder, 1);
        Grid.SetColumn(stackBtn, 2);
        card.Children.Add(detailsStack);
        card.Children.Add(totalOrder);
        card.Children.Add(stackBtn);
        detailBtn.Click+=(s, e)=>this._navigateTo(ShowDetails(order, order.Status=="Нове"));
        payBtn.Click+=(s, e)=>this._navigateTo(PayOrder(order));
        return MakeBorder(card, 550, 86, margin: 2, padding: 6);
    }
    private UserControl CreateOrder(){
        var nowTime=DateTime.Now;
        var todayDate=DateTime.Today;
        var stack=MakeStack(6, 10);
        var orderType=MakeStack(spacing: 4, margin: 4);
        var order=MakeRadioButton("Замовлення", "OrderType", check: true);
        var booking=MakeRadioButton("Бронювання", "OrderType");
        orderType.Children.Add(order);
        orderType.Children.Add(booking);
        var time=MakeStack(spacing: 4, margin: 4);
        var now=MakeRadioButton("Зараз", "time", check: true);
        var later=MakeRadioButton("Пізніше", "time");
        time.Children.Add(now);
        time.Children.Add(later);
        var startTime=MakeTimePicker(nowTime, hour: 0, visible: false, sizeX: 100, sizeY: 28);
        var endTime=MakeTimePicker(nowTime, sizeX: 100, sizeY: 28);
        var dateText=MakeTextBlock("Дата: ", fontSize: 16, visible: false);
        var date=MakeDatePicker(todayDate, visible: false, sizeX: 100, sizeY: 28);
        var commentBlock=MakeGrid("Center", "Center");
        commentBlock.ColumnDefinitions=new ColumnDefinitions("2*, Auto");
        var commentCB=MakeCheckBox(visible: true, posX: "Left", margin: 6);
        var comment=MakeTextBox("Коментар", 400, 400);
        Grid.SetColumn(commentCB, 0);
        Grid.SetColumn(comment, 1);
        commentBlock.Children.Add(commentCB);
        commentBlock.Children.Add(comment);
        commentCB.IsChecked=false;
        comment.IsEnabled=false;
        var error=MakeTextBlock("", color: "DarkRed", margin: 4, fontSize: 16, visible: false);
        var confirm=MakeButton("Обрати", margin: 3, sizeX: 200, sizeY: 40);
        var startText=MakeTextBlock("Початок: "+((TimePicker)startTime).SelectedTime, fontSize: 16);
        stack.Children.Add(MakeTextBlock("Створення замовлення", fontSize: 26));
        stack.Children.Add(orderType);
        stack.Children.Add(time);
        stack.Children.Add(startText);
        stack.Children.Add(startTime);
        stack.Children.Add(MakeTextBlock("Кінець: ", fontSize: 16));
        stack.Children.Add(endTime);
        stack.Children.Add(dateText);
        stack.Children.Add(date);
        stack.Children.Add(commentBlock);
        stack.Children.Add(error);
        stack.Children.Add(confirm);
        commentCB.IsCheckedChanged+=(s, e)=>comment.IsEnabled=commentCB.IsChecked==true;
        now.IsCheckedChanged+=(s, e)=>{
            ((TimePicker)startTime).SelectedTime=new TimeSpan(nowTime.Hour, nowTime.Minute, 0);
            ((TimePicker)endTime).SelectedTime=new TimeSpan(nowTime.AddHours(3).Hour, nowTime.Minute, 0);
        };
        later.IsCheckedChanged+=(s, e)=>{
            startTime.IsVisible=later.IsChecked==true;
            startText.Text=later.IsChecked==true ? "Початок: " : "Початок: "+((TimePicker)startTime).SelectedTime;
            ((TimePicker)startTime).SelectedTime=new TimeSpan(nowTime.Hour, nowTime.Minute, 0);
            ((TimePicker)endTime).SelectedTime=new TimeSpan(nowTime.AddHours(3).Hour, nowTime.Minute, 0);
        };
        booking.IsCheckedChanged+=(s, e)=>{
            ((DatePicker)date).SelectedDate=todayDate.AddDays(2);
            now.IsEnabled=booking.IsChecked!=true;
            now.IsChecked=booking.IsChecked!=true && later.IsChecked!=true;
            later.IsChecked=booking.IsChecked==true || now.IsChecked!=true;
            dateText.IsVisible=booking.IsChecked==true;
            date.IsVisible=booking.IsChecked==true;
            startTime.IsVisible=booking.IsChecked==true || later.IsChecked==true;    
        };
        confirm.Click+=async(s, e)=>{
            var input=new InputOrder();
            DateTime StartTime;
            DateTime EndTime;
            var val=new DateTimeValidator();
            if(order.IsChecked==true){
                StartTime=todayDate.Add(((TimePicker)startTime).SelectedTime!.Value);
                EndTime=todayDate.Add(((TimePicker)endTime).SelectedTime!.Value);
                input.Type="Замовлення";
            }
            else{
                StartTime=(((DatePicker)date).SelectedDate!.Value.DateTime).Add(((TimePicker)startTime).SelectedTime!.Value);
                EndTime=(((DatePicker)date).SelectedDate!.Value.DateTime).Add(((TimePicker)endTime).SelectedTime!.Value);
                input.Type="Бронювання";
            }
            if(EndTime.Hour-StartTime.Hour>3){error.Text="Час перебування перевищює норму"; error.IsVisible=true; return;}
            if(!val.CheckDateTime(StartTime, EndTime)){error.Text="Некоректний час перебування"; error.IsVisible=true; return;}
            input.StartTime=StartTime;
            input.EndTime=EndTime;
            input.Comment=comment.Text??"";
            input.IdCustomer=((App)Application.Current!).customer!.Id;
            var (ok, newOrder, msg)=this._orderServ.CreateOrder(input);
            if(!ok){error.Text=msg; error.IsVisible=true; return;}
            this._successNavigate(MakePage(MakeTextBlock("Замовлення успішно створено")));
            await Task.Delay(1000);
            LoadData();
            LoadData(newOrder!.Id);
            this._successNavigate(ChooseTables(newOrder!, input));
        };
        return MakePage(stack, posX: "Stretch", posY: "Stretch");
    }
    private Control MakeTableCard(CheckBox checkBox, int number, string status){
        var grid=MakeGrid();
        grid.ColumnDefinitions=new ColumnDefinitions("2*, Auto");
        var tableNum=MakeTextBlock($"Стіл №{number}", margin: 6);
        Grid.SetColumn(checkBox, 0);
        Grid.SetColumn(tableNum, 1);
        grid.Children.Add(checkBox);
        grid.Children.Add(tableNum);
        checkBox.IsVisible=status=="Вільний";
        return MakeBorder(grid, sizeX: 300);
    }
    private UserControl ChooseTables(Order order, InputOrder input){
        var stack=MakeStack(6, 8);
        var tables=this._tableServ.GetAll().OrderBy(t=>t.Id);
        var freeTables=this._tableServ.GetFreeForTime(input.StartTime, input.EndTime);
        var tableBtns=new Dictionary<CheckBox, (int id, string status)>();
        var error=MakeTextBlock("", fontSize: 18, visible: false, color: "DarkRed");
        var confirmBtn=MakeButton("Обрати", sizeX: 240, sizeY: 40);
        foreach(var table in tables){
            var newStatus=freeTables.Any(ft=>ft.Id==table.Id);
            table.Status=newStatus ? "Вільний" : "Зайнятий";
            tableBtns.Add(MakeCheckBox(posX: "Left", margin: 6, visible: true, enable: table.Status=="Вільний"), (table.Id, table.Status));
        }
        stack.Children.Add(MakeTextBlock("Оберіть столи", fontSize: 30));
        foreach(var tb in tableBtns) stack.Children.Add(MakeTableCard(tb.Key, tb.Value.id, tb.Value.status));
        stack.Children.Add(error);
        stack.Children.Add(confirmBtn);
        confirmBtn.Click+=async(s, e)=>{
            input.IdTables=tableBtns.Where(o=>o.Key.IsChecked==true).Select(o=>o.Value.id).ToList();
            var (success, message)=this._orderServ.AddTablesToOrder(order.Id, input);
            if(!success){error.Text=message; error.IsVisible=true; return;}
            this._successNavigate(MakePage(MakeTextBlock("Столи успішно додано")));
            await Task.Delay(1000);
            this._successNavigate(AddDishesToOrder(order!));
        };
        return MakePage(stack);
    }
    private Control MakeDishCard(CheckBox checkBox, Dish dish, int count, Slider slider){
        var grid=MakeGrid();
        grid.ColumnDefinitions=new ColumnDefinitions("Auto, 3*, 2*, Auto, 2*");
        var name=MakeTextBlock(dish.Name, margin: 2, fontSize: 12);
        var price=MakeTextBlock(dish.Price.ToString()+"₴", margin: 2, fontSize: 12);
        var quantity=MakeTextBlock(count.ToString(), margin: 2, fontSize: 12);
        Grid.SetColumn(checkBox, 0);
        Grid.SetColumn(name, 1);
        Grid.SetColumn(price, 2);
        Grid.SetColumn(quantity, 3);
        Grid.SetColumn(slider, 4);
        grid.Children.Add(checkBox);
        grid.Children.Add(name);
        grid.Children.Add(price);
        grid.Children.Add(quantity);
        grid.Children.Add(slider);
        checkBox.IsCheckedChanged+=(s, e)=>slider.IsEnabled=checkBox.IsChecked==true;
        slider.ValueChanged+=(s, e)=>quantity.Text=((int)slider.Value).ToString();
        return MakeBorder(grid, sizeX: 700, sizeY: 52);
    }
    private UserControl AddDishesToOrder(Order order){
        var stack=MakeStack(0, 10);
        var addDishCard=new Dictionary<CheckBox, (Dish dish, int count, CheckBox checkBox, Slider slider)>();
        var dishes=this._dishRepo.GetAll();
        var error=MakeTextBlock("", color: "DarkRed", fontSize: 16, visible: false);
        var laterBtn=MakeButton("Пізніше", 200, 40);
        var confirmBtn=MakeButton("Обрати", 200, 40);
        stack.Children.Add(MakeTextBlock("Оберіть страви"));
        foreach(var dish in dishes) addDishCard.Add(MakeCheckBox(visible: true, posX: "Left", margin: 4), (dish, 1, MakeCheckBox(visible: false), MakeSlider(sizeX: 128, sizeY: 36, posX: "Right", posY: "Bottom", margin: 6)));
        if(((App)Application.Current!).orderDishes.Any()){
            foreach(var orderDish in ((App)Application.Current!).orderDishes){
                foreach(var adc in addDishCard){
                    if(orderDish.IdDish==adc.Value.dish.Id){
                        adc.Key.IsChecked=true;
                        adc.Value.checkBox.IsVisible=true;
                        adc.Value.slider.Value=orderDish.Quantity;
                        adc.Value.slider.IsEnabled=true;
                    }
                }
            }
        }
        foreach(var adc in addDishCard) stack.Children.Add(MakeDishCard(adc.Key, adc.Value.dish, adc.Value.count, adc.Value.slider));
        stack.Children.Add(error);
        stack.Children.Add(laterBtn);
        stack.Children.Add(confirmBtn);
        laterBtn.Click+=(s, e)=>{
            LoadData();
            LoadData(order.Id);
            this._successNavigate(OrdersDetails());
        };
        confirmBtn.Click+=async(s, e)=>{
            var OkMsg=new List<(bool ok, string msg)>();
            var input=new InputOrderDish();
            if(((App)Application.Current!).orderDishes.Any()){
                var dishesToUpdate=addDishCard.Where(a=>a.Value.checkBox.IsVisible==true).Select(a=>(a.Value.dish.Id, (int)a.Value.slider.Value)).ToList();
                var dishesToAdd=addDishCard.Where(a=>a.Value.checkBox.IsVisible!=true && a.Key.IsChecked==true).Select(a=>(a.Value.dish.Id, (int)a.Value.slider.Value));
                foreach(var dtu in dishesToUpdate){
                    input.Quantity=dtu.Item2>1 ? dtu.Item2 : 1;
                    input.IdDish=dtu.Id;
                    input.IdOrder=order.Id;
                    OkMsg.Add(this._orderServ.UpdateQuantity(input));
                }
                foreach(var dta in dishesToAdd){
                    input.Quantity=dta.Item2>1 ? dta.Item2 : 1;
                    input.IdDish=dta.Id; 
                    input.IdOrder=order.Id;
                    OkMsg.Add(this._orderServ.AddDish(input));
                }
            }
            else{
                var addDishes=addDishCard.Where(a=>a.Key.IsChecked==true).Select(a=>(a.Value.dish.Id, (int)a.Value.slider.Value)).ToList();
                foreach(var dish in addDishes){
                    input.Quantity=dish.Item2>1 ? dish.Item2 : 1;
                    input.IdDish=dish.Id; 
                    input.IdOrder=order.Id;
                    OkMsg.Add(this._orderServ.AddDish(input));
                }
            }
            if(OkMsg.Any(o=>o.ok==false)){error.Text="Помилка додавання страв"; error.IsVisible=true; return;}
            this._successNavigate(MakePage(MakeTextBlock("Страви успішно додані до замовлення", fontSize: 26)));
            await Task.Delay(1000);
            LoadData();
            LoadData(order.Id);
            this._successNavigate(OrdersDetails());
        };
        return MakePage(stack);
    }
    private UserControl OrdersDetails(){
        var grid=MakeGrid();
        var orderDetails=MakeStack(8, 26, "Left", "Stretch");
        var createBtn=MakeButton("Нове замовлення", posX: "Right", posY: "Bottom", sizeX: 200, sizeY: 42, margin: 6);
        if(!((App)Application.Current!).orders.Any()) LoadData();
        if(!((App)Application.Current!).orders.Any()) grid.Children.Add(MakeTextBlock("Замовлень поки немає", margin: 12));
        else{
            ((App)Application.Current!).orders=((App)Application.Current!).orders.OrderByDescending(o=>o.StartTime).ToList();
            if(((App)Application.Current!).orders.Any(o=>o.Status=="Нове")){
                orderDetails.Children.Add(MakeTextBlock("Активне замовлення", "Top", "Left", 4, 16));
                orderDetails.Children.Add(MakeOrderCard(((App)Application.Current!).orders.FirstOrDefault(o=>o.Status=="Нове")!));
                createBtn.IsVisible=false;   
            }
            orderDetails.Children.Add(MakeTextBlock("Історія замовлень", "Top", "Left", 4, 16));
            foreach(var order in ((App)Application.Current!).orders){
                if(order.Status=="Нове") continue;
                orderDetails.Children.Add(MakeOrderCard(order));
            }

        }
        createBtn.Click+=(s, e)=>this._navigateTo(CreateOrder());
        grid.Children.Add(MakeScrolleble(orderDetails));
        grid.Children.Add(createBtn);
        return MakePage(grid, false, "Stretch", "Stretch");
    }
    public OrderSettings(OrderRepository orderRepo, OrderService orderServ, PaymentService paymentServ, TableService tableServ, DishRepository dishRepo, PersonnelRepository personnelRepo, Action<UserControl> navigateTo, Action<UserControl> successNavigate){
        this._orderRepo=orderRepo;
        this._orderServ=orderServ;
        this._paymentServ=paymentServ;
        this._tableServ=tableServ;
        this._dishRepo=dishRepo;
        this._navigateTo=navigateTo;
        this._successNavigate=successNavigate;
        this._personnelRepo=personnelRepo;
        UserControl page;
        if(((App)Application.Current!).customer==null) page=NoLoginPage();
        else page=OrdersDetails();
        Content=page;
    }
}