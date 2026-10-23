using App.Updates.Feed;
using App.Updates.Settings;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;

namespace App.Client.Controls.Settings;

public sealed partial class SettingsWindow : Window
{
    private readonly IUpdateSettingsStore _updateSettings;

    public SettingsWindow(IUpdateSettingsStore updateSettings)
    {
        _updateSettings = updateSettings;
        InitializeComponent();
        Title = "Settings";

        var windowId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(this));
        var appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.Resize(new SizeInt32(420, 420));
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumWidth = 360;
            presenter.PreferredMinimumHeight = 360;
        }

        applyChannel(updateSettings.GetChannel());
    }

    private void applyChannel(UpdateChannel channel)
    {
        if (channel == UpdateChannel.Beta)
        {
            BetaRadioButton.IsChecked = true;
        }
        else
        {
            StableRadioButton.IsChecked = true;
        }
    }

    private void okButton_Click(object sender, RoutedEventArgs e)
    {
        _updateSettings.SetChannel(BetaRadioButton.IsChecked == true ? UpdateChannel.Beta : UpdateChannel.Stable);
        Close();
    }

    private void cancelButton_Click(object sender, RoutedEventArgs e) => Close();
}