using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace KeepMeOnline;

public partial class App : Application
{
    private Timer? _timer;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        //Prevent system from sleeping
        ImmortalMethods.PreventSleep();
        
        //Adjust the TimeSpan to change the interval initially 240 seconds
        _timer = new Timer(state => ImmortalMethods.SimulateMouseMovement(), null, TimeSpan.Zero, TimeSpan.FromSeconds(240));

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
        _timer.Dispose();
        ImmortalMethods.AllowSleep();
        Environment.Exit(0); //OS will exit app immediately no OnExit event will be called
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        // TODO add additional logic for cleanup 
        System.Console.WriteLine("..Cleaning.."); // this will not execute


    }

}