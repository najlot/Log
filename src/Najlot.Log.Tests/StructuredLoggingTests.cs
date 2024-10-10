// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Tests.Mocks;
using System;
using Xunit;

namespace Najlot.Log.Tests;

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
		var output = "";

		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddCustomDestination(new DestinationMock(msg => output = msg.Message));

		var log = logAdministrator.GetLogger("default");

		log.Info("User {User} logon from {IP}", "admin", "127.0.0.1");
		Assert.Contains("User admin logon from 127.0.0.1", output);

		output = "";
		log.Info("User {User} logon. {User} comes from ip {IP}", "admin", "127.0.0.1");
		Assert.Contains("User admin logon. admin comes from ip 127.0.0.1", output);

		output = "";
		log.Info("All users logged out. Going into idle mode.");
		Assert.Contains("All users logged out. Going into idle mode.", output);

		output = "";
		log.Info("{count} users {{logged} out. Going into idle mode.", 10);
		Assert.Contains("10 users {logged} out. Going into idle mode.", output);

		output = "";
		log.Info("User {User} logon from {}", "admin", "127.0.0.1");
		Assert.Contains("User admin logon from 127.0.0.1", output);

		output = "";
		log.Info("User {User} logon from {ip}", "admin");
		Assert.Contains("User admin logon from ", output);

		output = "";
		log.Info("{{User} {User} logon", "admin");
		Assert.Contains("{User} admin logon", output);

		output = "";
		log.Info("User {{{User}} logon from {IP}", "admin", "127.0.0.1");
		Assert.Contains("User {admin} logon from 127.0.0.1", output);

		output = "";
		log.Info("ip {", "127.0.0.1");
		Assert.Contains("ip {", output);

		output = "";
		log.Info("ip: {ip} {", "127.0.0.1");
		Assert.Contains("ip: 127.0.0.1 {", output);

		output = "";
		log.Info("", 1, 2, 3);
		Assert.Contains(LogLevel.Info.ToString().ToUpper(), output);

		output = "";
		log.Info("{{", 1, 2, 3);
		Assert.Contains(LogLevel.Info.ToString().ToUpper(), output);
	}

	[Fact]
	public void StructuredLoggingMustFormatCorrect()
	{
		var output = "";

		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddCustomDestination(new DestinationMock(msg => output = msg.Message));

		var log = logAdministrator.GetLogger("default");

		log.Info("Number of users: {users:D3}", 50);
		Assert.Contains("Number of users: 050", output);

		output = "";
		log.Info("Three digits {num:D3} and five {num:D5}", 50);
		Assert.Contains("Three digits 050 and five 00050", output);

		output = "";
		log.Info("Three and five digits {num:D3}{num2:D5}", 50, 60);
		Assert.Contains("Three and five digits 05000060", output);
	}

	[Fact]
	public void NullShouldBeLoggedAsEmpty()
	{
		var output = "";

		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddCustomDestination(new DestinationMock(msg => output = msg.Message));

		var log = logAdministrator.GetLogger("default");

		// null as args -> nothing will be parsed
		log.Info("This is null: {var}.", null);
		Assert.Contains("This is null: {var}.", output);

		output = "";
		log.Info("This is null: {null}.", default(DestinationMock));
		Assert.Contains("This is null: .", output);

		output = "";
		log.Info("This is one {one} and this is null: {null}.", 1, null);
		Assert.Contains("This is one 1 and this is null: .", output);

		output = "";
		log.Info("This is one {one} and this is null: {null:D3}.", 1, null);
		Assert.Contains("This is one 1 and this is null: .", output);

		var now = DateTime.Now;
		var formattedNow = now.ToString("yyyy-MM-dd HH:mm:ss.fff");

		output = "";
		log.Info("This is a time: {time:yyyy-MM-dd HH:mm:ss.fff}", now);
		Assert.Contains($"This is a time: {formattedNow}", output);
	}

	[Fact]
	public void NullMessageShouldNotThrowExceptions()
	{
		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddCustomDestination(new DestinationMock(msg => _ = msg.Message));

		var log = logAdministrator.GetLogger("default");

		// log null variables must not produce exceptions
		log.Info(null);
		log.Info(default(string), 0);
	}

	[Fact]
	public void CachingShouldNotBreakFollowing()
	{
		string output;

		using var logAdministrator = LogAdministrator.CreateNew();
		logAdministrator.AddCustomDestination(new DestinationMock(msg => output = msg.Message));

		var log = logAdministrator.GetLogger("default");

		log.Info("User {user_id} writes Order {order_id} into DB.");

		output = "";
		log.Info("User {user_id} writes Order {order_id} into DB.", "U123");
		Assert.Contains("User U123 writes Order  into DB.", output);

		output = "";
		log.Info("User {user_id} writes Order {order_id} into DB.", "U123", 5243);
		Assert.Contains("User U123 writes Order 5243 into DB.", output);

		output = "";
		log.Info("User {user_id} writes Order {order_id} into DB.", null, null);
		Assert.Contains("User  writes Order  into DB.", output);

		output = "";
		log.Info("User {user_id} writes Order {order_id} into DB.", "U123");
		Assert.Contains("User U123 writes Order  into DB.", output);

		output = "";
		log.Info("User {user_id} writes Order {order_id} into DB.");
		Assert.Contains("User {user_id} writes Order {order_id} into DB.", output);

		output = "";
		log.Info("User {user_id} writes Order {order_id} into DB.", 123, 456);
		Assert.Contains("User 123 writes Order 456 into DB.", output);

		output = "";
		log.Info("User {user_id:D3} writes Order {order_id} into DB.", 1, 2);
		Assert.Contains("User 001 writes Order 2 into DB.", output);

		output = "";
		log.Info("User {user_id:D3} writes Order {order_id} into DB.", "db", 2);
		Assert.Contains("User db writes Order 2 into DB.", output);
	}
}