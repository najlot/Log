using Najlot.Log.Middleware;
using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class StructuredLoggingTests
	{
		[Fact]
		public void StructuredLoggingMustParseCorrect()
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

		[Fact]
		public void StructuredLoggingMustFormatCorrect()
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

				log.Info("Number of users: {users:D3}", 50);
				index = output.IndexOf("Number of users: 050");
				Assert.True(index != -1, output);

				output = "";
				log.Info("Three digits {num:D3} and five {num:D5}", 50);
				index = output.IndexOf("Three digits 050 and five 00050");
				Assert.True(index != -1, output);
			}
		}

		[Fact]
		public void NullShouldBeLoggedAsEmpty()
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
				
				// null as args -> nothing will be parsed
				log.Info("This is null: {var}.", null);
				index = output.IndexOf("This is null: {var}.");
				Assert.True(index != -1, output);

				output = "";
				log.Info("This is null: {null}.", default(LogDestinationMock));
				index = output.IndexOf("This is null: .");
				Assert.True(index != -1, output);

				output = "";
				log.Info("This is one {one} and this is null: {null}.", 1, null);
				index = output.IndexOf("This is one 1 and this is null: .");
				Assert.True(index != -1, output);

				output = "";
				log.Info("This is one {one} and this is null: {null:D3}.", 1, null);
				index = output.IndexOf("This is one 1 and this is null: .");
				Assert.True(index != -1, output);

				var now = DateTime.Now;
				var formattedNow = now.ToString("yyyy-MM-dd HH:mm:ss.fff");

				output = "";
				log.Info("This is a time: {time:yyyy-MM-dd HH:mm:ss.fff}", now);
				index = output.IndexOf($"This is a time: {formattedNow}");
				Assert.True(index != -1, output);
			}
		}
	}
}