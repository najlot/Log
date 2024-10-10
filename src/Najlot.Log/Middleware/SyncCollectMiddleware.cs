// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace Najlot.Log.Middleware;

/// <summary>
/// Middleware, that collects messages from different threads
/// and passes them further to the next middleware.
/// Caution: The calls block until finished and allow only one execution at a time (using lock)
/// </summary>
[LogConfigurationName(nameof(SyncCollectMiddleware))]
public sealed class SyncCollectMiddleware : ICollectMiddleware
{
	public IMiddleware NextMiddleware { get; set; }

	private readonly LogMessage[] _msgArr = new LogMessage[1];

	public void Execute(LogMessage message)
	{
		lock (_msgArr)
		{
			_msgArr[0] = message;
			NextMiddleware.Execute(_msgArr);
		}
	}

	public void Flush()
	{
		// Nothing to flush
	}

	public void Dispose() => Flush();
}