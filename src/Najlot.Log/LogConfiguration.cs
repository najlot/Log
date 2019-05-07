using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log
{
	internal class LogConfiguration : ILogConfiguration
	{
		public static LogConfiguration Instance { get; } = new LogConfiguration();

		internal LogConfiguration()
		{
		}

		private LogLevel _logLevel = LogLevel.Debug;

		public LogLevel LogLevel
		{
			get
			{
				return _logLevel;
			}
			set
			{
				if (_logLevel != value)
				{
					_logLevel = value;
					NotifyObservers();
				}
			}
		}

		private Type _executionMiddlewareType = typeof(SyncExecutionMiddleware);

		public Type ExecutionMiddlewareType
		{
			get
			{
				return _executionMiddlewareType;
			}
			set
			{
				Type iExecutionMiddlewareType = typeof(IExecutionMiddleware);

				if (value.GetInterfaces().FirstOrDefault(x => x == iExecutionMiddlewareType) == null)
				{
					Console.WriteLine("Najlot.Log: New execution middleware does not implement " + iExecutionMiddlewareType.Name);
					return;
				}

				if (_executionMiddlewareType.FullName != value.FullName)
				{
					_executionMiddlewareType = value;
					NotifyObservers();
				}
			}
		}

		private Type _filterMiddlewareType = typeof(OpenFilterMiddleware);

		public Type FilterMiddlewareType
		{
			get => _filterMiddlewareType;
			set
			{
				Type iFilterMiddlewareType = typeof(IFilterMiddleware);

				if (value.GetInterfaces().FirstOrDefault(x => x == iFilterMiddlewareType) == null)
				{
					Console.WriteLine("Najlot.Log: New filter middleware does not implement " + iFilterMiddlewareType.Name);
					return;
				}

				if (_filterMiddlewareType.FullName != value.FullName)
				{
					_filterMiddlewareType = value;
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

		#region Format middleware

		private readonly Dictionary<Type, Type> _formatMiddlewareTypes = new Dictionary<Type, Type>();

		public void GetFormatMiddlewareTypeForType(Type type, out Type middlewareType)
		{
			lock (_formatMiddlewareTypes)
			{
				if (!_formatMiddlewareTypes.TryGetValue(type, out middlewareType))
				{
					middlewareType = typeof(DefaultFormatMiddleware);
				}
			}
		}

		public void SetFormatMiddlewareForType<TMiddleware>(Type type) where TMiddleware : IFormatMiddleware, new()
		{
			var middlewareType = typeof(TMiddleware);

			lock (_formatMiddlewareTypes)
			{
				if (!_formatMiddlewareTypes.TryGetValue(type, out var oldMiddlewareType))
				{
					_formatMiddlewareTypes.Add(type, middlewareType);
					NotifyObservers();
				}
				else if (oldMiddlewareType != middlewareType)
				{
					_formatMiddlewareTypes[type] = middlewareType;
					NotifyObservers();
				}
			}
		}
		#endregion Format middleware
	}
}