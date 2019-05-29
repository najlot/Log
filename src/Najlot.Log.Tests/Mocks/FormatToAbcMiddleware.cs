// Licensed under the MIT License. 
// See LICENSE file in the project root for full license information.

namespace Najlot.Log.Tests.Mocks
{
	[LogConfigurationName(nameof(FormatToAbcMiddleware))]
	public class FormatToAbcMiddleware : Middleware.IFormatMiddleware
	{
		public string Format(LogMessage message) => "Abc";
	}
}