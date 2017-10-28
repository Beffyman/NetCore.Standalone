using NetCore.Standalone.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore.Standalone.Execution
{
	/// <summary>
	/// Application that can be ran
	/// </summary>
	public interface IApplication
	{
		/// <summary>
		/// The application has started
		/// </summary>
		bool Running { get; }

		/// <summary>
		/// Startup of the application
		/// </summary>
		BaseStartup Startup { get; }
		/// <summary>
		/// Token Source that controls when the application stops
		/// </summary>
		CancellationTokenSource Cancellation { get; }

		/// <summary>
		/// Blocking method that will start and stop the application when the token is canceled
		/// </summary>
		/// <returns></returns>
		Task RunAsync();

		/// <summary>
		/// Non-blocking start to the application, if the token is canceled, StopAsync is called, or it can be called manually.
		/// </summary>
		/// <returns></returns>
		Task<IApplication> StartAsync();

		/// <summary>
		/// Stops the application
		/// </summary>
		/// <returns></returns>
		Task StopAsync();
	}

	internal sealed class Application : IApplication
	{
		internal IApplicationBuilder Builder { get; set; }
		public BaseStartup Startup => Builder.Startup;
		public CancellationTokenSource Cancellation => Builder.Cancellation;

		public bool Running { get; private set; }

		public Application(ApplicationBuilder builder)
		{
			Builder = builder;
		}

		/// <summary>
		/// Starts and blocks the current thread until the cancellation token from the startup is canceled.
		/// </summary>
		/// <returns></returns>
		public async Task RunAsync()
		{
			Running = true;
			await Startup.StartAsync();
			if (Cancellation == null)
			{
				Console.WriteLine("Exit with Ctrl + C");
				Builder.Cancellation = new CancellationTokenSource();

				Console.CancelKeyPress += (obj, e) =>
				{
					Builder.Cancellation.Cancel();
				};

				await Cancellation.Token.WhenCanceled();
			}
			else
			{
				Console.WriteLine("Cancellation Token was provided, waiting for that to cancel.");
				await Cancellation.Token.WhenCanceled();
			}

			await Startup.ShutdownAsync();

			Running = false;
		}

		/// <summary>
		/// Starts the running application, non-blocking, will stop if token is canceled
		/// </summary>
		/// <returns></returns>
		public async Task<IApplication> StartAsync()
		{
			Running = true;
			if (Cancellation == null)
			{
				Builder.Cancellation = new CancellationTokenSource();
			}
			Cancellation.Token.Register(async () => await StopAsync());

			await Task.Run(Startup.StartAsync, Cancellation.Token);
			return this;
		}

		/// <summary>
		/// Stops the running application
		/// </summary>
		/// <returns></returns>
		public async Task StopAsync()
		{
			Cancellation.Cancel();
			await Startup.ShutdownAsync();
			Running = false;
		}


	}
}
