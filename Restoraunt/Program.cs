using Avalonia;
using Dapper;
using System;

namespace Restoraunt;
sealed class Program{
    [STAThread]
    public static void Main(string[] args){
        DefaultTypeMap.MatchNamesWithUnderscores=true;
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    public static AppBuilder BuildAvaloniaApp()=>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
        #if DEBUG
            .WithDeveloperTools()
        #endif
            .WithInterFont()
            .LogToTrace();
}
