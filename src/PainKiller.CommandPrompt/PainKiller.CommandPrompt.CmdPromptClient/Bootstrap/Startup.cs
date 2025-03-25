using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using Serilog;
using Serilog.Extensions.Logging;

namespace PainKiller.CommandPrompt.CmdPromptClient.Bootstrap;

public static class Startup
{
    public static void ConfigureServices()
    {
        ConfigureLogging();
    }
    private static void ConfigureLogging()
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {MachineName}/{EnvironmentUserName} {SourceContext}: {Message:lj}{NewLine}{Exception}"
            );
        var serilogLogger = loggerConfig.CreateLogger();
        var loggerFactory = new SerilogLoggerFactory(serilogLogger);
        LoggerProvider.Configure(loggerFactory);
    }
}