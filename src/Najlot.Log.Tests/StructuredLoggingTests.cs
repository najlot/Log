// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests
{
	public class StructuredLoggingTests
	{
		public StructuredLoggingTests()
		{
			foreach (var type in typeof(StructuredLoggingTests).Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void StructuredLoggingMustParseCorrect()
		{
			string output = "";
			int index = -1;

			using var logAdminitrator = LogAdministrator.CreateNew();
			logAdminitrator.AddCustomDestination(new DestinationMock(msg => output = msg.Message));

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
			index = output.IndexOf("User admin logon from ");
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

			output = "";
			log.Info("", 1, 2, 3);
			index = output.IndexOf(LogLevel.Info.ToString().ToUpper());
			Assert.True(index != -1, output);

			output = "";
			log.Info("{{", 1, 2, 3);
			index = output.IndexOf(LogLevel.Info.ToString().ToUpper());
			Assert.True(index != -1, output);
		}

		[Fact]
		public void StructuredLoggingMustFormatCorrect()
		{
			string output = "";
			int index = -1;

			using var logAdminitrator = LogAdministrator.CreateNew();
			logAdminitrator.AddCustomDestination(new DestinationMock(msg => output = msg.Message));

			var log = logAdminitrator.GetLogger("default");

			log.Info("Number of users: {users:D3}", 50);
			index = output.IndexOf("Number of users: 050");
			Assert.True(index != -1, output);

			output = "";
			log.Info("Three digits {num:D3} and five {num:D5}", 50);
			index = output.IndexOf("Three digits 050 and five 00050");
			Assert.True(index != -1, output);

			output = "";
			log.Info("Three and five digits {num:D3}{num2:D5}", 50, 60);
			index = output.IndexOf("Three and five digits 05000060");
			Assert.True(index != -1, output);
		}

		[Fact]
		public void NullShouldBeLoggedAsEmpty()
		{
			string output = "";
			int index = -1;

			using var logAdminitrator = LogAdministrator.CreateNew();
			logAdminitrator.AddCustomDestination(new DestinationMock(msg => output = msg.Message));

			var log = logAdminitrator.GetLogger("default");

			// null as args -> nothing will be parsed
			log.Info("This is null: {var}.", null);
			index = output.IndexOf("This is null: {var}.");
			Assert.True(index != -1, output);

			output = "";
			log.Info("This is null: {null}.", default(DestinationMock));
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

		[Fact]
		public void NullMessageShouldNotThrowExceptions()
		{
			using var logAdminitrator = LogAdministrator.CreateNew();
			logAdminitrator.AddCustomDestination(new DestinationMock(msg => _ = msg.Message));

			var log = logAdminitrator.GetLogger("default");

			// log null variables must not produce exceptions
			log.Info(null);
			log.Info(default(string), 0);
		}

		[Fact]
		public void CachingShouldNotBreakFollowing()
		{
			string output;
			int index;

			using var logAdminitrator = LogAdministrator.CreateNew();
			logAdminitrator.AddCustomDestination(new DestinationMock(msg => output = msg.Message));

			var log = logAdminitrator.GetLogger("default");

			log.Info("User {user_id} writes Order {order_id} into DB.");

			output = "";
			log.Info("User {user_id} writes Order {order_id} into DB.", "U123");
			index = output.IndexOf("User U123 writes Order  into DB.");
			Assert.True(index != -1, output);

			output = "";
			log.Info("User {user_id} writes Order {order_id} into DB.", "U123", 5243);
			index = output.IndexOf("User U123 writes Order 5243 into DB.");
			Assert.True(index != -1, output);

			output = "";
			log.Info("User {user_id} writes Order {order_id} into DB.", null, null);
			index = output.IndexOf("User  writes Order  into DB.");
			Assert.True(index != -1, output);

			output = "";
			log.Info("User {user_id} writes Order {order_id} into DB.", "U123");
			index = output.IndexOf("User U123 writes Order  into DB.");
			Assert.True(index != -1, output);

			output = "";
			log.Info("User {user_id} writes Order {order_id} into DB.");
			index = output.IndexOf("User {user_id} writes Order {order_id} into DB.");
			Assert.True(index != -1, output);

			output = "";
			log.Info("User {user_id} writes Order {order_id} into DB.", 123, 456);
			index = output.IndexOf("User 123 writes Order 456 into DB.");
			Assert.True(index != -1, output);

			output = "";
			log.Info("User {user_id:D3} writes Order {order_id} into DB.", 1, 2);
			index = output.IndexOf("User 001 writes Order 2 into DB.");
			Assert.True(index != -1, output);

			output = "";
			log.Info("User {user_id:D3} writes Order {order_id} into DB.", "db", 2);
			index = output.IndexOf("User db writes Order 2 into DB.");
			Assert.True(index != -1, output);
		}
	}
}