# Najlot.Log 

![Build status](https://dev.azure.com/Najlot/Log/_apis/build/status/Log%20build?branchName=master) ![NuGet Version](https://img.shields.io/nuget/v/Najlot.Log.svg) ![alert_status](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=alert_status) ![sqale_rating](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=sqale_rating) ![reliability_rating](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=reliability_rating) ![security_rating](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=security_rating) ![sqale_index](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=sqale_index) ![code_smells](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=code_smells) ![duplicated_lines_density](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=duplicated_lines_density) ![vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=najlot_Log&metric=vulnerabilities)

Simple and yet powerful logging library with performance and extensibility in mind.

### Getting started:
You can start with just two lines of code:
```csharp
LogConfigurator.Instance.GetLoggerPool(out LoggerPool loggerPool);
loggerPool.GetLogger(this.GetType()).Info("Hello, World!");
```
The logger does not know any destinations to log to and will log to console, starting with the "Debug" logging level.

You can set the logging level on initialization and may change it later:
```csharp
LogConfigurator.Instance
  .SetLogLevel(LogLevel.Trace)
  .GetLoggerPool(out LoggerPool loggerPool)
  .GetLogConfiguration(out ILogConfiguration logConfiguration);

var log = loggerPool.GetLogger(this.GetType());

log.Info("Hello, World!");

logConfiguration.LogLevel = LogLevel.Warn;
log.Info("This message will not be logged.");
```

You may set up file and console destinations, 
tell the logger to use custom formatting function for console destination and log asynchronous (Putting the writing in a Task):
```csharp
LogConfigurator.Instance
  .SetExecutionMiddleware<TaskExecutionMiddleware>()
  .AddConsoleLogDestination(message =>
  {
    return $"{message.DateTime} {message.Message} {message.Exception}";
  })
  .AddFileLogDestination("log.txt")
  .GetLoggerPool(out LoggerPool loggerPool)
  .GetLogConfiguration(out ILogConfiguration logConfiguration);
```

When you need something, then there is an example of a lot of things that are implemented:
```csharp
class Program
{
  // Function used to format the output for console.
  private static string FormatForConsole(LogMessage message)
  {
    return $"{message.DateTime} {message.Message} {message.Exception}";
  }

  static void Main(string[] args)
  {
    LogConfigurator.Instance
      .GetLogConfiguration(out ILogConfiguration logConfiguration)
      // Using synchronous or asynchronous middleware.
      // You can not use both. The last one gets applied.
      //.SetExecutionMiddleware<SyncExecutionMiddleware>()
      .SetExecutionMiddleware<TaskExecutionMiddleware>()

      // Tell the logger to write to a file and
      // calculate the file path it should write to.
      .AddFileLogDestination(() =>
      {
        var time = DateTime.Now;
        return $"{time.Year}-{time.Month}-{time.Day}-log.txt";
      })

      // Write to console using custom formatting
      // or write to a custom destination (console with colors there) using the same formatting
      //.AddConsoleLogDestination(FormatForConsole)
      .AddCustomDestination(new ColorfulConsoleDestination(logConfiguration), FormatForConsole)

      // Add a destination implemented below.
      .AddCustomDestination(new DebugDestination(logConfiguration))

      // Read configuration from an XML file,
      // write an example file when not found,
      // listen for and apply changes to all loggers, 
      // without restarting your application.
      .ReadConfigurationFromXmlFile("Najlot.Log.config", 
        listenForChanges: true,
        writeExampleIfSourceDoesNotExists: true)

      // Get the pool of loggers to take specific loggers from.
      .GetLoggerPool(out LoggerPool loggerPool);

    // Take specific logger.
    Logger log = loggerPool.GetLogger(typeof(Program));

    // Begin a scope of work
    using (log.BeginScope("start up"))
    {
      log.Trace("Beginning to start...");

      try
      {
        throw new NotImplementedException();
      }
      catch (Exception ex)
      {
        // Log a fatal error with the exception that caused the error.
        log.Fatal("Could not start the application: ", ex);
      }
    }

    log.Info("Press any key.");

    Console.Read();
  }
}

// Custom destination that writes to debug output.
public class DebugDestination : LogDestinationBase
{
  public DebugDestination(ILogConfiguration logConfiguration) : base(logConfiguration)
  {
  }

  protected override void Log(LogMessage message)
  {
    System.Diagnostics.Debug.WriteLine(Format(message));
  }
}
```


###### TODO:
- [ ] Add examples for Microsoft.Extensions.Logging.
- [ ] Implement structured logging.
