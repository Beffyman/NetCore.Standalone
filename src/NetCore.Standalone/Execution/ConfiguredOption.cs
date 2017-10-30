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
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public ConfiguredOption(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Name of the configuration
		/// </summary>
		public string Name { get; set; }

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

		/// <summary>
		/// Returns name
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Are two configuration options equal?
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is ConfiguredOption co)
			{
				return this.Name.Equals(co.Name, StringComparison.CurrentCultureIgnoreCase);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gets hash code of the object
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}
