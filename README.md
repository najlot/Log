# Najlot.Log 
![NuGet Version](https://img.shields.io/nuget/v/Najlot.Log.svg)

A high-performance, extensible logging library for .NET applications targeting .NET Standard 2.0. Provides flexible architecture with configurable destinations, middleware pipeline, and extensive customization options.

## 📦 Packages

- **Najlot.Log** - Core logging library
- **Najlot.Log.Configuration.FileSource** - XML/JSON configuration support
- **Najlot.Log.Extensions.Logging** - Microsoft.Extensions.Logging integration

## 🚀 Getting Started

### Basic Usage
Start logging with just two lines of code:
```csharp
var log = LogAdministrator.Instance.AddConsoleDestination().GetLogger(typeof(Program));
log.Info("Hello, World!");
```

### Setting Log Level
Configure logging level and use colored console output:
```csharp
LogAdministrator.Instance
    .AddConsoleDestination(useColors: true)
    .SetLogLevel(LogLevel.Info);

var log = LogAdministrator.Instance.GetLogger(typeof(Program));

log.Info("This will be logged");
log.Debug("This will not be logged");

// Change log level at runtime
LogAdministrator.Instance.SetLogLevel(LogLevel.Debug);
log.Debug("Now this will be logged");
```

### Asynchronous Logging
Configure async processing for better performance:
```csharp
LogAdministrator.Instance
    .SetCollectMiddleware<ConcurrentCollectMiddleware, FileDestination>()
    .SetCollectMiddleware<SyncCollectMiddleware, ConsoleDestination>()
    .AddConsoleDestination(useColors: true)
    .AddFileDestination("logs/app-{Year}-{Month}-{Day}.log");
```

## 📋 Advanced Features

### Structured Logging
Use structured logging with parameters for better searchability:
```csharp
var log = LogAdministrator.Instance.AddConsoleDestination().GetLogger(typeof(Program));

// Structured logging
log.Info("User {UserId} logged in from {IPAddress}", 12345, "192.168.1.1");
log.Error("Failed to process order {OrderId} for customer {CustomerId}", orderId, customerId);
```

### Scoped Logging
Group related log messages with scopes:
```csharp
var log = LogAdministrator.Instance.AddConsoleDestination().GetLogger(typeof(Program));

using (log.BeginScope("UserRegistration"))
{
    log.Info("Starting user registration process");
    log.Debug("Validating user input");
    
    try
    {
        // Registration logic
        log.Info("User registered successfully");
    }
    catch (Exception ex)
    {
        log.Error(ex, "User registration failed");
    }
}
```

### JSON Formatting
Output logs in JSON format for structured logging systems:
```csharp
LogAdministrator.Instance
    .AddMiddleware<JsonFormatMiddleware, FileDestination>()
    .AddFileDestination("logs/app.json")
    .AddConsoleDestination(useColors: true);

var log = LogAdministrator.Instance.GetLogger(typeof(Program));
log.Info("This will be JSON formatted in the file");
```

### Multiple Destinations
Send logs to different destinations with different configurations:
```csharp
LogAdministrator.Instance
    // File logging with JSON format
    .SetCollectMiddleware<ConcurrentCollectMiddleware, FileDestination>()
    .AddMiddleware<JsonFormatMiddleware, FileDestination>()
    .AddFileDestination("logs/{Year}-{Month}-{Day}.json")
    
    // Console logging with colors
    .AddConsoleDestination(useColors: true)
    
    // HTTP logging to remote server
    .AddHttpDestination("https://logs.example.com/api/logs", "your-auth-token");
```

## 🔧 Configuration

### XML Configuration
Load configuration from XML files with hot-reload support:
```csharp
// Add required components to mapping first
LogConfigurationMapper.Instance.AddToMapping<JsonFormatMiddleware>();

LogAdministrator.Instance
    .ReadConfigurationFromXmlFile("Najlot.Log.config",
        listenForChanges: true,
        writeExampleIfSourceDoesNotExists: true);
```

### JSON Configuration  
Load configuration from JSON files:
```csharp
LogAdministrator.Instance
    .ReadConfigurationFromJsonFile("appsettings.json",
        listenForChanges: true,
        writeExampleIfSourceDoesNotExists: true);
```

### Microsoft.Extensions.Configuration Integration
Use with ASP.NET Core or .NET Generic Host:
```csharp
// Add the NuGet package: Najlot.Log.Configuration.FileSource
LogAdministrator.Instance.ReadConfiguration(configuration, "Logging");
```

## 🔌 Microsoft.Extensions.Logging Integration

Use Najlot.Log as a provider for Microsoft.Extensions.Logging:

```csharp
// Add the NuGet package: Najlot.Log.Extensions.Logging
using Microsoft.Extensions.Logging;
using Najlot.Log;
using Najlot.Log.Extensions.Logging;

// With ILoggerFactory
var loggerFactory = new LoggerFactory();
loggerFactory.AddNajlotLog(administrator =>
{
    administrator
        .SetLogLevel(Najlot.Log.LogLevel.Info)
        .AddConsoleDestination(useColors: true)
        .AddFileDestination("logs/app.log");
});

var logger = loggerFactory.CreateLogger("MyApp");
logger.LogInformation("Hello from Microsoft.Extensions.Logging!");

// With ILoggingBuilder (ASP.NET Core)
builder.Logging.AddNajlotLog(administrator =>
{
    administrator
        .AddConsoleDestination(useColors: true)
        .AddFileDestination("logs/web-{Year}-{Month}-{Day}.log");
});
```

## 🔧 Custom Extensions

### Custom Middleware
Create custom middleware to transform log messages:
```csharp
[LogConfigurationName(nameof(TimestampMiddleware))]
public sealed class TimestampMiddleware : IMiddleware
{
    public IMiddleware NextMiddleware { get; set; }

    public void Execute(IEnumerable<LogMessage> messages)
    {
        foreach (var message in messages)
        {
            message.Message = $"[{message.DateTime:yyyy-MM-dd HH:mm:ss}] {message.Message}";
        }

        NextMiddleware?.Execute(messages);
    }

    public void Flush() => NextMiddleware?.Flush();
    public void Dispose() => NextMiddleware?.Dispose();
}
```

### Custom Destination
Create custom destinations to send logs anywhere:
```csharp
[LogConfigurationName(nameof(DatabaseDestination))]
public sealed class DatabaseDestination : IDestination
{
    private readonly string _connectionString;

    public DatabaseDestination(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Log(IEnumerable<LogMessage> messages)
    {
        foreach (var message in messages)
        {
            // Save to database
            SaveToDatabase(message);
        }
    }

    public void Flush() { }
    public void Dispose() { }

    private void SaveToDatabase(LogMessage message)
    {
        // Implementation here
    }
}

// Usage
LogAdministrator.Instance
    .AddCustomDestination(new DatabaseDestination("connection-string"));
```

## ✨ Features

- **High Performance**: Minimal allocation logging with optional asynchronous processing
- **Extensible**: Plugin-based destinations and middleware system  
- **Thread-Safe**: Built-in concurrent and synchronous middleware options
- **Structured Logging**: Support for structured logging with parameters
- **Flexible Configuration**: Programmatic, XML, JSON, and Microsoft.Extensions.Logging integration
- **Hot Reload**: Configuration changes without application restart
- **Multiple Destinations**: Console, File, HTTP, and custom destinations
- **Rich Formatting**: JSON output, custom formatting, and colored console output
- **Path Templating**: Dynamic file paths with `{Year}`, `{Month}`, `{Day}` placeholders
- **.NET Standard 2.0**: Compatible with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+

## 📚 Log Levels

Available log levels in order of severity:
- `LogLevel.Trace` - Very detailed logs, typically for debugging
- `LogLevel.Debug` - Debugging information
- `LogLevel.Info` - General information
- `LogLevel.Warn` - Warning messages
- `LogLevel.Error` - Error messages
- `LogLevel.Fatal` - Critical failures

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.