// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System.Collections.Generic;

namespace Najlot.Log.Util
{
	internal sealed class DestinationWrapper : IMiddleware
	{
		public IMiddleware NextMiddleware { get; set; }
		private readonly ILogDestination _destination;

		public DestinationWrapper(ILogDestination destination) => _destination = destination;

		public void Execute(IEnumerable<LogMessage> messages) => _destination.Log(messages);

		public void Flush()
		{
		}

		public void Dispose() => Flush();
	}
}