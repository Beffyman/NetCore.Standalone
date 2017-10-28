using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Standalone.Lifecycle
{
	/// <summary>
	/// Background services that will start at app start and stop when the app stops. Order is not ensured.
	/// </summary>
	public interface ILifecycleService
	{
		/// <summary>
		/// A non-blocking startup for a service
		/// </summary>
		Task StartAsync();

		/// <summary>
		/// A non-blocking stop for a service
		/// </summary>
		Task StopAsync();

	}
}
