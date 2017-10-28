using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Standalone.Execution
{
	/// <summary>
	/// Options to configure application initialization
	/// </summary>
	public class ConfiguredOption
	{
		/// <summary>
		/// 1st Step to be ran
		/// </summary>
		public Func<IApplicationBuilder, Task> OnBuild { get; set; }
		/// <summary>
		/// 2nd Step to be ran
		/// </summary>
		public Func<IServiceCollection, Task> OnConfigure { get; set; }
		/// <summary>
		/// 3rd Step to be ran
		/// </summary>
		public Func<IServiceProvider, Task> OnStart { get; set; }
		/// <summary>
		/// 4th Step to be ran
		/// </summary>
		public Func<IServiceProvider, Task> OnStop { get; set; }

	}
}
