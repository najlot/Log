// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

namespace Najlot.Log.Middleware
{
	[LogConfigurationName(nameof(NoFilterMiddleware))]
	public class NoFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(LogMessage message) => true;
	}
}