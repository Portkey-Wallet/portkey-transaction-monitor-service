using System;
using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Transaction.Monitor;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        try
        {
            Log.Information("Starting Transaction.Monitor.HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog((context, services, loggerConfiguration) =>
                {
                    loggerConfiguration
#if DEBUG
                        .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Async(c => c.File("Logs/logs.txt"))
                        .WriteTo.Async(c => c.Console())
                        .WriteTo.Async(c => c.AbpStudio(services));
                }).ConfigureAppConfiguration(
                    (_, builder) => { builder.AddJsonFile("appsettings.apollo.json"); })
                .ConfigureAppConfiguration((_, builder) =>
                {
                    if (!builder.Build().GetValue<bool>("IsApolloEnabled", false))
                    {
                        return;
                    }
                    //To display the Apollo console logs 
                    #if DEBUG
                    LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
                    #endif
                    builder.AddApollo(builder.Build().GetSection("apollo"));
                });
            await builder.AddApplicationAsync<MonitorHttpApiHostModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
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