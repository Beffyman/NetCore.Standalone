using NetCore.Standalone.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCore.Standalone.AppServices
{
	/// <summary>
	/// Options for the configuration of IAppServices
	/// </summary>
	public sealed class AppServiceManagerOptions
	{

		internal Type[] _appServices;
		internal bool _findAll;

		internal AppServiceManagerOptions()
		{

		}

		/// <summary>
		/// Only the types provided will be loaded
		/// </summary>
		/// <param name="appServices"></param>
		public void UseOnlyTheseAppServices(params Type[] appServices)
		{
			_appServices = appServices.Where(x => x != null && typeof(AppService).IsAssignableFrom(x)).ToArray();
			if (_appServices.Length != appServices.Length)
			{
				var invalidServices = appServices.Where(x => x == null || (x?.IsAssignableFrom(typeof(AppService)) ?? true)).ToArray();
				foreach (var invalid in invalidServices)
				{
					if (invalid == null)
					{
						ConsoleExtensions.WriteError($"Do not pass in a null type for ${nameof(appServices)}.");
					}
					else
					{
						ConsoleExtensions.WriteError($"Type {invalid.FullName} does not implement {nameof(AppService)} and won't be loaded.");
					}
				}
			}
		}

		/// <summary>
		/// Loads all IAppServices inside the assembly domain
		/// </summary>
		public void FindAllWithInterface()
		{
			_findAll = true;
		}
	}
}
