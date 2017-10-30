using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCore.Standalone.Execution;
using System.Threading;
using System;
using NetCore.Standalone.AppServices;
using NetCore.Standalone.Startup;
using NetCore.Standalone.Injectable;

namespace NetCore.Standalone.Tests
{
	public class TestCancelerService : AppService
	{
		private IApplicationCanceler _applicationCanceller;
		private ITestSingletonService _testSingletonService;
		private ITestTransientService _testTransientService;
		private ITestScopedService _testScopedService;

		public override bool WorkBlocks { get; protected set; } = false;
		protected override TimeSpan WorkInterval { get; set; } = TimeSpan.FromSeconds(10);

		public TestCancelerService(IApplicationCanceler applicationCanceler,
			ITestSingletonService testSingletonService,
			ITestTransientService testTransientService,
			ITestScopedService testScopedService)
		{
			_applicationCanceller = applicationCanceler;
			_testSingletonService = testSingletonService;
			_testTransientService = testTransientService;
			_testScopedService = testScopedService;
		}

		public override async Task OnStartAsync()
		{
			await Task.CompletedTask;
		}

		public override async Task OnStopAsync()
		{

			await Task.CompletedTask;
		}

		protected override async Task DoWorkAsync()
		{
			_applicationCanceller.Shutdown();
			await Task.CompletedTask;
		}

		protected override void HandleError(Exception ex)
		{

		}
	}

	public class TestStartup : BaseStartup
	{
		public override async Task CustomConfigureAsync(IServiceCollection services, IApplicationBuilder app)
		{
			await Task.CompletedTask;
		}
	}


	public interface ITestSingletonService : ISingletonInjectable { }
	public class TestSingletonService : ITestSingletonService { }

	public interface ITestTransientService : ITransientInjectable { }
	public class TestTransientService : ITestTransientService { }

	public interface ITestScopedService : IScopedInjectable { }
	public class TestScopedService : ITestScopedService { }



	[TestClass]
	public class TestApp
	{
		TimeSpan testCancelWait = TimeSpan.FromSeconds(1);

		[TestMethod]
		public async Task TestCancelToken()
		{
			var source = new CancellationTokenSource(testCancelWait);

			await Builder.New()
				.UseStartup<TestStartup>(options =>
				{
					options.UseCancellationToken(source);
				})
				.UseInstallables()
				.UseInjectables()
				.Build()
				.RunAsync();

		}


		[TestMethod]
		public async Task TestIApplicationCanceller()
		{
			await Builder.New()
				.UseStartup<TestStartup>(options =>
				{

				})
				.UseInstallables()
				.UseInjectables()
				.UseAppServices(options =>
				{
					options.UseOnlyTheseAppServices(typeof(TestCancelerService));
				})
				.Build()
				.RunAsync();

		}


		[TestMethod]
		public async Task TestStartStop()
		{
			var application = await Builder.New()
				.UseStartup<TestStartup>(options =>
				{

				})
				.UseInstallables()
				.UseInjectables()
				.Build()
				.StartAsync();

			await Task.Delay(testCancelWait);


			await application.StopAsync();

		}


		[TestMethod]
		public async Task TestStartCancelToken()
		{
			var source = new CancellationTokenSource();

			var application = await Builder.New()
				.UseStartup<TestStartup>(options =>
				{
					options.UseCancellationToken(source);
				})
				.UseInstallables()
				.UseInjectables()
				.Build()
				.StartAsync();

			source.Cancel();
			await Task.Delay(testCancelWait);
			Assert.IsFalse(application.Running);

		}
	}
}
