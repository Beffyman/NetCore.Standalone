using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Standalone.Startup
{
	/// <summary>
	/// Base startup implementation for applications
	/// </summary>
	public abstract class BaseStartup
	{
		/// <summary>
		/// Start events that will be fired during StartAsync
		/// </summary>
		public event Func<IServiceProvider, Task> OnStartAsync;

		/// <summary>
		/// Shutdown events that will be fired during ShutdownAsync
		/// </summary>
		public event Func<IServiceProvider, Task> OnShutdownAsync;

		/// <summary>
		/// Configuration events that will be fired during ConfigureAsync
		/// </summary>
		public event Func<IServiceCollection, Task> OnConfigureServicesAsync;

		/// <summary>
		/// Arguments provided to the application on start
		/// </summary>
		public string[] Arguments { get; set; }


		/// <summary>
		/// Invokes the configuration events for services
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		protected async Task InvokeOnConfigureServicesAsync(IServiceCollection collection)
		{
			if (OnConfigureServicesAsync != null)
			{
				await OnConfigureServicesAsync(collection);
			}
		}

		/// <summary>
		/// Service Provider for the application
		/// </summary>
		protected IServiceProvider Provider { get; set; }

		/// <summary>
		/// Custom configure options
		/// </summary>
		/// <param name="services"></param>
		/// <param name="app"></param>
		/// <returns></returns>
		public abstract Task CustomConfigureAsync(IServiceCollection services, IApplicationBuilder app);

		/// <summary>
		/// Configures the services
		/// </summary>
		/// <param name="services"></param>
		/// <param name="app"></param>
		/// <returns></returns>
		public async Task ConfigureAsync(IServiceCollection services, IApplicationBuilder app)
		{
			services.AddSingleton(app);
			await InvokeOnConfigureServicesAsync(services);


			await CustomConfigureAsync(services, app);

			Provider = services.BuildServiceProvider();
		}

		/// <summary>
		/// Starts the lifecycle services.
		/// </summary>
		/// <returns></returns>
		public async Task StartAsync()
		{
			if (OnStartAsync != null)
			{
				await OnStartAsync(Provider);
			}
		}

		/// <summary>
		/// Stops the lifecycle services.
		/// </summary>
		/// <returns></returns>
		public async Task ShutdownAsync()
		{
			if (OnShutdownAsync != null)
			{
				await OnShutdownAsync(Provider);
			}
		}
	}
}
