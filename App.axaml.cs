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
        
        try
        {
            AvaloniaXamlLoader.Load(this);
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in Initialize: {ex.Message}");
        }

    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Startup += OnStartup;
                desktop.Exit += OnExit;

            }
            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in OnFrameworkInitializationCompleted: {ex.Message}");
        }
    }

    private void OnStartup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
    {
        
        try
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
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in OnStartup: {ex.Message}");
        }
    }

    private void OnSessionLocked(object? sender, EventArgs e)
    {
        try
        {
            _isSessionLocked = true;
            _inactiveTimer?.Stop();
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in OnSessionLocked: {ex.Message}");
        }
    }

    private void OnSessionUnlocked(object? sender, EventArgs e)
    {
        try
        {
            _isSessionLocked = false;
            ResetInactivityTimer();
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in OnSessionUnlocked: {ex.Message}");
        }
    }

    private void OnUserActivity(object? sender, EventArgs e)
    {
        
        try
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
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in OnUserActivity: {ex.Message}");
        }
    }

    private void ResetInactivityTimer()
    {
        try
        {
            Logger.LogMessage($"Resetting the timer... ");
            _inactiveTimer?.Stop();
            _inactiveTimer?.Start();
            Logger.LogMessage($"Timer resetted.");
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in ResetInactivityTimer: {ex.Message}");
        }
    }

    private void OnInactivityTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        
        try
        {
            // Handle user inactivity 
            if (!_isSessionLocked)
            {
                MoveMouse();
            }
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in OnInactivityTimerElapsed: {ex.Message}");
        }
    }

    private void MoveMouse()
    {
        try
        {
            _isProgrammaticMovement = true;
            ImmortalMethods.SimulateMouseMovement();
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in MoveMouse: {ex.Message}");
        }
    }

    private void CloseApplication(object? sender, EventArgs e)
    {        
        
        try
        {
            Logger.LogMessage($"Exiting the app.");
            _keyboardHook?.Dispose();
            _mouseHook?.Dispose();
            _inactiveTimer?.Dispose();
            ImmortalMethods.AllowSleep();
            _sessionLockHandler?.Dispose();
            Environment.Exit(0); //OS will exit app immediately no OnExit event will be called
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in CloseApplication: {ex.Message}");
        }
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        try
        {
            // TODO add additional logic for cleanup 
            // this will not execute
            Logger.LogMessage($"App exited.");
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Error in OnExit: {ex.Message}");
        }
    }

}