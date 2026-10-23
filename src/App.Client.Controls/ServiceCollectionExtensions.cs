using App.Client.Controls.Main;
using App.Client.Controls.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace App.Client.Controls;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientControls(this IServiceCollection services)
    {
        services.addWindowFactory<SettingsWindow>();
        services.AddTransient<MainWindow>();
        return services;
    }

    /// <summary>Registers a window as transient, plus a Func&lt;TWindow&gt; factory so a longer-lived owner can create a fresh instance on demand.</summary>
    private static IServiceCollection addWindowFactory<TWindow>(this IServiceCollection services) where TWindow : Window
    {
        services.AddTransient<TWindow>();
        services.AddSingleton<Func<TWindow>>(sp => sp.GetRequiredService<TWindow>);
        return services;
    }
}
