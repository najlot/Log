using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class StructuredLoggingTests
	{
		[Fact]
		public void StructuredLoggingTest()
		{
			var middleware = new DefaultFormatMiddleware();
			string output = "";
			int index = -1;

			using (var logAdminitrator = LogAdminitrator
				.CreateNew()
				.SetLogLevel(LogLevel.Trace)
				.SetExecutionMiddleware<SyncExecutionMiddleware>()
				.AddCustomDestination(new LogDestinationMock(msg =>
				{
					output = middleware.Format(msg);
				})))
			{
				var log = logAdminitrator.GetLogger("default");

				log.Info("User {User} logon from {IP}", "admin", "127.0.0.1");
				index = output.IndexOf("User admin logon from 127.0.0.1");
				Assert.True(index != -1, output);

				output = "";
				log.Info("User {User} logon. {User} comes from ip {IP}", "admin", "127.0.0.1");
				index = output.IndexOf("User admin logon. admin comes from ip 127.0.0.1");
				Assert.True(index != -1, output);

				output = "";
				log.Info("All users logged out. Going into idle mode.");
				index = output.IndexOf("All users logged out. Going into idle mode.");
				Assert.True(index != -1, output);

				output = "";
				log.Info("{count} users {{logged} out. Going into idle mode.", 10);
				index = output.IndexOf("10 users {logged} out. Going into idle mode.");
				Assert.True(index != -1, output);

				output = "";
				log.Info("User {User} logon from {}", "admin", "127.0.0.1");
				index = output.IndexOf("User admin logon from 127.0.0.1");
				Assert.True(index != -1, output);

				output = "";
				log.Info("User {User} logon from {ip}", "admin");
				index = output.IndexOf("User admin logon from {ip}");
				Assert.True(index != -1, output);

				output = "";
				log.Info("{{User} {User} logon", "admin");
				index = output.IndexOf("{User} admin logon");
				Assert.True(index != -1, output);

				output = "";
				log.Info("User {{{User}} logon from {IP}", "admin", "127.0.0.1");
				index = output.IndexOf("User {admin} logon from 127.0.0.1");
				Assert.True(index != -1, output);

				output = "";
				log.Info("ip {", "127.0.0.1");
				index = output.IndexOf("ip {");
				Assert.True(index != -1, output);

				output = "";
				log.Info("ip: {ip} {", "127.0.0.1");
				index = output.IndexOf("ip: 127.0.0.1 {");
				Assert.True(index != -1, output);
			}
		}

	}
}