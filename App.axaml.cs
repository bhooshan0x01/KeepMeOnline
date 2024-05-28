using System;
using System.IO;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace KeepMeOnline;

public partial class App : Application
{
    private Timer? _inactiveTimer;
    internal GlobalKeyboardHook? _keyboardHook;
    internal GlobalMouseHook? _mouseHook;
    private SessionLockHandler? _sessionLockHandler;
    private bool _isSessionLocked = false;
    private bool _isProgrammaticMovement;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Startup += OnStartup;
            desktop.Exit += OnExit;

        }
        base.OnFrameworkInitializationCompleted();
    }

    private void OnStartup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
    {
        _mouseHook = new GlobalMouseHook();
        _mouseHook.MouseMoved += OnUserActivity;

        _keyboardHook = new GlobalKeyboardHook();
        _keyboardHook.KeyPressed += OnUserActivity;

        // 4 minutes timer
        _inactiveTimer = new Timer(TimeSpan.FromSeconds(240));
        _inactiveTimer.Elapsed += OnInactivityTimerElapsed;

        _sessionLockHandler = new SessionLockHandler();
        _sessionLockHandler.SessionLocked += OnSessionLocked;
        _sessionLockHandler.SessionUnlocked += OnSessionUnlocked;
    }

    private void OnSessionLocked(object? sender, EventArgs e)
    {
        _isSessionLocked = true;
        _inactiveTimer?.Stop();
    }

    private void OnSessionUnlocked(object? sender, EventArgs e)
    {
        _isSessionLocked = false;
        ResetInactivityTimer();
    }

    private void OnUserActivity(object? sender, EventArgs e)
    {
        Logger.LogMessage($"User Activity..");

        if (!_isProgrammaticMovement && !_isSessionLocked)
        {
            ResetInactivityTimer();
        }
        else
        {
            Logger.LogMessage($"Disable mouse simulation.");
            _isProgrammaticMovement = false;
        }
    }

    private void ResetInactivityTimer()
    {
        Logger.LogMessage($"Resetting the timer... ");
        _inactiveTimer?.Stop();
        _inactiveTimer?.Start();
        Logger.LogMessage($"Timer resetted.");
    }

    private void OnInactivityTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        // Handle user inactivity 
        if (!_isSessionLocked)
        {
            MoveMouse();
        }
    }

    private void MoveMouse()
    {
        _isProgrammaticMovement = true;
        ImmortalMethods.SimulateMouseMovement();
    }

    private void CloseApplication(object? sender, EventArgs e)
    {        
        Logger.LogMessage($"Exiting the app.");
        _keyboardHook?.Dispose();
        _mouseHook?.Dispose();
        _inactiveTimer?.Dispose();
        ImmortalMethods.AllowSleep();
        _sessionLockHandler?.Dispose();
        Environment.Exit(0); //OS will exit app immediately no OnExit event will be called
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        // TODO add additional logic for cleanup 
        // this will not execute
        Logger.LogMessage($"App exited.");
    }

}