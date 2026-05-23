namespace ZenithTime;

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _showSeconds = false;
    private readonly DispatcherTimer _clockTimer;

    public MainWindow()
    {
        InitializeComponent();
        RecenterWindow();
        
        // Initialize clock timer
        _clockTimer = new DispatcherTimer(DispatcherPriority.Render);
        _clockTimer.Interval = TimeSpan.FromMilliseconds(500);
        _clockTimer.Tick += ClockTimer_Tick;
        _clockTimer.Start();
        
        // Initial clock update
        UpdateClockDisplay();

        // Subscribe to display changes to keep the window centered
        Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        
        // Cleanup on close
        this.Closed += (s, e) => Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
    }

    private void ClockTimer_Tick(object? sender, EventArgs e)
    {
        UpdateClockDisplay();
    }

    private void UpdateClockDisplay()
    {
        TxtClock.Text = DateTime.Now.ToString(_showSeconds ? "HH:mm:ss" : "HH:mm");
    }

    private void TxtClock_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _showSeconds = !_showSeconds;
        UpdateClockDisplay();
    }

    private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(RecenterWindow);
    }

    private void RecenterWindow()
    {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        this.Left = (screenWidth - this.Width) / 2;
        this.Top = 0;
    }
}