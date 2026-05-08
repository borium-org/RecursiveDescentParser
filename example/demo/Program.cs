using System;
using System.IO;

namespace Borium.Demo
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
				args = new string[] { "demo.icp" };
			TextReader textReader = new StreamReader(args[0]);
			TextWriter textWriter = new StringWriter();
			DemoCompiler compiler = new DemoCompiler(args[0], textReader, textWriter);
			compiler.Compile();
			Console.Write(textWriter.ToString());
		}
	}
}
