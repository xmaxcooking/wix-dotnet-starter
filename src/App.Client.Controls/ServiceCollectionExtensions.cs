using App.Client.Controls.Main;
using App.Client.Controls.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace App.Client.Controls;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientControls(this IServiceCollection services)
    {
        services.addFormFactory<SettingsForm>();
        services.AddTransient<MainForm>();
        return services;
    }

    /// <summary>Registers a form as transient, plus a Func&lt;TForm&gt; factory so a longer-lived owner can create a fresh instance on demand.</summary>
    private static IServiceCollection addFormFactory<TForm>(this IServiceCollection services) where TForm : Form
    {
        services.AddTransient<TForm>();
        services.AddSingleton<Func<TForm>>(sp => sp.GetRequiredService<TForm>);
        return services;
    }
}