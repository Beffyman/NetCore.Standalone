using NetCore.Standalone.Container;
using NetCore.Standalone.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore.Standalone.Lifecycle
{
	/// <summary>
	/// Singleton manager for the services that start and stop with the application
	/// </summary>
	public interface ILifecycleServiceManager
	{
		/// <summary>
		/// Resolves the ILifecycleService T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Get<T>() where T : ILifecycleService;

		/// <summary>
		/// Resolves the ILifecycleService from the type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ILifecycleService Get(Type type);

		/// <summary>
		/// Starts all ILifecycleServices
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		Task StartAllAsync(LifecycleServiceOptions options);

		/// <summary>
		/// Stops all ILifecycleServices
		/// </summary>
		/// <returns></returns>
		Task StopAllAsync();
	}


	internal class LifecycleServiceManager : ILifecycleServiceManager
	{
		protected IList<ILifecycleService> Services { get; set; }
		protected IServiceProvider Container { get; set; }

		public LifecycleServiceManager(IServiceProvider container)
		{
			Container = container;
			Services = new List<ILifecycleService>();
		}

		/// <summary>
		/// Returns a type and adds it into the list, but does not start it, that is expected of you.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Get<T>() where T : ILifecycleService
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

		public ILifecycleService Get(Type type)
		{
			var existing = Services.SingleOrDefault(x => x.GetType() == type);
			if (existing != null)
			{
				return existing;
			}
			else
			{
				var instance = Container.GetService(type);
				if (instance is ILifecycleService lcserv)
				{
					Services.Add(lcserv);
					return lcserv;
				}
				else
				{
					throw new Exception($"Type {type.FullName} is not a {nameof(ILifecycleService)}");
				}
			}
		}

		public async Task StartAllAsync(LifecycleServiceOptions options)
		{
			Type[] types = null;

			if (options._lifecycleServices != null && options._lifecycleServices.Length != 0)
			{
				types = options._lifecycleServices;
			}
			else if (options._findAll)
			{
				types = ReflectionLoader.Types.Where(x => typeof(ILifecycleService).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToArray();
			}
			else
			{
				return;
			}


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
