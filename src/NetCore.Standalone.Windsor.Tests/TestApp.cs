using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCore.Standalone.Execution;
using System.Threading;
using System;
using NetCore.Standalone.Windsor;
using NetCore.Standalone.AppServices;
using NetCore.Standalone.Injectable;

namespace NetCore.Standalone.Windsor.Tests
{
	[TestClass]
	public class TestApp
	{
		TimeSpan testCancelWait = TimeSpan.FromSeconds(1);

		[TestMethod]
		public async Task TestCancelToken()
		{
			var source = new CancellationTokenSource(testCancelWait);

			await Builder.New()
				.UseStartup<WindsorStartup>(options =>
				{
					options.UseCancellationToken(source);
				})
				.Build()
				.RunAsync();

		}
	}
}
