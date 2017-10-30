using Microsoft.Extensions.DependencyInjection;
using NetCore.Standalone.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCore.Standalone.Injectable
{
	/// <summary>
	/// Magic service registration
	/// </summary>
	public interface IInjectable { }
	/// <summary>
	/// Magically register this service with a singleton lifecycle
	/// </summary>
	public interface ISingletonInjectable : IInjectable { }
	/// <summary>
	/// Magically register this service with a transient lifecycle
	/// </summary>
	public interface ITransientInjectable : IInjectable { }
	/// <summary>
	/// Magically register this service with a scoped lifecycle
	/// </summary>
	public interface IScopedInjectable : IInjectable { }
}
