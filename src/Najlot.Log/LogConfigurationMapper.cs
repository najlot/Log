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

		public Dictionary<string, Type> _stringToTypeMaping = new Dictionary<string, Type>();
		public Dictionary<Type, string> _typeToStringMapping = new Dictionary<Type, string>();

		private LogConfigurationMapper()
		{
			AddToMapping(nameof(FormatMiddleware), typeof(FormatMiddleware));
			AddToMapping(nameof(JsonFormatMiddleware), typeof(JsonFormatMiddleware));
			AddToMapping(nameof(NoFilterMiddleware), typeof(NoFilterMiddleware));
			AddToMapping(nameof(NoQueueMiddleware), typeof(NoQueueMiddleware));
			AddToMapping(nameof(SyncExecutionMiddleware), typeof(SyncExecutionMiddleware));
			AddToMapping(nameof(TaskExecutionMiddleware), typeof(TaskExecutionMiddleware));
			AddToMapping(nameof(ConsoleLogDestination), typeof(ConsoleLogDestination));
			AddToMapping(nameof(FileLogDestination), typeof(FileLogDestination));
		}

		private void AddToMapping(string name, Type type)
		{
			lock (_stringToTypeMaping)
			{
				_stringToTypeMaping[name] = type;
			}

			lock (_typeToStringMapping)
			{
				_typeToStringMapping[type] = name;
			}
		}

		public void AddToMapping(Type type)
		{
			var attribute = type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), false).FirstOrDefault();

			if (attribute is LogConfigurationNameAttribute nameAttribute)
			{
				var name = nameAttribute.Name;
				AddToMapping(name, type);
			}
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