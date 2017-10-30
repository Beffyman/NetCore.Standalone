using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCore.Standalone.Windsor.Tests
{
	/// <summary>
	/// Container for service registration and resolving
	/// </summary>
	public interface IContainer : IWindsorContainer
	{
		/// <summary>
		/// Resolve a type as another that it implements.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		T Resolve<T>(Type type) where T : class;
		/// <summary>
		/// Creates the container, this should be called within the overridden CreateProvider
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		IServiceProvider Create(IServiceCollection collection);
	}

	/// <summary>
	/// Implementation of the Windsor container
	/// </summary>
	public class WindsorStandaloneContainer : WindsorContainer, IContainer
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public WindsorStandaloneContainer()
		{

		}

		/// <summary>
		/// Creates the container
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public IServiceProvider Create(IServiceCollection collection)
		{
			var provider = WindsorRegistrationHelper.CreateServiceProvider(this, collection);

			this.Register(Component.For<IWindsorContainer, IContainer>()
									.Instance(this)
									.IsDefault()
									.Named("IContainer"));

			return provider;
		}

		/// <summary>
		/// Tries to resolve a type as the generic type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		public T Resolve<T>(Type type) where T : class
		{
			try
			{
				return (T)this.Resolve(type);
			}
			catch (Exception ex)
			{
				try
				{
					return (T)Activator.CreateInstance(type);
				}
				catch
				{
					throw ex;
				}

			}
		}


	}
}
