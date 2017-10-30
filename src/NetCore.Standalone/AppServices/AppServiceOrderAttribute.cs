using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Standalone.AppServices
{
	/// <summary>
	/// Determines what order the AppService will be loaded in. This is important for dependency injection if one depends on another.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class AppServiceOrderAttribute : Attribute
	{
		/// <summary>
		/// Order in which the AppService will be loaded in, lower order will be loaded first.
		/// </summary>
		public int Order { get; set; }
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="order"></param>
		public AppServiceOrderAttribute(int order)
		{
			Order = order;
		}
	}
}
