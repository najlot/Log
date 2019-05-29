// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(FormatTo123Middleware))]
	public class FormatTo123Middleware : Middleware.IFormatMiddleware
	{
		public string Format(LogMessage message) => "123";
	}
}