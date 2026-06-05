using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Repositories;
using Services;
using System;
using System.Collections.Generic;

namespace Restoraunt.Views;
public class MainWindow : Window{
    private  readonly OrderRepository _orderRepo;
    private readonly MenuRepository _menuRepo;
    private readonly CustomerService _customerServ;
    private readonly OrderService _orderServ;
    private readonly PaymentService _paymentServ;
    private readonly TableService _tableServ;
    private readonly DishRepository _dishRepo;
    private readonly PersonnelRepository _personnelRepo;
    private DockPanel _window=null!;
    private ContentControl _pageArea=null!;
    private Grid _contentArea=null!;
    private Button _backBtn=null!;
    private Button _cancelBtn=null!;
    private Button _homeBtn=null!;
    private Button _menuDishBtn=null!;
    private Button _orderBtn=null!;
    private Button _userBtn=null!;
    private readonly Stack<UserControl> _history=new Stack<UserControl>();
    private void NavigateTo(UserControl page){
        if(this._pageArea.Content is UserControl current){
            if(current.GetType()!=page.GetType()) this._history.Push(current);
        }
        this._pageArea.Content=page;
        UpdateButtons();
    }
    private void NavigateBack(){
        if(this._history.Count==0) return;
        this._pageArea.Content=this._history.Pop();
        UpdateButtons();
    }
    private void SuccessNavigate(UserControl page){
        this._history.Clear();
        this._pageArea.Content=page;
        UpdateButtons();
    }
    public void GoHome(){
        this._history.Clear();
        this._pageArea.Content=HomePage();
        UpdateButtons();
    }
    private void UpdateButtons(){
        this._backBtn.IsVisible=this._history.Count>1;
        this._cancelBtn.IsVisible=this._history.Count>0;
    }
    private Image LoadIcon(string fileName){
        var uri=new Uri($"avares://Restoraunt/Assets/{fileName}");
        var bitmap=new Bitmap(AssetLoader.Open(uri));
        return new Image{Source=bitmap, Width=32, Height=32,};
    }
    private StackPanel ContentBtn(string name, int fontSize, string? fileName=null){
        var content=new StackPanel{Spacing=2};
        if(!string.IsNullOrWhiteSpace(fileName)) content.Children.Add(LoadIcon(fileName));
        content.Children.Add(new TextBlock{Text=name, FontSize=fontSize,});
        return content;
    }
    private Button MakeButton(string name, bool isVisible=true, int sizeX=64, int sizeY=64, int fontSize=12, int spacing=0, string posX="Left", string posY="Top", int margin=0, string? fileName=null)=>new Button{
        Content=ContentBtn(name, fontSize, fileName),
        FontSize=fontSize,
        Width=sizeX,
        Height=sizeY,
        IsVisible=isVisible,
        Foreground=Brushes.White,
        Background=new SolidColorBrush(Color.FromRgb(42, 42, 42)),
        Margin=new Thickness(margin),
        LetterSpacing=spacing,
        CornerRadius=new CornerRadius(9),
        HorizontalContentAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalContentAlignment=Enum.Parse<VerticalAlignment>(posY),
    };
    private UserControl HomePage(){
        var panel=new StackPanel{
            HorizontalAlignment=HorizontalAlignment.Center,
            VerticalAlignment=VerticalAlignment.Center,
            Spacing=12,
        };
        panel.Children.Add(new TextBlock{
            Text="Ласкаво просимо",
            FontSize=28,
            Foreground=Brushes.White,
            HorizontalAlignment=HorizontalAlignment.Center,
        });
        var ctrl=new UserControl{Content=panel};
        return ctrl;
    }
    private void OnHomeClick(object? s, RoutedEventArgs e)=>GoHome();
    private void OnMenuClick(object? s, RoutedEventArgs e)=>NavigateTo(new MenuShowDishes(this._menuRepo));
    private void OnUserClick(object? s, RoutedEventArgs e)=>NavigateTo(new UserSettings(this._customerServ, NavigateTo, SuccessNavigate));
    private void OnBackClick(object? s, RoutedEventArgs e)=>NavigateBack();
    private void OnCancelClick(object? s, RoutedEventArgs e)=>GoHome();
    private void OnOrderClick(object? s, RoutedEventArgs e)=>NavigateTo(new OrderSettings(this._orderRepo, this._orderServ, this._paymentServ, this._tableServ, this._dishRepo, this._personnelRepo, NavigateTo, SuccessNavigate));
    private void BuildUI(){
        this._window=new DockPanel();
        var menu=new StackPanel{
            Width=80,
            Background=new SolidColorBrush(Color.FromRgb(24, 24, 24)),
            Spacing=3,
            Margin=new Thickness(0),
        };
        DockPanel.SetDock(menu, Dock.Left);
        this._homeBtn=MakeButton("Home", posY: "Bottom", posX: "Center", margin: 6, fileName: "Home.png");
        this._userBtn=MakeButton(" User", posY: "Bottom", posX: "Center", margin: 6, fileName: "User.png");
        this._menuDishBtn=MakeButton("Menu", posY: "Bottom", posX: "Center", margin: 6, fileName: "Menu.png");
        this._orderBtn=MakeButton("Order", posY: "Bottom", posX: "Center", margin: 6, fileName: "Order.png");
        this._cancelBtn=MakeButton("✕", isVisible: false, sizeX: 26, sizeY: 26, fontSize: 9, posX: "Center", posY: "Center", margin: 4);
        this._backBtn=MakeButton("←", isVisible: false, sizeX: 26, sizeY: 26, fontSize: 9, posX: "Center", posY: "Center", margin: 4);
        this._homeBtn.Click+=OnHomeClick;
        this._menuDishBtn.Click+=OnMenuClick;
        this._orderBtn.Click+=OnOrderClick;
        this._userBtn.Click+=OnUserClick;
        this._backBtn.Click+=OnBackClick;
        this._cancelBtn.Click+=OnCancelClick;
        this._backBtn.HorizontalAlignment=HorizontalAlignment.Left;
        this._backBtn.VerticalAlignment=VerticalAlignment.Top;
        this._cancelBtn.HorizontalAlignment=HorizontalAlignment.Right;
        this._cancelBtn.VerticalAlignment=VerticalAlignment.Top;
        this._pageArea=new ContentControl{
            HorizontalAlignment=HorizontalAlignment.Stretch,
            VerticalAlignment=VerticalAlignment.Stretch,
            Content=HomePage(),
        };
        this._contentArea=new Grid();
        this._contentArea.Children.Add(this._pageArea);
        this._contentArea.Children.Add(this._backBtn);
        this._contentArea.Children.Add(this._cancelBtn);
        menu.Children.Add(this._homeBtn);
        menu.Children.Add(this._menuDishBtn);
        menu.Children.Add(this._orderBtn);
        menu.Children.Add(this._userBtn);
        this._window.Children.Add(menu);
        this._window.Children.Add(this._contentArea);
        Content=this._window;
    }
    public MainWindow(OrderRepository orderRepo, MenuRepository menuRepo, CustomerService customerServ, OrderService orderServ, PaymentService paymentServ, TableService tableServ, DishRepository dishRepo, PersonnelRepository personnelRepo){
        this._orderRepo=orderRepo;
        this._menuRepo=menuRepo;
        this._customerServ=customerServ;
        this._orderServ=orderServ;
        this._paymentServ=paymentServ;
        this._tableServ=tableServ;
        this._dishRepo=dishRepo;
        this._personnelRepo=personnelRepo;
        this.Title="Ресторан";
        this.Width=900;
        this.Height=600;
        this.WindowStartupLocation=WindowStartupLocation.CenterScreen;
        this.HorizontalAlignment=HorizontalAlignment.Stretch;
        this.VerticalAlignment=VerticalAlignment.Stretch;
        BuildUI();
    }
}