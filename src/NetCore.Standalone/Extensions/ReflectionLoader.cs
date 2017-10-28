using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetCore.Standalone.Extensions
{
	/// <summary>
	/// Loader for all types and assemblies within the assembly domain
	/// </summary>
	public static class ReflectionLoader
	{

		private static List<Assembly> _loadedAssemblies;
		private static List<Type> _loadedTypes;

		private static readonly object _lock = new object();

		private static void Load()
		{
			var referencedAssemblyNames = new List<AssemblyName>();

			var assemblyNames = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var refAsmName in assemblyNames.SelectMany(x => x.GetReferencedAssemblies()))
			{
				referencedAssemblyNames.Add(refAsmName);
			}
			referencedAssemblyNames = referencedAssemblyNames.GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();

			_loadedAssemblies = new List<Assembly>();
			_loadedAssemblies.AddRange(assemblyNames);

			foreach (var assName in referencedAssemblyNames)
			{
				try
				{
					_loadedAssemblies.Add(Assembly.Load(assName));
					ConsoleExtensions.WriteAssemblyLoad(assName);
				}
				catch
				{
					ConsoleExtensions.WriteError($"Assembly {assName.Name} has failed to load.");
				}
			}
			_loadedAssemblies = _loadedAssemblies.Where(x => !x.IsDynamic).Distinct().ToList();

			_loadedTypes = _loadedAssemblies.SelectMany(x => x.GetTypes())
											.Where(x => !x.IsNotPublic)
											.Distinct(new TypeEquality())
											.ToList();
		}

		/// <summary>
		/// Collection of all assemblies within the application
		/// </summary>
		public static ICollection<Assembly> Assemblies
		{
			get
			{
				lock (_lock)
				{
					if (_loadedAssemblies == null)
					{
						Load();
					}
				}

				return _loadedAssemblies;
			}
		}

		/// <summary>
		/// Collection of all types within the application
		/// </summary>
		public static ICollection<Type> Types
		{
			get
			{
				lock (_lock)
				{
					if (_loadedTypes == null)
					{
						Load();
					}
				}

				return _loadedTypes;
			}
		}

		/// <summary>
		/// Purges all types and assemblies that have been loaded, freeing the memory.  However, they are loaded again when they are used.
		/// </summary>
		public static void Purge()
		{
			_loadedAssemblies = null;
			_loadedTypes = null;
		}

		internal class TypeEquality : IEqualityComparer<Type>
		{
			public bool Equals(Type x, Type y)
			{
				return x.FullName == y.FullName;
			}

			public int GetHashCode(Type obj)
			{
				return obj.GetHashCode();
			}
		}

	}
}
