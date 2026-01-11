using System;
using System.IO;

using static Borium.RDP.Text;

namespace Borium.RDP
{
	internal class RdpPrintCsharp : RdpPrint
	{
		/** Path where the files will be written */
		private string outputPath;
		/** Package for all generated classes */
		private string classPackage;

		public RdpPrintCsharp(string outputPath, string classPackage)
		{
			this.outputPath = outputPath;
			this.classPackage = classPackage;
		}

		internal void print(SymbolScopeData rdp_base, bool parser_only)
		{
			TextWriter file = createFile("Compiler");
			text_redirect(file);
			iprintln("namespace " + classPackage + ";");
			iprintln("{");
			rdp_indentation++;
			iprintln("public class Compiler : CompilerBase");
			iprintln("{");
			rdp_indentation++;
			rdp_indentation--;
			iprintln("}");
			rdp_indentation--;
			iprintln("}");
			text_redirect(Console.Out);
			file.Close();
			throw new NotImplementedException();
		}

		protected override int indent()
		{
			return indent(0);
		}

		protected override int indent(int extraIndent)
		{
			for (int temp = 0; temp < rdp_indentation + extraIndent; temp++)
			{
				text_printf("\t");
			}
			return (rdp_indentation + extraIndent) * 4;
		}

		private TextWriter createFile(string className)
		{
			TextWriter file = null;
			try
			{
				Directory.CreateDirectory(outputPath);
				file = new StreamWriter(outputPath + "/" + className + ".cs");
				rdp_indentation = 0;
			}
			catch (FileNotFoundException)
			{
				throw new Exception();
			}
			return file;
		}

	}
}
