﻿using Najlot.Log.Middleware;
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

		private string _executionMiddlewareName = nameof(SyncExecutionMiddleware);

		public string ExecutionMiddlewareName
		{
			get
			{
				return _executionMiddlewareName;
			}
			set
			{
				var type = LogConfigurationMapper.Instance.GetType(value);

				if (type == null)
				{
					return;
				}

				Type iExecutionMiddlewareType = typeof(IExecutionMiddleware);

				if (type.GetInterfaces().FirstOrDefault(x => x == iExecutionMiddlewareType) == null)
				{
					Console.WriteLine("Najlot.Log: New execution middleware does not implement " + iExecutionMiddlewareType.Name);
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

		public void GetFormatMiddlewareNameForName(string name, out string middlewareName)
		{
			lock (_formatMiddlewareNames)
			{
				if (!_formatMiddlewareNames.TryGetValue(name, out middlewareName))
				{
					middlewareName = nameof(FormatMiddleware);
					_formatMiddlewareNames.Add(name, middlewareName);
				}
			}
		}

		public IReadOnlyCollection<KeyValuePair<string, string>> GetFormatMiddlewares()
		{
			lock (_formatMiddlewareNames)
			{
				return new Dictionary<string, string>(_formatMiddlewareNames);
			}
		}

		public void SetFormatMiddlewareForName<TMiddleware>(string name) where TMiddleware : IFormatMiddleware, new()
		{
			var middlewareType = typeof(TMiddleware);
			var middlewareName = LogConfigurationMapper.Instance.GetName(middlewareType);

			if (middlewareName == null)
			{
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

		public void GetQueueMiddlewareNameForName(string name, out string middlewareName)
		{
			lock (_queueMiddlewareNames)
			{
				if (!_queueMiddlewareNames.TryGetValue(name, out middlewareName))
				{
					middlewareName = nameof(NoQueueMiddleware);
					_queueMiddlewareNames.Add(name, middlewareName);
				}
			}
		}

		public IReadOnlyCollection<KeyValuePair<string, string>> GetQueueMiddlewares()
		{
			lock (_queueMiddlewareNames)
			{
				return new Dictionary<string, string>(_queueMiddlewareNames);
			}
		}

		public void SetQueueMiddlewareForName<TMiddleware>(string name) where TMiddleware : IQueueMiddleware, new()
		{
			var middlewareType = typeof(TMiddleware);
			var middlewareName = LogConfigurationMapper.Instance.GetName(middlewareType);

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

		public void GetFilterMiddlewareNameForName(string name, out string middlewareName)
		{
			lock (_filterMiddlewareNames)
			{
				if (!_filterMiddlewareNames.TryGetValue(name, out middlewareName))
				{
					middlewareName = nameof(NoFilterMiddleware);
					_filterMiddlewareNames.Add(name, middlewareName);
				}
			}
		}

		public IReadOnlyCollection<KeyValuePair<string, string>> GetFilterMiddlewares()
		{
			lock (_filterMiddlewareNames)
			{
				return new Dictionary<string, string>(_filterMiddlewareNames);
			}
		}

		public void SetFilterMiddlewareForName<TMiddleware>(string name) where TMiddleware : IFilterMiddleware, new()
		{
			var middlewareType = typeof(TMiddleware);
			var middlewareName = LogConfigurationMapper.Instance.GetName(middlewareType);

			if (middlewareName == null)
			{
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