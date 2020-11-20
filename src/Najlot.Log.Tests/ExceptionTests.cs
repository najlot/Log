// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Najlot.Log.Tests.Mocks;
using Xunit;

namespace Najlot.Log.Tests
{
	public class ExceptionTests
	{
		public ExceptionTests()
		{
			foreach (var type in GetType().Assembly.GetTypes())
			{
				if (type.GetCustomAttributes(typeof(LogConfigurationNameAttribute), true).Length > 0)
				{
					LogConfigurationMapper.Instance.AddToMapping(type);
				}
			}
		}

		[Fact]
		public void MiddlewareExecutionExceptionsShouldGoToErrorHandler()
		{
			var gotError = false;

			void ExecutionErrorOccured(object sender, LogErrorEventArgs e)
			{
				gotError = true;
			}

			LogErrorHandler.Instance.ErrorOccured += ExecutionErrorOccured;

			using (var logAdministrator = LogAdministrator.CreateNew())
			{
				logAdministrator.AddCustomDestination(new DestinationMock((message) => { }));
				logAdministrator.AddMiddleware<ExecuteExceptionThrowingMiddleware, DestinationMock>();

				Assert.False(gotError);
				logAdministrator.GetLogger(GetType()).Error("");
				Assert.True(gotError);
			}

			LogErrorHandler.Instance.ErrorOccured -= ExecutionErrorOccured;
		}

		[Fact]
		public void MiddlewareCreationExceptionsShouldGoToErrorHandler()
		{
			var gotError = false;

			void CreationErrorOccured(object sender, LogErrorEventArgs e)
			{
				gotError = true;
			}

			LogErrorHandler.Instance.ErrorOccured += CreationErrorOccured;

			using (var logAdministrator = LogAdministrator.CreateNew())
			{
				logAdministrator.AddCustomDestination(new DestinationMock((message) => { }));
				Assert.False(gotError);
				logAdministrator.AddMiddleware<CtorExceptionThrowingMiddleware, DestinationMock>();
				Assert.True(gotError);
			}

			LogErrorHandler.Instance.ErrorOccured -= CreationErrorOccured;
		}

		[Fact]
		public void DestinationExceptionsShouldGoToErrorHandler()
		{
			var gotError = false;

			void DestinationErrorOccured(object sender, LogErrorEventArgs e)
			{
				gotError = true;
			}

			LogErrorHandler.Instance.ErrorOccured += DestinationErrorOccured;

			try
			{
				using var logAdministrator = LogAdministrator.CreateNew();
				logAdministrator.AddCustomDestination(new DestinationMock((message) => throw new System.Exception()));

				Assert.False(gotError);
				logAdministrator.GetLogger(GetType()).Info("");
				Assert.True(gotError);
			}
			finally
			{
				LogErrorHandler.Instance.ErrorOccured -= DestinationErrorOccured;
			}
		}
	}
}