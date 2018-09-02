using NajlotLog.Middleware;
using System;
using System.Collections.Generic;

namespace NajlotLog.Configuration
{
	internal class LogConfiguration : ILogConfiguration
	{
		public static ILogConfiguration Instance { get; } = new LogConfiguration();

		internal LogConfiguration()
		{

		}
		
		LogLevel logLevel = LogLevel.Debug;
		public LogLevel LogLevel
		{
			get
			{
				return logLevel;
			}
			set
			{
				if(logLevel != value)
				{
					logLevel = value;
					NotifyObservers();
				}
			}
		}

		ILogExecutionMiddleware logExecutionMiddleware = new SyncLogExecutionMiddleware();
		public ILogExecutionMiddleware LogExecutionMiddleware
		{
			get
			{
				return logExecutionMiddleware;
			}
			set
			{
				if(logExecutionMiddleware != value)
				{
					// Observers get new middleware and we dispose the old one
					using (var oldMiddleware = logExecutionMiddleware)
					{
						logExecutionMiddleware = value;
						NotifyObservers();
					}
				}
			}
		}

		#region ConfigurationChanged observers
		private List<IConfigurationChangedObserver> _observerList = new List<IConfigurationChangedObserver>();

		public void AttachObserver(IConfigurationChangedObserver observer)
		{
			lock(_observerList)
			{
				_observerList.Add(observer);
			}
		}

		public void DetachObserver(IConfigurationChangedObserver observer)
		{
			lock (_observerList)
			{
				while(_observerList.Remove(observer));
			}
		}
		
		public void NotifyObservers(Type type = null)
		{
			lock (_observerList)
			{
				foreach (var observer in _observerList)
				{
					if(type != null)
					{
						if (type != observer.GetType())
						{
							continue;
						}
					}
					
					observer.NotifyConfigurationChanged(this);
				}
			}
		}
		#endregion

		#region Format functions
		private Dictionary<Type, Func<LogMessage, string>> _formatFunctions = new Dictionary<Type, Func<LogMessage, string>>();

		public bool TryGetFormatFunctionForType(Type type, out Func<LogMessage, string> function)
		{
			lock(_formatFunctions)
			{
				return _formatFunctions.TryGetValue(type, out function);
			}
		}
		
		public bool TrySetFormatFunctionForType(Type type, Func<LogMessage, string> function)
		{
			bool addOrUpdateOk;

			if(function == null)
			{
				return false;
			}

			lock (_formatFunctions)
			{
				Func<LogMessage, string> oldFunction;

				if (!_formatFunctions.TryGetValue(type, out oldFunction))
				{
					_formatFunctions.Add(type, function);
					addOrUpdateOk = true;
				}
				else
				{
					addOrUpdateOk = oldFunction != function;

					if(addOrUpdateOk)
					{
						_formatFunctions[type] = function;
					}
				}
			}

			if (addOrUpdateOk)
			{
				NotifyObservers(type);
			}

			return addOrUpdateOk;
		}

		public void ClearAllFormatFunctions()
		{
			lock (_formatFunctions)
			{
				_formatFunctions.Clear();
			}
		}
		#endregion
	}
}
