using App.Client.Controls.Settings;
using App.Infrastructure;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;

namespace App.Client.Controls.Main;

public sealed partial class MainWindow : Window
{
    private readonly Func<SettingsWindow> _settingsWindowFactory;

    public MainWindow(Func<SettingsWindow> settingsWindowFactory, IAppDbContext dbContext)
    {
        _settingsWindowFactory = settingsWindowFactory;
        InitializeComponent();
        Title = "WiX .NET Starter — Sample Data";

        var windowId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(this));
        var appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.Resize(new SizeInt32(960, 640));
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumWidth = 640;
            presenter.PreferredMinimumHeight = 480;
        }

        var customers = dbContext.Customers.ToList();
        CustomersList.ItemsSource = customers;
        CustomerCountText.Text = $"{customers.Count} customers";
    }

    private void settingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var mainWindowHandle = WindowNative.GetWindowHandle(this);
        var settingsWindow = _settingsWindowFactory();
        var settingsWindowHandle = WindowNative.GetWindowHandle(settingsWindow);

        NativeMethods.SetOwner(settingsWindowHandle, mainWindowHandle);
        NativeMethods.EnableWindow(mainWindowHandle, false);
        settingsWindow.Closed += (_, _) => NativeMethods.EnableWindow(mainWindowHandle, true);
        settingsWindow.Activate();
    }
}