using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetCore.Standalone.Extensions
{
	internal static class ConsoleExtensions
	{
		public static void WriteVariableLoad(string path, string value, ConsoleColor color = ConsoleColor.Green)
		{
			var prev = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write($"VAR ");
			Console.ForegroundColor = prev;
			Console.Write($"{path} ");
			Console.ForegroundColor = color;
			Console.Write($"=");
			Console.ForegroundColor = prev;
			Console.Write($" {value}");
			Console.WriteLine();
		}

		public static void WriteAssemblyLoad(AssemblyName value, ConsoleColor color = ConsoleColor.Blue)
		{
			var prev = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write($"ASSEMBLY ");
			Console.ForegroundColor = prev;
			Console.Write($"{value.Name} ");
			Console.ForegroundColor = color;
			Console.Write($" V");
			Console.ForegroundColor = prev;
			Console.Write($"{value.Version}");
			Console.WriteLine();
		}

		public static void WriteError(string value, ConsoleColor color = ConsoleColor.Red)
		{
			var prev = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(value);
			Console.ForegroundColor = prev;
		}
		public static void WriteError(Exception value, ConsoleColor color = ConsoleColor.Red)
		{
			var prev = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(value);
			Console.ForegroundColor = prev;
		}

		public static void WriteServiceRegister(string lifecycle, Type interfaceType, Type implementationType, ConsoleColor color = ConsoleColor.Magenta)
		{
			var prev = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write($"{lifecycle.ToUpper()} SERVICE: ");
			Console.ForegroundColor = prev;
			Console.Write($"{interfaceType.FullName} ");
			Console.ForegroundColor = color;
			Console.Write($"=>");
			Console.ForegroundColor = prev;
			Console.Write($" {implementationType.FullName}");
			Console.WriteLine();
		}

		public static void WriteAppServiceRegister(Type type, ConsoleColor color = ConsoleColor.Yellow)
		{
			var prev = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write($"APP SERVICE ");
			Console.ForegroundColor = prev;
			Console.Write($"{type.FullName}");
			Console.WriteLine();
		}
	}
}
