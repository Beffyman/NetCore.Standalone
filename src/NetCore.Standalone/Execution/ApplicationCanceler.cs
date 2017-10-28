using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Container;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Standalone.Execution
{
	/// <summary>
	/// Service for stopping the application
	/// </summary>
	public interface IApplicationCanceler
	{
		/// <summary>
		/// Cancels the Application Cancellation token
		/// </summary>
		void Shutdown();
	}

	internal class ApplicationCanceler : IApplicationCanceler
	{
		protected IApplicationBuilder _application;
		public ApplicationCanceler(IApplicationBuilder application)
		{
			_application = application;
		}

		public void Shutdown()
		{
			_application.Cancellation.Cancel();
		}
	}
}
