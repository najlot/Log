# Najlot.Log - Copilot Instructions

## Project Overview

Najlot.Log is a high-performance, extensible logging library for .NET applications targeting .NET Standard 2.0. It provides a flexible architecture with configurable destinations, middleware pipeline, and extensive customization options while maintaining excellent performance characteristics.

### Key Features
- **High Performance**: Minimal allocation logging with optional asynchronous processing
- **Extensible Architecture**: Plugin-based destinations and middleware system
- **Configuration Flexibility**: Programmatic, XML file-based, and Microsoft.Extensions.Logging integration
- **Thread Safety**: Built-in concurrent and synchronous middleware options
- **Rich Formatting**: JSON, structured logging, and custom format support

## Architecture Overview

### Core Components

**LogAdministrator** (`LogAdministrator.cs`)
- Central configuration hub and singleton instance
- Manages destinations, middleware chains, and log levels
- Provides fluent API for setup: `LogAdministrator.Instance.AddConsoleDestination().SetLogLevel(LogLevel.Info)`

**Logger** (`Logger.cs`, `ILogger.cs`)
- Main logging interface with methods: `Trace()`, `Debug()`, `Info()`, `Warn()`, `Error()`, `Fatal()`
- Supports structured logging with parameters: `log.Info("User {UserId} logged in", userId)`
- Includes scoping support: `using (log.BeginScope("operation")) { }`

**Destinations** (`Destinations/`)
- `ConsoleDestination`: Output to console with optional colors
- `FileDestination`: File logging with path templating (`{Year}-{Month}-{Day}.log`)
- `HttpDestination`: HTTP endpoint logging
- `IDestination`: Interface for custom destinations

**Middleware** (`Middleware/`)
- `SyncCollectMiddleware`/`ConcurrentCollectMiddleware`: Processing mode control
- `FormatMiddleware`: Message formatting pipeline
- `JsonFormatMiddleware`: JSON output formatting
- `IMiddleware`/`ICollectMiddleware`: Extension interfaces

**Configuration** 
- `LogConfigurationMapper`: XML configuration support with hot-reload
- File-based configuration with `ReadConfigurationFromXmlFile()`
- Attribute-based middleware discovery via `LogConfigurationNameAttribute`

## Development Standards

### Code Style
- Use latest C# language features (nullable reference types, collection expressions, etc.)
- Follow Microsoft naming conventions strictly
- Prefer `sealed` classes where inheritance isn't intended
- Use `readonly` fields and properties where possible
- Initialize collections with collection expressions: `private readonly List<IDestination> _destinations = [];`

### Patterns to Follow

**Destination Implementation**
```csharp
[LogConfigurationName(nameof(MyDestination))]
public sealed class MyDestination : IDestination
{
    public void Log(IEnumerable<LogMessage> messages)
    {
        foreach (var message in messages)
        {
            // Process message
        }
    }

    public void Flush() { }
    public void Dispose() { }
}
```

**Middleware Implementation**
```csharp
[LogConfigurationName(nameof(MyMiddleware))]
public sealed class MyMiddleware : IMiddleware
{
    public IMiddleware NextMiddleware { get; set; }

    public void Execute(IEnumerable<LogMessage> messages)
    {
        // Transform messages
        var transformedMessages = messages.Select(Transform);
        NextMiddleware.Execute(transformedMessages);
    }

    public void Flush() => NextMiddleware?.Flush();
    public void Dispose() => NextMiddleware?.Dispose();
}
```

**Extension Method Pattern**
```csharp
public static LogAdministrator AddMyDestination(this LogAdministrator administrator, /* parameters */)
{
    return administrator.AddDestination(new MyDestination(/* parameters */));
}
```

### Error Handling
- Use defensive programming - validate parameters
- Prefer graceful degradation over exceptions in logging paths
- Use `LogErrorHandler` for internal error reporting
- Follow fail-fast principle in configuration methods

### Performance Considerations
- Minimize allocations in hot paths
- Use `Span<T>` and `ReadOnlySpan<T>` for string operations where applicable
- Prefer struct types for value objects (like `LogLevel`)
- Cache expensive operations in static fields
- Use concurrent collections for thread-safe scenarios

## Testing Standards

### Test Structure
- Use MSTest framework with `[TestClass]` and `[TestMethod]` attributes
- Follow naming convention: `[MethodName]_[Scenario]_[ExpectedResult]`
- Group related tests in nested classes for organization

### Required Test Coverage
- **Public API Methods**: Every public method must have tests
- **Configuration Scenarios**: Test all configuration combinations
- **Error Conditions**: Test validation and error handling
- **Thread Safety**: Test concurrent access where applicable
- **Integration**: Test complete logging pipeline

