using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace TourismTracking;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        bool isLightweight = args != null && (Array.Exists(args, a => a.Equals("--lightweight", StringComparison.OrdinalIgnoreCase)) || Environment.GetEnvironmentVariable("LIGHTWEIGHT_MODE") == "true");

        var logConfig = new LoggerConfiguration();
        if (isLightweight)
        {
            logConfig.MinimumLevel.Warning()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning);
        }
        else
        {
#if DEBUG
            logConfig.MinimumLevel.Debug();
#else
            logConfig.MinimumLevel.Information();
#endif
            logConfig.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
        }

        Log.Logger = logConfig
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information(isLightweight ? "Starting TourismTracking.HttpApi.Host (LIGHTWEIGHT MODE)." : "Starting TourismTracking.HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<TourismTrackingHttpApiHostModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();

            if (isLightweight)
            {
                // Liberar memoria transitoria de inicialización
                GC.Collect(2, GCCollectionMode.Aggressive, true, true);
                GC.WaitForPendingFinalizers();
                GC.Collect(2, GCCollectionMode.Aggressive, true, true);
            }

            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
