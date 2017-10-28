using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Container;
using NetCore.Standalone.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCore.Standalone.Lifecycle
{
	/// <summary>
	/// Installs ILifecycleServices
	/// </summary>
	public class LifecycleServiceInstaller : IInstallable
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public Action<IServiceProvider> Install(IServiceCollection container)
		{
			var types = ReflectionLoader.Types.Where(x => typeof(ILifecycleService).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

			foreach (var type in types)
			{
				container.AddSingleton(type);
				ConsoleExtensions.WriteLifecycleServiceRegister(type);
			}

			return null;
		}
	}
}
