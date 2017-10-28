using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetCore.Standalone.Execution
{
	/// <summary>
	/// Options for startup
	/// </summary>
	public sealed class ApplicationStartupOptions
	{
		internal CancellationTokenSource _cancellationToken = null;


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
	}

}
