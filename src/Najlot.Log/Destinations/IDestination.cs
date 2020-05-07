// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Najlot.Log.Destinations
{
	/// <summary>
	/// Common interface for all destinations
	/// </summary>
	public interface IDestination : IDisposable
	{
		/// <summary>
		/// Tells the destination to log the messages
		/// </summary>
		/// <param name="messages">Messages to be logged</param>
		void Log(IEnumerable<LogMessage> messages);
	}
}