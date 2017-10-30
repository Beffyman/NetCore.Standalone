using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetCore.Standalone.Startup;

namespace NetCore.Standalone.Windsor.Tests
{
	/// <summary>
	/// Implementation of a Windsor container inside the startup
	/// </summary>
	public class WindsorStartup : BaseStartup
	{
		/// <summary>
		/// Container for the application.
		/// </summary>
		protected IContainer Container { get; set; }

		/// <summary>
		/// Custom configure options
		/// </summary>
		/// <param name="services"></param>
		/// <param name="app"></param>
		/// <returns></returns>
		public override async Task CustomConfigureAsync(IServiceCollection services, IApplicationBuilder app)
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Builds the provider, can be overridden
		/// </summary>
		/// <param name="services"></param>
		/// <param name="app"></param>
		/// <returns></returns>
		protected override IServiceProvider CreateProvider(IServiceCollection services, IApplicationBuilder app)
		{
			Container = new WindsorStandaloneContainer();
			return Container.Create(services);
		}
	}
}
