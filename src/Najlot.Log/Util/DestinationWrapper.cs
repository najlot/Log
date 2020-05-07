// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using System.Collections.Generic;

namespace Najlot.Log.Util
{
	/// <summary>
	/// Class used for passing data from IMiddleware to IDestination
	/// </summary>
	internal sealed class DestinationWrapper : IMiddleware
	{
		public IMiddleware NextMiddleware { get; set; }
		private readonly IDestination _destination;

		public DestinationWrapper(IDestination destination) => _destination = destination;

		public void Execute(IEnumerable<LogMessage> messages) => _destination.Log(messages);

		public void Flush()
		{
			// NextMiddleware will be null and IDestination has no flush method
		}

		public void Dispose() => Flush();
	}
}