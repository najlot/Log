// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

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
				{ nameof(SyncCollectMiddleware), typeof(SyncCollectMiddleware) },
				{ nameof(ConsoleDestination), typeof(ConsoleDestination) },
				{ nameof(FileDestination), typeof(FileDestination) },
				{ nameof(ConcurrentCollectMiddleware), typeof(ConcurrentCollectMiddleware) },
			};

			_typeToStringMapping = new Dictionary<Type, string>
			{
				{ typeof(FormatMiddleware), nameof(FormatMiddleware) },
				{ typeof(JsonFormatMiddleware), nameof(JsonFormatMiddleware) },
				{ typeof(SyncCollectMiddleware), nameof(SyncCollectMiddleware) },
				{ typeof(ConsoleDestination), nameof(ConsoleDestination) },
				{ typeof(FileDestination), nameof(FileDestination) },
				{ typeof(ConcurrentCollectMiddleware), nameof(ConcurrentCollectMiddleware) },
			};
		}

		public void AddToMapping<T>()
		{
			AddToMapping(typeof(T));
		}

		public void AddToMapping(Type type)
		{
			var attribute = type?.GetCustomAttributes(typeof(LogConfigurationNameAttribute), false).FirstOrDefault();

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

		public string GetName(object from)
		{
			return GetName(from?.GetType());
		}

		public string GetName<T>()
		{
			return GetName(typeof(T));
		}

		public string GetName(Type type)
		{
			if (type == null) return null;

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