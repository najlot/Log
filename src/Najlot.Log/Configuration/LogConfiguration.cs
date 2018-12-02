using Najlot.Log.Middleware;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Najlot.Log.Configuration
{
	/// <summary>
	/// Internal implementation of the ILogConfiguration interface
	/// </summary>
	internal class LogConfiguration : ILogConfiguration
	{
		public static LogConfiguration Instance { get; } = new LogConfiguration();

		internal LogConfiguration()
		{
		}

		private LogLevel logLevel = LogLevel.Debug;

		public LogLevel LogLevel
		{
			get
			{
				return logLevel;
			}
			set
			{
				if (logLevel != value)
				{
					logLevel = value;
					NotifyObservers();
				}
			}
		}

		private Type executionMiddlewareType = typeof(SyncExecutionMiddleware);

		public Type ExecutionMiddlewareType
		{
			get
			{
				return executionMiddlewareType;
			}
			set
			{
				if (value == null)
				{
					Console.WriteLine("Najlot.Log: New execution middleware type is null.");
					return;
				}

				Type iExecutionMiddlewareType = typeof(IExecutionMiddleware);

				if(value.GetInterfaces().FirstOrDefault(x => x == iExecutionMiddlewareType) == null)
				{
					Console.WriteLine("Najlot.Log: New execution middleware does not implement " + iExecutionMiddlewareType.Name);
					return;
				}

				if (executionMiddlewareType.FullName != value.GetType().FullName)
				{
					executionMiddlewareType = value;
					NotifyObservers();
				}
			}
		}

		#region ConfigurationChanged observers

		private readonly List<IConfigurationChangedObserver> _observerList = new List<IConfigurationChangedObserver>();

		public void AttachObserver(IConfigurationChangedObserver observer)
		{
			lock (_observerList)
			{
				_observerList.Add(observer);
			}
		}

		public void DetachObserver(IConfigurationChangedObserver observer)
		{
			bool couldRemove;

			lock (_observerList)
			{
				do
				{
					couldRemove = _observerList.Remove(observer);
				}
				while (couldRemove);
			}
		}

		public void NotifyObservers()
		{
			foreach (var observer in _observerList)
			{
				observer.NotifyConfigurationChanged(this);
			}
		}

		#endregion ConfigurationChanged observers

		#region Format functions

		private readonly Dictionary<Type, Func<LogMessage, string>> _formatFunctions = new Dictionary<Type, Func<LogMessage, string>>();

		public bool TryGetFormatFunctionForType(Type type, out Func<LogMessage, string> function)
		{
			lock (_formatFunctions)
			{
				return _formatFunctions.TryGetValue(type, out function);
			}
		}

		public bool TrySetFormatFunctionForType(Type type, Func<LogMessage, string> function)
		{
			bool addOrUpdateOk;

			if (function == null)
			{
				return false;
			}

			lock (_formatFunctions)
			{
				if (!_formatFunctions.TryGetValue(type, out Func<LogMessage, string> oldFunction))
				{
					_formatFunctions.Add(type, function);
					addOrUpdateOk = true;
				}
				else
				{
					addOrUpdateOk = oldFunction != function;

					if (addOrUpdateOk)
					{
						_formatFunctions[type] = function;
					}
				}
			}

			if (addOrUpdateOk)
			{
				NotifyObservers();
			}

			return addOrUpdateOk;
		}

		public void ClearAllFormatFunctions()
		{
			lock (_formatFunctions)
			{
				_formatFunctions.Clear();
			}

			NotifyObservers();
		}

		#endregion Format functions
	}
}