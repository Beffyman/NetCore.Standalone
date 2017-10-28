using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Container;
using NetCore.Standalone.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Standalone.Execution
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
		/// Configures the container and required services implemented by ConfigureServices. If you want a custom container, override this.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="app"></param>
		/// <returns></returns>
		public abstract Task ConfigureAsync(IServiceCollection services, IApplicationBuilder app);

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
