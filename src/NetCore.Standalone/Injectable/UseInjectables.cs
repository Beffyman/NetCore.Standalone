using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Execution;
using NetCore.Standalone.Extensions;
using NetCore.Standalone.Injectable;
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
		internal static void Register<InjectionType>(IServiceCollection container, Type interfaceType, IList<Type> lifestypeTypes) where InjectionType : IInjectable
		{
			var implementationTypes = lifestypeTypes.Where(x => x.IsClass && interfaceType.IsAssignableFrom(x)).ToList();
			if (implementationTypes.Count > 1)
			{
				ConsoleExtensions.WriteError($"There are {implementationTypes.Count} implementers of {interfaceType.Name}. Please manually define the registration.");
				return;
			}

			var implementationType = implementationTypes.SingleOrDefault();
			if (implementationType == null)
			{
				throw new Exception($"{interfaceType.Name} does not have a class that implements it.");
			}


			if (typeof(InjectionType) == typeof(ISingletonInjectable))
			{
				container.AddSingleton(interfaceType, implementationType);
				ConsoleExtensions.WriteServiceRegister("Singleton", interfaceType, implementationType);
			}
			else if (typeof(InjectionType) == typeof(ITransientInjectable))
			{
				container.AddTransient(interfaceType, implementationType);
				ConsoleExtensions.WriteServiceRegister("Transient", interfaceType, implementationType);
			}
			else if (typeof(InjectionType) == typeof(IScopedInjectable))
			{
				container.AddScoped(interfaceType, implementationType);
				ConsoleExtensions.WriteServiceRegister("Scoped", interfaceType, implementationType);
			}
			else
			{
				throw new Exception($"Unsupported {nameof(IInjectable)} type.");
			}
		}

		/// <summary>
		/// Configures the builder to lookup all IInjectable and IInstallable classes so they are auto-registered
		/// </summary>
		/// <param name="applicationBuilder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseInjectables(this IApplicationBuilder applicationBuilder)
		{
			Func<IServiceCollection, Task> onConfigure = async (IServiceCollection collection) =>
			{
				var types = ReflectionLoader.Types.Where(x => typeof(IInjectable).IsAssignableFrom(x)
																							&& x != typeof(ISingletonInjectable)
																							&& x != typeof(ITransientInjectable)
																							&& x != typeof(IScopedInjectable)).ToList();

				var singletons = types.Where(x => x.IsInterface && typeof(ISingletonInjectable).IsAssignableFrom(x)).ToList();
				var transients = types.Where(x => x.IsInterface && typeof(ITransientInjectable).IsAssignableFrom(x)).ToList();
				var scoped = types.Where(x => x.IsInterface && typeof(IScopedInjectable).IsAssignableFrom(x)).ToList();

				foreach (var type in singletons)
				{
					Register<ISingletonInjectable>(collection, type, types);
				}

				foreach (var type in transients)
				{
					Register<ITransientInjectable>(collection, type, types);
				}

				foreach (var type in scoped)
				{
					Register<IScopedInjectable>(collection, type, types);
				}

				await Task.CompletedTask;
			};

			var config = new ConfiguredOption("Injectables")
			{
				OnBuild = null,
				OnConfigure = onConfigure,
				OnStart = null,
				OnStop = null
			};


			applicationBuilder.ConfiguredOptions.Add(config);

			return applicationBuilder;
		}

	}
}
