using NetCore.Standalone.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCore.Standalone.Lifecycle
{
	/// <summary>
	/// Options for the configuration of ILifecycleServices
	/// </summary>
	public sealed class LifecycleServiceOptions
	{

		internal Type[] _lifecycleServices;
		internal bool _findAll;

		internal LifecycleServiceOptions()
		{

		}

		/// <summary>
		/// Only the types provided will be loaded
		/// </summary>
		/// <param name="lifecycleServices"></param>
		public void UseLifecycleServices(params Type[] lifecycleServices)
		{
			_lifecycleServices = lifecycleServices.Where(x => x != null && x.IsAssignableFrom(typeof(ILifecycleService))).ToArray();
			if (_lifecycleServices.Length != lifecycleServices.Length)
			{
				var invalidServices = lifecycleServices.Where(x => x == null || (x?.IsAssignableFrom(typeof(ILifecycleService)) ?? true)).ToArray();
				foreach (var invalid in invalidServices)
				{
					if (invalid == null)
					{
						ConsoleExtensions.WriteError($"Do not pass in a null type for lifecycle services.");
					}
					else
					{
						ConsoleExtensions.WriteError($"Type {invalid.FullName} does not implement {nameof(ILifecycleService)} and won't be loaded.");
					}
				}
			}
		}

		/// <summary>
		/// Loads all ILifecycleServices inside the assembly domain
		/// </summary>
		public void FindAllWithInterface()
		{
			_findAll = true;
		}
	}
}
