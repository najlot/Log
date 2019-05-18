using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log
{
	public class LogConfigurationMapper
	{
		public static LogConfigurationMapper Instance { get; } = new LogConfigurationMapper();

		private readonly Dictionary<string, Type> _stringToTypeMaping;
		private readonly Dictionary<Type, string> _typeToStringMapping;

		private LogConfigurationMapper()
		{
			_stringToTypeMaping = new Dictionary<string, Type>
			{
				{ nameof(FormatMiddleware), typeof(FormatMiddleware) },
				{ nameof(JsonFormatMiddleware), typeof(JsonFormatMiddleware) },
				{ nameof(NoFilterMiddleware), typeof(NoFilterMiddleware) },
				{ nameof(NoQueueMiddleware), typeof(NoQueueMiddleware) },
				{ nameof(SyncExecutionMiddleware), typeof(SyncExecutionMiddleware) },
				{ nameof(TaskExecutionMiddleware), typeof(TaskExecutionMiddleware) },
				{ nameof(ConsoleLogDestination), typeof(ConsoleLogDestination) },
				{ nameof(FileLogDestination), typeof(FileLogDestination) }
			};

			_typeToStringMapping = new Dictionary<Type, string>
			{
				{ typeof(FormatMiddleware), nameof(FormatMiddleware) },
				{ typeof(JsonFormatMiddleware), nameof(JsonFormatMiddleware) },
				{ typeof(NoFilterMiddleware), nameof(NoFilterMiddleware) },
				{ typeof(NoQueueMiddleware), nameof(NoQueueMiddleware) },
				{ typeof(SyncExecutionMiddleware), nameof(SyncExecutionMiddleware) },
				{ typeof(TaskExecutionMiddleware), nameof(TaskExecutionMiddleware) },
				{ typeof(ConsoleLogDestination), nameof(ConsoleLogDestination) },
				{ typeof(FileLogDestination), nameof(FileLogDestination) }
			};
		}

		public void AddToMapping<T>()
		{
			AddToMapping(typeof(T));
		}

		public void AddToMapping(Type type)
		{
			var attribute = type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), false).FirstOrDefault();

			if (attribute is LogConfigurationNameAttribute nameAttribute)
			{
				var name = nameAttribute.Name;

				lock (_stringToTypeMaping)
				{
					_stringToTypeMaping[name] = type;
				}

				lock (_typeToStringMapping)
				{
					_typeToStringMapping[type] = name;
				}
			}
		}

		public string GetName(object obj)
		{
			return GetName(obj.GetType());
		}

		public string GetName<T>()
		{
			return GetName(typeof(T));
		}

		public string GetName(Type type)
		{
			lock (_typeToStringMapping)
			{
				if (_typeToStringMapping.TryGetValue(type, out string value))
				{
					return value;
				}
			}

			return null;
		}

		public Type GetType(string name)
		{
			if (name == null)
			{
				return null;
			}

			lock (_stringToTypeMaping)
			{
				if (_stringToTypeMaping.TryGetValue(name, out Type value))
				{
					return value;
				}
			}

			return null;
		}
	}
}