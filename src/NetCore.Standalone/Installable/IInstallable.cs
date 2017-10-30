using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Standalone.Installable
{
	/// <summary>
	/// This class is used as a locator for installable services.
	/// </summary>
	public interface IInstallable
	{
		/// <summary>
		/// Installation method that will register services then use the callback before the application starts
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		Action<IServiceProvider> Install(IServiceCollection container);
	}
}
