using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Execution;
using NetCore.Standalone.Extensions;
using NetCore.Standalone.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore.Standalone
{
	/// <summary>
	/// Extensions to enhance the IApplicationBuilder
	/// </summary>
	public static partial class ApplicationExtensions
	{
		/// <summary>
		/// Use the startup class specified
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="applicationBuilder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseStartup<T>(this IApplicationBuilder applicationBuilder) where T : BaseStartup, new()
		{
			return UseStartup<T>(applicationBuilder, null);
		}

		/// <summary>
		/// Use the startup class specified with the options
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="applicationBuilder"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseStartup<T>(this IApplicationBuilder applicationBuilder, Action<ApplicationStartupOptions> options) where T : BaseStartup, new()
		{
			var startupOptions = new ApplicationStartupOptions();
			options?.Invoke(startupOptions);

			BaseStartup startup = Activator.CreateInstance<T>();
			applicationBuilder.Startup = startup;
			applicationBuilder.Cancellation = startupOptions._cancellationToken;


			Func<IApplicationBuilder, Task> onBuild = async (IApplicationBuilder appBuilder) =>
			{
				IServiceCollection serviceCollection = new ServiceCollection();

				await startup.ConfigureAsync(serviceCollection, applicationBuilder);
			};

			var configureOptions = new ConfiguredOption
			{
				OnBuild = onBuild,
				OnConfigure = null,
				OnStart = null,
				OnStop = null
			};

			applicationBuilder.ConfiguredOptions.Add(configureOptions);

			return applicationBuilder;
		}
	}
}
