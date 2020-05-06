// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;

namespace Najlot.Log.Middleware
{
	public interface ICollectMiddleware : IDisposable
	{
		IMiddleware NextMiddleware { get; set; }

		void Execute(LogMessage message);

		void Flush();
	}
}