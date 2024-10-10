// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Xunit;

namespace Najlot.Log.Tests;

public class CommonTests
{
	[Fact]
	public void LogAdministratorMustBeSingleton()
	{
		var instance = LogAdministrator.Instance;
		Assert.Same(instance, LogAdministrator.Instance);
	}
}