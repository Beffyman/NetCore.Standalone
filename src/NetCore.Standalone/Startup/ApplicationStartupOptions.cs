using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetCore.Standalone.Startup
{
	/// <summary>
	/// Options for startup
	/// </summary>
	public sealed class ApplicationStartupOptions
	{
		internal CancellationTokenSource _cancellationToken = null;
		internal string[] Arguments = null;

		internal ApplicationStartupOptions()
		{

		}

		/// <summary>
		/// Provide a custom cancellation token for the application to use
		/// </summary>
		/// <param name="cancellationToken"></param>
		public void UseCancellationToken(CancellationTokenSource cancellationToken)
		{
			_cancellationToken = cancellationToken;
		}

		/// <summary>
		/// Provide application arguments for the startup class to use.
		/// </summary>
		/// <param name="arguments"></param>
		public void UseArguments(params string[] arguments)
		{
			Arguments = arguments;
		}
	}

}
