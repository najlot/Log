// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;

namespace Najlot.Log
{
	public static class LogAdministratorGenericExtensions
	{
		public static LogAdministrator AddMiddleware<TMiddleware, TDestination>(this LogAdministrator logAdministrator)
			where TMiddleware : IMiddleware
			where TDestination : IDestination
		{
			var destinationName = LogConfigurationMapper.Instance.GetName<TDestination>();
			var middlewareName = LogConfigurationMapper.Instance.GetName<TMiddleware>();

			return logAdministrator.AddMiddleware(destinationName, middlewareName);
		}

		public static LogAdministrator SetCollectMiddleware<TMiddleware, TDestination>(this LogAdministrator logAdministrator)
			where TMiddleware : ICollectMiddleware
			where TDestination : IDestination
		{
			var destinationName = LogConfigurationMapper.Instance.GetName<TDestination>();
			var middlewareName = LogConfigurationMapper.Instance.GetName<TMiddleware>();

			return logAdministrator.SetCollectMiddleware(destinationName, middlewareName);
		}
	}
}