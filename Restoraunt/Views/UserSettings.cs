using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using Input;
using Services;
using System.Threading.Tasks;

namespace Restoraunt.Views;
public class UserSettings : UserControl{
    private readonly CustomerService _customerServ;
    private readonly Action<UserControl> _navigateTo;
    private readonly Action<UserControl> _successNavigate;
    private StackPanel MakeStack()=>new StackPanel{
        Spacing=12,
        Margin=new Thickness(18),
        HorizontalAlignment=HorizontalAlignment.Center,
        VerticalAlignment=VerticalAlignment.Center,
    };
    private TextBlock MakeTextBlock(string text="", string posX="Center", int fontSize=22, string color="White", bool visible=true)=>new TextBlock{
        Text=text,
        Foreground=Brush.Parse(color),
        FontSize=fontSize,
        IsVisible=visible,
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
    };
    private Button MakeBtn(string name)=>new Button{
        Content=name,
        Height=52, 
        Width=256,
        FontSize=18,
        Foreground=Brushes.White,
        Background=new SolidColorBrush(Color.FromRgb(42, 42, 42)),
        CornerRadius=new CornerRadius(9),
        HorizontalAlignment=HorizontalAlignment.Center,
        HorizontalContentAlignment=HorizontalAlignment.Center,
        VerticalContentAlignment=VerticalAlignment.Center,
    };
    private TextBox MakeTextBox(string waterMark, int sizeX=256, int maxSizeX=256, int maxLength=20, string wrap="NoWrap", string posX="Center", int padding=8, bool returning=false)=>new TextBox{
        PlaceholderText=waterMark,
        Width=sizeX,
        MaxWidth=maxSizeX,
        MaxLength=maxLength,
        Padding=new Thickness(padding),
        TextWrapping=Enum.Parse<TextWrapping>(wrap),
        AcceptsReturn=returning,
        HorizontalAlignment=Enum.Parse<HorizontalAlignment>(posX),
    };
    private UserControl MakePage(Control control){
        var page=new UserControl{
            HorizontalAlignment=HorizontalAlignment.Center,
            VerticalAlignment=VerticalAlignment.Center,
            Content=control,  
        };
        return page;
    }
    private UserControl BuildLoginPage(){
        var stack=MakeStack();
        var phone=MakeTextBox("Номер телефону");
        var error=MakeTextBlock(fontSize: 14, color: "DarkRed", visible: false);
        var confirm=MakeBtn("Увійти");
        confirm.Click+=(s, e)=>{
            var(success, customer, message)=this._customerServ.Login(phone.Text??"");
            if(!success){error.Text=message; error.IsVisible=true; return;}
            ((App)Application.Current!).customer=customer;
            this._successNavigate(BuildProfilePage($"Ласкаво просимо, {customer!.FullName}"));
        };
        stack.Children.Add(MakeTextBlock("Вхід"));
        stack.Children.Add(phone);
        stack.Children.Add(error);
        stack.Children.Add(confirm);
        return MakePage(stack);
    }
    private UserControl BuildCreatePage(){
        var error=MakeTextBlock(fontSize: 14, color: "DarkRed", visible: false);
        var stack=MakeStack();
        var fullName=MakeTextBox("ПІБ", maxLength: 100);
        var phone=MakeTextBox("Номер телефону");
        var confirm=MakeBtn("Зареєструватись");
        confirm.Click+=async(s, e)=>{
            var input=new InputCustomer{FullName=fullName.Text??"", Phone=phone.Text??""};
            var(success, customer, message)=this._customerServ.Register(input);
            if(!success){error.Text=message; error.IsVisible=true; return;}
            ((App)Application.Current!).customer=customer;
            this._successNavigate(BuildNotification(message));
            await Task.Delay(1500);
            this._successNavigate(BuildProfilePage($"Ласкаво просимо, {customer!.FullName}"));
        };
        stack.Children.Add(MakeTextBlock("Реєстрація"));
        stack.Children.Add(fullName);
        stack.Children.Add(phone);
        stack.Children.Add(error);
        stack.Children.Add(confirm);
        return MakePage(stack);
    }
    private UserControl BuildNotification(string msg){
        var stack=MakeStack();
        stack.Children.Add(MakeTextBlock(msg));
        return MakePage(stack);
    }
    private UserControl BuildProfilePage(string msg){
        var stack=MakeStack();
        stack.Children.Add(MakeTextBlock(msg));
        var logOutBtn=MakeBtn("Вийти");
        logOutBtn.Click+=async(s, e)=>{
            this._successNavigate(BuildNotification("Вихід з акаунта..."));
            await Task.Delay(700);
            ((App)Application.Current!).customer=null;
            this._successNavigate(BuildUI());
        };
        var deleteBtn=MakeBtn("Видалити");
        deleteBtn.Click+=async(s, e)=>{
            this._successNavigate(BuildNotification("Їде видалення акаунта..."));
            await Task.Delay(700);
            var (success, message)=this._customerServ.Remove(((App)Application.Current!).customer!.Phone);
            if(!success){this._successNavigate(BuildNotification(message)); return;}
            ((App)Application.Current).customer=null;
            this._successNavigate(BuildNotification(message));
            await Task.Delay(1200);
            this._successNavigate(BuildUI());
        };
        stack.Children.Add(logOutBtn);
        stack.Children.Add(deleteBtn);        
        return MakePage(stack);
    }
    private UserControl BuildUI(){
        var loginBtn=MakeBtn("Увійти");
        var createBtn=MakeBtn("Зареєструватись");
        var stack=MakeStack();
        loginBtn.Click+=(s, e)=>this._navigateTo(BuildLoginPage());
        createBtn.Click+=(s, e)=>this._navigateTo(BuildCreatePage());
        stack.Children.Add(loginBtn);
        stack.Children.Add(createBtn);
        return MakePage(stack);
    }
    public UserSettings(CustomerService customerServ, Action<UserControl> navigateTo, Action<UserControl> successNavigate){
        this._customerServ=customerServ;
        this._navigateTo=navigateTo;
        this._successNavigate=successNavigate;
        UserControl page;
        if(((App)Application.Current!).customer==null){page=BuildUI();}
        else page=BuildProfilePage($"Ласкаво просимо, {((App)Application.Current).customer!.FullName}");
        Content=page;
    }
}