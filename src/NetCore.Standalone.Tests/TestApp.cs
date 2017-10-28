using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCore.Standalone.Execution;
using System.Threading;
using System;
using NetCore.Standalone.Lifecycle;
using NetCore.Standalone.Container;

namespace NetCore.Standalone.Tests
{
	public class TestCancelerService : ILifecycleService
	{
		private IApplicationCanceler _applicationCanceller;
		private ITestSingletonService _testSingletonService;
		private ITestTransientService _testTransientService;
		private ITestScopedService _testScopedService;
		private Timer _timer;

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

		private void Callback(object state)
		{
			_applicationCanceller.Shutdown();
		}

		public async Task StartAsync()
		{
			_timer = new Timer(Callback, null, 5000, Timeout.Infinite);


			await Task.CompletedTask;
		}

		public async Task StopAsync()
		{

			await Task.CompletedTask;
		}
	}

	public class TestStartup : BaseStartup
	{
		public override async Task ConfigureAsync(IServiceCollection services, IApplicationBuilder app)
		{
			services.AddSingleton(app);
			await InvokeOnConfigureServicesAsync(services);

			Provider = services.BuildServiceProvider();

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
				.UseLifecycleService()
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
