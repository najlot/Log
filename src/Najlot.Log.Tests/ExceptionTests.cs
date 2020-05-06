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
			bool gotError = false;

			void ExecutionErrorOccured(object sender, LogErrorEventArgs e)
			{
				gotError = true;
			}

			LogErrorHandler.Instance.ErrorOccured += ExecutionErrorOccured;

			using (var logAdminitrator = LogAdministrator.CreateNew())
			{
				logAdminitrator.AddCustomDestination(new LogDestinationMock((message) => { }));
				logAdminitrator.AddMiddleware<ExecuteExceptionThrowingMiddleware, LogDestinationMock>();

				Assert.False(gotError);
				logAdminitrator.GetLogger(GetType()).Error("");
				Assert.True(gotError);
			}

			LogErrorHandler.Instance.ErrorOccured -= ExecutionErrorOccured;
		}

		[Fact]
		public void MiddlewareCreationExceptionsShouldGoToErrorHandler()
		{
			bool gotError = false;

			void CreationErrorOccured(object sender, LogErrorEventArgs e)
			{
				gotError = true;
			}

			LogErrorHandler.Instance.ErrorOccured += CreationErrorOccured;

			using (var logAdminitrator = LogAdministrator.CreateNew())
			{
				logAdminitrator.AddCustomDestination(new LogDestinationMock((message) => { }));
				Assert.False(gotError);
				logAdminitrator.AddMiddleware<CtorExceptionThrowingMiddleware, LogDestinationMock>();
				Assert.True(gotError);
			}

			LogErrorHandler.Instance.ErrorOccured -= CreationErrorOccured;
		}

		[Fact]
		public void DestinationExceptionsShouldGoToErrorHandler()
		{
			bool gotError = false;

			void DestinationErrorOccured(object sender, LogErrorEventArgs e)
			{
				gotError = true;
			}

			LogErrorHandler.Instance.ErrorOccured += DestinationErrorOccured;

			using (var logAdminitrator = LogAdministrator.CreateNew())
			{
				logAdminitrator.AddCustomDestination(new LogDestinationMock((message) => throw new System.Exception()));

				Assert.False(gotError);
				logAdminitrator.GetLogger(GetType()).Info("");
				Assert.True(gotError);
			}

			LogErrorHandler.Instance.ErrorOccured -= DestinationErrorOccured;
		}
	}
}