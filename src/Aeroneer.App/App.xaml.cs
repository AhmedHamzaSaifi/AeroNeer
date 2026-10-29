using Aeroneer.App.ViewModels;
using Aeroneer.Core.Assessments;
using Aeroneer.Core.Models;
using Aeroneer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Aeroneer.App;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("Postgres");
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    services.AddDbContextFactory<AppDbContext>(options => options.UseNpgsql(connectionString));
                    services.AddTransient<ICandidateRepository, EfCandidateRepository>();
                    services.AddTransient<IAssessmentLifecycleStore, EfAssessmentLifecycleStore>();
                    services.AddTransient<RegistrationViewModel>();
                }
            })
            .Build();

        await _host.StartAsync();
            var contextFactory = _host.Services.GetService<IDbContextFactory<AppDbContext>>();
            if (contextFactory is null)
        {
            MessageBox.Show(
                "Aeroneer needs a local PostgreSQL connection. Set ConnectionStrings__Postgres and start the local database. See README.md for setup steps.",
                "Local database required",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            Shutdown(-1);
            return;
        }

        try
        {
            await using var dbContext = await contextFactory.CreateDbContextAsync();
            await dbContext.Database.MigrateAsync();
            var mainWindow = new MainWindow
            {
                DataContext = _host.Services.GetRequiredService<RegistrationViewModel>()
            };
            MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception)
        {
            MessageBox.Show(
                "Aeroneer could not connect to the local database or apply its schema. Check the PostgreSQL service and connection settings, then review README.md.",
                "Database unavailable",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            _host.StopAsync().GetAwaiter().GetResult();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
