﻿using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log
{
	public class LogConfigurationMapper
	{
		public static LogConfigurationMapper Instance { get; } = new LogConfigurationMapper();

		private Dictionary<string, Type> _stringToTypeMaping;
		private Dictionary<Type, string> _typeToStringMapping;

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