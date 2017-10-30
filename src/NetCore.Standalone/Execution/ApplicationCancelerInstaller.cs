using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Installable;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Standalone.Execution
{
	/// <summary>
	/// Installer for IApplicationCanceller
	/// </summary>
	public class ApplicationCancelerInstaller : IInstallable
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		public Action<IServiceProvider> Install(IServiceCollection container)
		{
			container.AddSingleton<IApplicationCanceler, ApplicationCanceler>();

			return null;
		}
	}
}
