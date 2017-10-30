using NetCore.Standalone.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NetCore.Standalone.AppServices
{
	/// <summary>
	/// Singleton manager for the services that start and stop with the application
	/// </summary>
	public interface IAppServiceManager
	{
		/// <summary>
		/// Resolves the IAppService T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Get<T>() where T : AppService;

		/// <summary>
		/// Resolves the IAppService from the type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		AppService Get(Type type);

		/// <summary>
		/// Starts all IAppServices
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		Task StartAllAsync(AppServiceManagerOptions options);

		/// <summary>
		/// Stops all IAppServices
		/// </summary>
		/// <returns></returns>
		Task StopAllAsync();
	}


	internal class AppServiceManager : IAppServiceManager
	{
		protected IList<AppService> Services { get; set; }
		protected IServiceProvider Container { get; set; }

		public AppServiceManager(IServiceProvider container)
		{
			Container = container;
			Services = new List<AppService>();
		}

		/// <summary>
		/// Returns a type and adds it into the list, but does not start it, that is expected of you.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Get<T>() where T : AppService
		{
			var existing = Services.SingleOrDefault(x => x.GetType() == typeof(T));
			if (existing != null)
			{
				return (T)existing;
			}
			else
			{
				var instance = Container.GetService<T>();
				Services.Add(instance);
				return instance;
			}
		}

		public AppService Get(Type type)
		{
			var existing = Services.SingleOrDefault(x => x.GetType() == type);
			if (existing != null)
			{
				return existing;
			}
			else
			{
				var instance = Container.GetService(type);
				if (instance is AppService lcserv)
				{
					Services.Add(lcserv);
					return lcserv;
				}
				else
				{
					throw new Exception($"Type {type.FullName} is not a {nameof(AppService)}");
				}
			}
		}

		public async Task StartAllAsync(AppServiceManagerOptions options)
		{
			Type[] types = null;

			if (options._appServices != null && options._appServices.Length != 0)
			{
				types = options._appServices;
			}
			else if (options._findAll)
			{
				types = ReflectionLoader.Types.Where(x => typeof(AppService).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToArray();
			}
			else
			{
				return;
			}

			//Order by attribute, and if none,
			types = types.ToLookup(x => x?.GetCustomAttribute<AppServiceOrderAttribute>()?.Order ?? int.MaxValue, y => y)
						.OrderBy(x => x.Key)
						.SelectMany(x => x)
						.ToArray();

			foreach (var type in types)
			{
				if (Services.Any(x => type.IsAssignableFrom(x.GetType())))//This means the type was pre-called
				{
					continue;
				}

				var instance = Get(type);
				await instance.StartAsync();
				Console.WriteLine($"{type.FullName} has started!");
			}
		}

		public async Task StopAllAsync()
		{
			foreach (var instance in Services)
			{
				await instance.StopAsync();
				Console.WriteLine($"{instance.GetType().FullName} has stopped!");
			}
		}

	}
}
