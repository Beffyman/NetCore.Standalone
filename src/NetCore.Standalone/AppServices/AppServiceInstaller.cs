using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Extensions;
using NetCore.Standalone.Installable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCore.Standalone.AppServices
{
	/// <summary>
	/// Installs IAppService
	/// </summary>
	public class AppServiceInstaller : IInstallable
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public Action<IServiceProvider> Install(IServiceCollection container)
		{
			var types = ReflectionLoader.Types.Where(x => typeof(AppService).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

			foreach (var type in types)
			{
				container.AddSingleton(type);
				ConsoleExtensions.WriteAppServiceRegister(type);
			}

			return null;
		}
	}
}
