using Avalonia;
using Avalonia.Controls;
using Repositories;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Linq;
using Models;
using System.Threading.Tasks;

namespace Restoraunt.Views;
public class MenuShowDishes : UserControl{
    private readonly MenuRepository _menuRepo;
    private StackPanel _dishesContainer;
    private void LoadData(){
        var menu=this._menuRepo.GetFirst();
        ((App)Application.Current!).menu=this._menuRepo.Get(menu!.Id);
    }
    private TextBlock MakeTextBlock(string text, string posY="Center", string posX="Center", int margin=0, int fontSize=22, string color="White")=>new TextBlock{
        Text=text,
        Foreground=Brush.Parse(color),
        FontSize=fontSize,
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
        VerticalAlignment=Enum.Parse<VerticalAlignment>(posY),
        Margin=new Thickness(margin),
    };
    private Control CreateDishCard(Dish dish){
        var card=new Grid{
            ColumnDefinitions=new ColumnDefinitions("3*, 1*, 1*"),
            VerticalAlignment=VerticalAlignment.Center,
        };
        var name=MakeTextBlock(dish.Name, posX: "Left", fontSize: 16);
        var category=MakeTextBlock(dish.Category.Name, fontSize: 16);
        var price=MakeTextBlock(dish.Price.ToString()+"₴", posX: "Right", fontSize: 16);
        Grid.SetColumn(name, 0);
        Grid.SetColumn(category, 1);
        Grid.SetColumn(price, 2);
        card.Children.Add(name);
        card.Children.Add(category);
        card.Children.Add(price);
        return new Border{
            Background=new SolidColorBrush(Color.FromRgb(24, 24, 24)),
            CornerRadius=new CornerRadius(14),
            Height=48,
            Margin=new Thickness(10, 6),
            Padding=new Thickness(14, 6),
            Width=660,
            HorizontalAlignment=HorizontalAlignment.Stretch,
            Child=card,
        };
    }
    private async void BuildUI(){
        this._dishesContainer.Children.Add(MakeTextBlock("Завантаження даних..."));
        await Task.Delay(700);
        try{
            if(((App)Application.Current!).menu==null) LoadData();
            if(((App)Application.Current!).menu==null){
                this._dishesContainer.Children.Add(MakeTextBlock("Меню не знайдено", color: "DarkRed"));
                return;
            }
            if(!((App)Application.Current!).menu!.Dishes.Any()){
                this._dishesContainer.Children.Clear();
                this._dishesContainer.HorizontalAlignment=HorizontalAlignment.Center;
                this._dishesContainer.VerticalAlignment=VerticalAlignment.Center;
                this._dishesContainer.Children.Add(MakeTextBlock("Страви не знайдено", color: "DarkRed"));
                return;
            } 
            this._dishesContainer.Children.Clear();
            this._dishesContainer.HorizontalAlignment=HorizontalAlignment.Center;
            this._dishesContainer.VerticalAlignment=VerticalAlignment.Top;
        }
        catch(Exception ex){
            this._dishesContainer.Children.Clear();
            this._dishesContainer.HorizontalAlignment=HorizontalAlignment.Center;
            this._dishesContainer.VerticalAlignment=VerticalAlignment.Top;
            this._dishesContainer.Children.Add(MakeTextBlock($"Помилка завантаження: {ex.Message}", color: "DarkRed"));
        }
        ShowDishes();
    }
    private void ShowDishes(){
        this._dishesContainer.Children.Add(MakeTextBlock($"{((App)Application.Current!).menu!.Name}", fontSize: 30, posX: "Center", posY: "Top"));
        foreach(var dish in ((App)Application.Current!).menu!.Dishes) this._dishesContainer.Children.Add(CreateDishCard(dish));
    }
    public MenuShowDishes(MenuRepository menuRepo){
        this._menuRepo=menuRepo;
        this._dishesContainer=new StackPanel{
            Spacing=2,
            Margin=new Thickness(10),
            HorizontalAlignment=HorizontalAlignment.Center,
            VerticalAlignment=VerticalAlignment.Center,
        };
        var scrollView=new ScrollViewer{
            VerticalScrollBarVisibility=Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            Content=this._dishesContainer,
        };
        if(((App)Application.Current!).menu==null) BuildUI();
        else ShowDishes();
        Content=scrollView;
    }
}