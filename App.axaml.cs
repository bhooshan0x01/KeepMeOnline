using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace KeepMeOnline;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += OnExit;

        }
        base.OnFrameworkInitializationCompleted();
    }

    private void CloseApplication(object sender, EventArgs e)
    {
        //add logic to cleanup memory and backround process
        System.Console.WriteLine("Exiting..");
        Environment.Exit(0); //OS will exit app immediately no OnExit event will be called
        System.Console.WriteLine("..Exited.."); // this will not execute

    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        // TODO add additional logic for cleanup 
        System.Console.WriteLine("..Cleaning.."); // this will not execute


    }

}