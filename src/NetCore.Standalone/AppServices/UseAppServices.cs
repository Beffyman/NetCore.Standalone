using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Execution;
using NetCore.Standalone.Extensions;
using NetCore.Standalone.AppServices;
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
		/// Allow the loading of IAppServices
		/// </summary>
		/// <param name="applicationBuilder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseAppServices(this IApplicationBuilder applicationBuilder)
		{
			return applicationBuilder.UseAppServices(options =>
			{
				options.FindAllWithInterface();
			});
		}

		/// <summary>
		/// Allow the loading of IAppServices using the configured options
		/// </summary>
		/// <param name="applicationBuilder"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseAppServices(this IApplicationBuilder applicationBuilder, Action<AppServiceManagerOptions> options)
		{
			var appServicesOptions = new AppServiceManagerOptions();
			options?.Invoke(appServicesOptions);

			Func<IServiceCollection, Task> onConfigure = async (IServiceCollection collection) =>
			{
				collection.AddSingleton<IAppServiceManager, AppServiceManager>();
				await Task.CompletedTask;
			};

			Func<IServiceProvider, Task> onStart = async (IServiceProvider container) =>
			{
				var lifestyleManager = container.GetService<IAppServiceManager>();
				await lifestyleManager.StartAllAsync(appServicesOptions);
			};

			Func<IServiceProvider, Task> onStop = async (IServiceProvider container) =>
			{
				var lifestyleManager = container.GetService<IAppServiceManager>();
				await lifestyleManager.StopAllAsync();
			};


			var config = new ConfiguredOption("AppServices")
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
