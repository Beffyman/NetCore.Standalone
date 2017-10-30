using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCore.Standalone.AppServices
{
	/// <summary>
	/// Background services that will start at app start and stop when the app stops. Order is not ensured.
	/// </summary>
	public abstract class AppService
	{
		internal CancellationTokenSource tokenSource = new CancellationTokenSource();

		internal async Task StartAsync()
		{
			await OnStartAsync();

			var work = Task.Run(StartWork);
		}

		internal async Task StopAsync()
		{
			await OnStopAsync();
			tokenSource.Cancel();
		}


		internal async Task StartWork()
		{
			await Task.Delay(WorkInterval, tokenSource.Token);
			while (!tokenSource.IsCancellationRequested)
			{
				try
				{
					if (WorkBlocks)
					{
						var work = Task.Factory.StartNew(DoWorkAsync, TaskCreationOptions.LongRunning);
					}
					else
					{
						await DoWorkAsync();
					}

					await Task.Delay(WorkInterval, tokenSource.Token);
				}
				catch (Exception ex)
				{
					HandleError(ex);
				}
			}
		}

		/// <summary>
		/// A non-blocking startup for a service
		/// </summary>
		public abstract Task OnStartAsync();

		/// <summary>
		/// A non-blocking stop for a service
		/// </summary>
		public abstract Task OnStopAsync();

		/// <summary>
		/// Will the timer for work only begin after the last work has finished
		/// </summary>
		public abstract bool WorkBlocks { get; protected set; }

		/// <summary>
		/// How often will work be done?
		/// </summary>
		protected abstract TimeSpan WorkInterval { get; set; }

		/// <summary>
		/// Do work periodically
		/// </summary>
		/// <returns></returns>
		protected abstract Task DoWorkAsync();

		/// <summary>
		/// Handles any errors that would occur within the DoWorkAsync
		/// </summary>
		/// <param name="ex"></param>
		protected abstract void HandleError(Exception ex);

	}
}
