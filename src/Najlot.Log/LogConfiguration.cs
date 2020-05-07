// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System;
using System.Collections.Generic;

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

					lock (_loglevelObserverList)
					{
						foreach (var observer in _loglevelObserverList)
						{
							try
							{
								observer.NotifyLogLevelChanged(value);
							}
							catch (Exception ex)
							{
								LogErrorHandler.Instance.Handle("An error while notifying occured.", ex);
							}
						}
					}
				}
			}
		}

		public Dictionary<string, List<string>> Middlewares { get; } = new Dictionary<string, List<string>>();
		public Dictionary<string, string> CollectMiddlewares { get; } = new Dictionary<string, string>();

		public IEnumerable<string> GetDestinationNames()
		{
			foreach (var middlewares in Middlewares)
			{
				yield return middlewares.Key;
			}
		}

		public void AddMiddleware<TMiddleware, TDestination>()
			where TMiddleware : IMiddleware
			where TDestination : IDestination
		{
			var destinationName = LogConfigurationMapper.Instance.GetName<TDestination>();
			var middlewareName = LogConfigurationMapper.Instance.GetName<TMiddleware>();

			AddMiddleware(destinationName, middlewareName);
		}

		public void AddMiddleware(string destinationName, string middlewareName)
		{
			if (Middlewares.TryGetValue(destinationName, out var list))
			{
				list.Add(middlewareName);
			}
			else
			{
				var newList = new List<string>
				{
					middlewareName
				};

				Middlewares.Add(destinationName, newList);
			}

			lock (_observerList)
			{
				foreach (var observer in _observerList)
				{
					try
					{
						observer.NotifyMiddlewareAdded(destinationName, middlewareName);
					}
					catch (Exception ex)
					{
						LogErrorHandler.Instance.Handle("An error while notifying occured.", ex);
					}
				}
			}
		}

		public void SetCollectMiddleware<TMiddleware, TDestination>()
			where TMiddleware : ICollectMiddleware
			where TDestination : IDestination
		{
			var destinationName = LogConfigurationMapper.Instance.GetName<TDestination>();
			var middlewareName = LogConfigurationMapper.Instance.GetName<TMiddleware>();

			SetCollectMiddleware(destinationName, middlewareName);
		}

		public void SetCollectMiddleware(string destinationName, string middlewareName)
		{
			CollectMiddlewares[destinationName] = middlewareName;

			lock (_observerList)
			{
				foreach (var observer in _observerList)
				{
					try
					{
						observer.NotifyCollectMiddlewareChanged(destinationName, middlewareName);
					}
					catch (Exception ex)
					{
						LogErrorHandler.Instance.Handle("An error while notifying occured.", ex);
					}
				}
			}
		}

		public IEnumerable<string> GetMiddlewareNames(string destinationName)
		{
			if (Middlewares.TryGetValue(destinationName, out var list))
			{
				return list;
			}

			return Array.Empty<string>();
		}

		public string GetCollectMiddlewareName(string destinationName)
		{
			if (CollectMiddlewares.TryGetValue(destinationName, out var entry))
			{
				return entry;
			}

			return LogConfigurationMapper.Instance.GetName<SyncCollectMiddleware>();
		}

		#region Configuration observers

		private readonly List<IMiddlewareConfigurationObserver> _observerList = new List<IMiddlewareConfigurationObserver>();
		private readonly List<ILogLevelObserver> _loglevelObserverList = new List<ILogLevelObserver>();

		public void AttachObserver(IMiddlewareConfigurationObserver observer)
		{
			lock (_observerList)
			{
				_observerList.Add(observer);
			}
		}

		public void DetachObserver(IMiddlewareConfigurationObserver observer)
		{
			lock (_observerList)
			{
				while (_observerList.Remove(observer))
				{
					// Remove returns true, if it could remove.
					// -> Remove all
				}
			}
		}

		public void AttachObserver(ILogLevelObserver observer)
		{
			lock (_loglevelObserverList)
			{
				_loglevelObserverList.Add(observer);
			}
		}

		public void DetachObserver(ILogLevelObserver observer)
		{
			lock (_loglevelObserverList)
			{
				while (_loglevelObserverList.Remove(observer))
				{
					// Remove returns true, if it could remove.
					// -> Remove all
				}
			}
		}

		public void ClearMiddlewares(string destinationName)
		{
			if (Middlewares.TryGetValue(destinationName, out var list))
			{
				list.Clear();
			}

			lock (_observerList)
			{
				foreach (var observer in _observerList)
				{
					try
					{
						observer.NotifyClearMiddlewares(destinationName);
					}
					catch (Exception ex)
					{
						LogErrorHandler.Instance.Handle("An error while notifying occured.", ex);
					}
				}
			}
		}

		#endregion Configuration observers
	}
}