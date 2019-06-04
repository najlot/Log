// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Najlot.Log
{
	internal class LogConfiguration : ILogConfiguration
	{
		public static LogConfiguration Instance { get; } = new LogConfiguration();

		internal LogConfiguration(){}

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

		private string _executionMiddlewareName = nameof(SyncExecutionMiddleware);

		public string ExecutionMiddlewareName
		{
			get
			{
				return _executionMiddlewareName;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					LogErrorHandler.Instance.Handle("New execution middleware name is null or empty");
					return;
				}

				var type = LogConfigurationMapper.Instance.GetType(value);

				if (type == null)
				{
					LogErrorHandler.Instance.Handle(value + " is not registered");
					return;
				}

				if (!typeof(IExecutionMiddleware).IsAssignableFrom(type))
				{
					LogErrorHandler.Instance.Handle(type.FullName + " does not implement IExecutionMiddleware");
					return;
				}

				if (_executionMiddlewareName != value)
				{
					_executionMiddlewareName = value;
					NotifyObservers();
				}
			}
		}

		#region Configuration observers

		private readonly List<IConfigurationObserver> _observerList = new List<IConfigurationObserver>();

		public void AttachObserver(IConfigurationObserver observer)
		{
			lock (_observerList)
			{
				_observerList.Add(observer);
			}
		}

		public void DetachObserver(IConfigurationObserver observer)
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

		#endregion Configuration observers

		#region Format middleware

		private readonly Dictionary<string, string> _formatMiddlewareNames = new Dictionary<string, string>();

		public string GetFormatMiddlewareName(string name)
		{
			string middlewareName;

			lock (_formatMiddlewareNames)
			{
				if (!_formatMiddlewareNames.TryGetValue(name, out middlewareName))
				{
					middlewareName = nameof(FormatMiddleware);
					_formatMiddlewareNames.Add(name, middlewareName);
				}
			}

			return middlewareName;
		}

		public IReadOnlyCollection<KeyValuePair<string, string>> GetFormatMiddlewares()
		{
			lock (_formatMiddlewareNames)
			{
				return new Dictionary<string, string>(_formatMiddlewareNames);
			}
		}

		public void SetFormatMiddleware<TMiddleware>(string name) where TMiddleware : IFormatMiddleware, new()
		{
			var middlewareName = LogConfigurationMapper.Instance.GetName<TMiddleware>();

			if (string.IsNullOrEmpty(middlewareName))
			{
				LogErrorHandler.Instance.Handle(typeof(TMiddleware).FullName + " is not registered");
				return;
			}

			if (!typeof(IFormatMiddleware).IsAssignableFrom(typeof(TMiddleware)))
			{
				LogErrorHandler.Instance.Handle(typeof(TMiddleware).FullName + " does not implement IFormatMiddleware");
				return;
			}

			lock (_formatMiddlewareNames)
			{
				if (!_formatMiddlewareNames.TryGetValue(name, out var oldMiddlewareName))
				{
					_formatMiddlewareNames.Add(name, middlewareName);
					NotifyObservers();
				}
				else if (oldMiddlewareName != middlewareName)
				{
					_formatMiddlewareNames[name] = middlewareName;
					NotifyObservers();
				}
			}
		}

		#endregion Format middleware

		#region Queue middleware

		private readonly Dictionary<string, string> _queueMiddlewareNames = new Dictionary<string, string>();

		public string GetQueueMiddlewareName(string name)
		{
			string middlewareName;

			lock (_queueMiddlewareNames)
			{
				if (!_queueMiddlewareNames.TryGetValue(name, out middlewareName))
				{
					middlewareName = nameof(NoQueueMiddleware);
					_queueMiddlewareNames.Add(name, middlewareName);
				}
			}

			return middlewareName;
		}

		public IReadOnlyCollection<KeyValuePair<string, string>> GetQueueMiddlewares()
		{
			lock (_queueMiddlewareNames)
			{
				return new Dictionary<string, string>(_queueMiddlewareNames);
			}
		}

		public void SetQueueMiddleware<TMiddleware>(string name) where TMiddleware : IQueueMiddleware, new()
		{
			var middlewareName = LogConfigurationMapper.Instance.GetName<TMiddleware>();

			if (string.IsNullOrEmpty(middlewareName))
			{
				LogErrorHandler.Instance.Handle(typeof(TMiddleware).FullName + " is not registered");
				return;
			}

			if (!typeof(IQueueMiddleware).IsAssignableFrom(typeof(TMiddleware)))
			{
				LogErrorHandler.Instance.Handle(typeof(TMiddleware).FullName + " does not implement IQueueMiddleware");
				return;
			}

			lock (_queueMiddlewareNames)
			{
				if (!_queueMiddlewareNames.TryGetValue(name, out var oldMiddlewareName))
				{
					_queueMiddlewareNames.Add(name, middlewareName);
					NotifyObservers();
				}
				else if (oldMiddlewareName != middlewareName)
				{
					_queueMiddlewareNames[name] = middlewareName;
					NotifyObservers();
				}
			}
		}

		#endregion Queue middleware

		#region Filter middleware

		private readonly Dictionary<string, string> _filterMiddlewareNames = new Dictionary<string, string>();

		public string GetFilterMiddlewareName(string name)
		{
			string middlewareName;

			lock (_filterMiddlewareNames)
			{
				if (!_filterMiddlewareNames.TryGetValue(name, out middlewareName))
				{
					middlewareName = nameof(NoFilterMiddleware);
					_filterMiddlewareNames.Add(name, middlewareName);
				}
			}

			return middlewareName;
		}

		public IReadOnlyCollection<KeyValuePair<string, string>> GetFilterMiddlewares()
		{
			lock (_filterMiddlewareNames)
			{
				return new Dictionary<string, string>(_filterMiddlewareNames);
			}
		}

		public void SetFilterMiddleware<TMiddleware>(string name) where TMiddleware : IFilterMiddleware, new()
		{
			var middlewareName = LogConfigurationMapper.Instance.GetName(typeof(TMiddleware));

			if (string.IsNullOrEmpty(middlewareName))
			{
				LogErrorHandler.Instance.Handle(typeof(TMiddleware).FullName + " is not registered");
				return;
			}

			if (!typeof(IFilterMiddleware).IsAssignableFrom(typeof(TMiddleware)))
			{
				LogErrorHandler.Instance.Handle(typeof(TMiddleware).FullName + " does not implement IQueueMiddleware");
				return;
			}

			lock (_filterMiddlewareNames)
			{
				if (!_filterMiddlewareNames.TryGetValue(name, out var oldMiddlewareName))
				{
					_filterMiddlewareNames.Add(name, middlewareName);
					NotifyObservers();
				}
				else if (oldMiddlewareName != middlewareName)
				{
					_filterMiddlewareNames[name] = middlewareName;
					NotifyObservers();
				}
			}
		}

		#endregion Filter middleware
	}
}