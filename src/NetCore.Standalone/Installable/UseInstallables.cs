using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Execution;
using NetCore.Standalone.Extensions;
using NetCore.Standalone.Installable;
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
		/// Configures the builder to lookup all IInjectable and IInstallable classes so they are auto-registered
		/// </summary>
		/// <param name="applicationBuilder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseInstallables(this IApplicationBuilder applicationBuilder)
		{

			var actions = new List<Action<IServiceProvider>>();


			Func<IServiceCollection, Task> onConfigure = async (IServiceCollection collection) =>
			{
				var types = ReflectionLoader.Types.Where(x => typeof(IInstallable).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

				foreach (var type in types)
				{
					var installable = Activator.CreateInstance(type) as IInstallable;
					actions.Add(installable.Install(collection));
				}

				await Task.CompletedTask;
			};

			Func<IServiceProvider, Task> onStart = async (IServiceProvider container) =>
			{
				foreach (var action in actions.Where(x => x != null))
				{
					action(container);
				}

				await Task.CompletedTask;
			};

			var config = new ConfiguredOption("Installables")
			{
				OnBuild = null,
				OnConfigure = onConfigure,
				OnStart = onStart,
				OnStop = null
			};


			applicationBuilder.ConfiguredOptions.Add(config);

			return applicationBuilder;
		}

	}
}
