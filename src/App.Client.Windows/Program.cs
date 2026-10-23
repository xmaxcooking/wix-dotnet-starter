using App.Client.Controls;
using App.Client.Controls.Main;
using App.Infrastructure;
using App.Updates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Client.Windows;

internal static class Program
{
    [STAThread]
    private static async Task Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        configureServices(services);
        using var provider = services.BuildServiceProvider();

        if (await provider.GetRequiredService<UpdateCheck>().RunIfDueAsync())
        {
            return;
        }

        Application.Run(provider.GetRequiredService<MainForm>());
    }

    private static void configureServices(IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddDebug());
        services.AddHttpClient();

        services.AddInfrastructure();
        services.AddUpdates();
        services.AddClientControls();
    }
}