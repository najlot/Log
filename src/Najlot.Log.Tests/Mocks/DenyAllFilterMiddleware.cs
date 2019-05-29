// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

using Najlot.Log.Middleware;

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(DenyAllFilterMiddleware))]
	public sealed class DenyAllFilterMiddleware : IFilterMiddleware
	{
		public bool AllowThrough(LogMessage message) => false;
	}
}