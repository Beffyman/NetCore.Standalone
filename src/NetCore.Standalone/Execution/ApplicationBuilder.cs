using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetCore.Standalone.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Startup;

namespace NetCore.Standalone.Execution
{
	/// <summary>
	/// Configurable application builder
	/// </summary>
	public interface IApplicationBuilder
	{
		/// <summary>
		/// Startup object of the application
		/// </summary>
		BaseStartup Startup { get; set; }
		/// <summary>
		/// Token used to cancel the application once it starts
		/// </summary>
		CancellationTokenSource Cancellation { get; set; }
		/// <summary>
		/// Options that can be configured that will modify application initialization
		/// </summary>
		ICollection<ConfiguredOption> ConfiguredOptions { get; }

		/// <summary>
		/// Builds and Configures the application
		/// </summary>
		/// <returns></returns>
		IApplication Build();

	}

	/// <summary>
	/// Default ApplicationBuilder
	/// </summary>
	public static class Builder
	{
		/// <summary>
		/// Creates a new ApplicationBuilder
		/// </summary>
		/// <returns></returns>
		public static IApplicationBuilder New()
		{
			return new ApplicationBuilder();
		}
	}


	internal sealed class ApplicationBuilder : IApplicationBuilder
	{
		public BaseStartup Startup { get; set; }
		public CancellationTokenSource Cancellation { get; set; }
		public ICollection<ConfiguredOption> ConfiguredOptions { get; private set; }

		public ApplicationBuilder()
		{
			ConfiguredOptions = new HashSet<ConfiguredOption>();
		}

		/// <summary>
		/// Builds the application through the configuration methods.
		/// </summary>
		/// <returns></returns>
		public IApplication Build()
		{
			IApplicationBuilder app = this;

			if (app.Startup == null)
			{
				throw new Exception("Startup has not been defined.  Please use UseStartup to define a startup object.");
			}

			//Add all of the configure options to the startup
			foreach (var option in ConfiguredOptions)
			{
				app.Startup.OnConfigureServicesAsync += option.OnConfigure;
				app.Startup.OnStartAsync += option.OnStart;
				app.Startup.OnShutdownAsync += option.OnStop;
			}

			//Execute the OnBuild functions, which will in turn execute the OnConfigure since Startup is within here(else we throw an error saying that startup is required).
			foreach (var option in ConfiguredOptions)
			{
				if (option.OnBuild != null)
				{
					option.OnBuild?.Invoke(app).GetAwaiter().GetResult();
				}
			}

			return new Application(this);
		}




	}
}