### Test Patterns
```csharp
[TestClass]
public class LogAdministratorTests
{
    [TestMethod]
    public void AddConsoleDestination_WithColors_ConfiguresCorrectly()
    {
        // Arrange
        var administrator = LogAdministrator.CreateNew();
        
        // Act
        administrator.AddConsoleDestination(useColors: true);
        
        // Assert
        var logger = administrator.GetLogger<LogAdministratorTests>();
        logger.Info("Test message");
        // Verify expected behavior
    }
}
```

## Build and Development Workflow

### Prerequisites
- .NET 8 SDK (for building and testing)
- Target: .NET Standard 2.0 (for library compatibility)

### Commands
```bash
# Restore packages
dotnet restore src/Najlot.Log.sln

# Build solution
dotnet build src/Najlot.Log.sln

# Run tests
dotnet test src/Najlot.Log.sln

# Pack NuGet packages
dotnet pack src/Najlot.Log.sln -c Release
```

### Project Structure
```
src/
├── Najlot.Log/                              # Core library
│   ├── Destinations/                        # Built-in destinations
│   ├── Middleware/                          # Processing pipeline
│   ├── Util/                               # Helper utilities
│   └── LogAdministrator.cs                 # Main configuration API
├── Najlot.Log.Configuration.FileSource/    # XML configuration
├── Najlot.Log.Extensions.Logging/          # Microsoft.Extensions.Logging provider
└── Najlot.Log.Tests/                       # Comprehensive test suite
```

## Common Development Tasks

### Adding a New Destination
1. Create class implementing `IDestination` in `Destinations/` folder
2. Add `LogConfigurationNameAttribute` for XML configuration support
3. Implement `Log()`, `Flush()`, and `Dispose()` methods
4. Create extension method in `LogAdministratorDestinationExtensions.cs`
5. Add comprehensive tests in `Najlot.Log.Tests/`
6. Update configuration schema if needed

### Adding Middleware
1. Implement `IMiddleware` interface in `Middleware/` folder
2. Ensure proper chaining with `NextMiddleware` property
3. Add configuration attribute for discoverability
4. Create fluent API extension methods
5. Write tests covering transformation logic
6. Document any performance implications

### Configuration Changes
1. Update XML schema in configuration project
2. Add corresponding properties to configuration classes
3. Implement mapping logic in `LogConfigurationMapper`
4. Add validation and error handling
5. Write tests for new configuration scenarios
6. Update example configurations

## Extension Points

### Custom Destinations
Implement `IDestination` for new output targets:
- Database logging
- Cloud service integration
- Message queues
- Custom file formats

### Custom Middleware
Implement `IMiddleware` for message transformation:
- Filtering and sampling
- Data enrichment
- Format conversion
- Performance monitoring

### Configuration Sources
Extend configuration system:
- Environment variables
- Cloud configuration
- Database configuration
- Real-time updates

## Security Considerations

- **No Sensitive Data**: Never log passwords, tokens, or PII by default
- **Input Validation**: Validate all configuration inputs
- **File Permissions**: Ensure appropriate file access permissions
- **Network Security**: Secure HTTP destination endpoints
- **Configuration Security**: Protect configuration files from unauthorized access

## Performance Guidelines

### Logging Best Practices
- Use appropriate log levels to control verbosity
- Prefer structured logging over string concatenation
- Use scopes to group related operations
- Configure async middleware for high-throughput scenarios

### Memory Management
- Destinations should not hold references to `LogMessage` objects
- Dispose resources properly in `Dispose()` methods
- Use object pooling for high-frequency scenarios
- Monitor allocations in performance-critical paths

## Debugging and Troubleshooting

### Common Issues
- **Configuration Problems**: Check XML schema validation
- **Performance Issues**: Profile middleware pipeline
- **Thread Safety**: Verify concurrent access patterns
- **Memory Leaks**: Ensure proper disposal chain

### Debugging Tools
- Use `LogErrorHandler` to capture internal errors
- Enable verbose logging for troubleshooting
- Use profiling tools for performance analysis
- Test with concurrent load for thread safety verification

## Version and Compatibility

- **Current Version**: 4.0.4
- **Target Framework**: .NET Standard 2.0 (compatible with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+)
- **Semantic Versioning**: Major.Minor.Patch (breaking.feature.fix)
- **Backward Compatibility**: Maintain API compatibility within major versions

## AI Assistant Guidelines

When working on this codebase:

1. **Understand the Pipeline**: Log messages flow through middleware chain to destinations
2. **Maintain Performance**: This is a logging library - performance is critical
3. **Preserve Patterns**: Follow established architectural patterns
4. **Test Thoroughly**: Logging libraries must be extremely reliable
5. **Document Changes**: Update examples and documentation for API changes
6. **Consider Threading**: Many scenarios involve concurrent logging
7. **Validate Inputs**: Configuration errors should be caught early
8. **Review Examples**: Check if changes require updates to README examples

Always prioritize reliability, performance, and backward compatibility when making changes to this library.