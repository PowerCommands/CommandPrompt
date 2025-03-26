using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Configuration.Services;
using PainKiller.CommandPrompt.CoreLib.Logging.Services;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace PainKiller.CommandPrompt.CmdPromptClient.Bootstrap;

public static class Startup
{
    public static CommandPromptConfiguration ConfigureServices()
    {
        var configuration = ConfigurationService.Service.Get<CommandPromptConfiguration>();
        ConfigureLogging(configuration.Configuration.Log);
        return configuration.Configuration;
    }
    private static void ConfigureLogging(LogConfiguration config)
    {
        var parsedLevel = Enum.TryParse<LogEventLevel>(config.RestrictedToMinimumLevel, ignoreCase: true, out var minimumLevel) ? minimumLevel : LogEventLevel.Information;
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(parsedLevel)
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: Path.Combine(config.FilePath, config.FileName),
                rollingInterval: (RollingInterval) Enum.Parse(typeof(RollingInterval), config.RollingIntervall),
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {MachineName}/{EnvironmentUserName} {SourceContext}: {Message:lj}{NewLine}{Exception}"
            );
        var serilogLogger = loggerConfig.CreateLogger();
        var loggerFactory = new SerilogLoggerFactory(serilogLogger);
        LoggerProvider.Configure(loggerFactory);
    }
}