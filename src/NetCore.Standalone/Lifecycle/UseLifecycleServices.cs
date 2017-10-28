using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Execution;
using NetCore.Standalone.Extensions;
using NetCore.Standalone.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Standalone
{
	/// <summary>
	/// Extensions to enhance the IApplicationBuilder
	/// </summary>
	public static partial class ApplicationExtensions
	{
		/// <summary>
		/// Allow the loading of ILifecycleServices
		/// </summary>
		/// <param name="applicationBuilder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseLifecycleService(this IApplicationBuilder applicationBuilder)
		{
			return applicationBuilder.UseLifecycleService(options =>
			{
				options.FindAllWithInterface();
			});
		}

		/// <summary>
		/// Allow the loading of ILifecycleServices using the configured options
		/// </summary>
		/// <param name="applicationBuilder"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseLifecycleService(this IApplicationBuilder applicationBuilder, Action<LifecycleServiceOptions> options)
		{
			var lifecycleOptions = new LifecycleServiceOptions();
			options?.Invoke(lifecycleOptions);

			Func<IServiceCollection, Task> onConfigure = async (IServiceCollection collection) =>
			{
				collection.AddSingleton<ILifecycleServiceManager, LifecycleServiceManager>();
				await Task.CompletedTask;
			};

			Func<IServiceProvider, Task> onStart = async (IServiceProvider container) =>
			{
				var lifestyleManager = container.GetService<ILifecycleServiceManager>();
				await lifestyleManager.StartAllAsync(lifecycleOptions);
			};

			Func<IServiceProvider, Task> onStop = async (IServiceProvider container) =>
			{
				var lifestyleManager = container.GetService<ILifecycleServiceManager>();
				await lifestyleManager.StopAllAsync();
			};


			var config = new ConfiguredOption
			{
				OnBuild = null,
				OnConfigure = onConfigure,
				OnStart = onStart,
				OnStop = onStop
			};


			applicationBuilder.ConfiguredOptions.Add(config);

			return applicationBuilder;
		}

	}
}
