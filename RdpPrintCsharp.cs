using System;
using System.IO;

using static Borium.RDP.RdpAux;
using static Borium.RDP.RdpProgram;
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
			printCompilerSets(rdp_base);
			printKeywords();
		}

		private void printCompilerSets(SymbolScopeData scopeData)
		{
			TextWriter file = createFile("CompilerSets");
			text_redirect(file);
			iprintln("namespace " + classPackage + ";");
			iprintln("{");
			rdp_indentation++;
			iprintln("public partial class Compiler : CompilerBase");
			iprintln("{");
			rdp_indentation++;

			RdpData temp = (RdpData)scopeData.nextSymbolInScope();
			while (temp != null)
			{
				if (rdp_production_set.includes(temp.kind) && temp.code_only == 0)
				{
					if (temp.first_cardinality > 1)
					{
						int column = rdp_indentation * 3; // one less than actual indent per tab
						column += iprint($"protected Set {text_get_string(temp.id)}_first = new Set(");
						temp.first.printIndented(rdp_enum_string, column, 120, rdp_indentation);
						println(");");
					}
					if (temp.kind == K_PRIMARY)
					{
						int column = rdp_indentation * 3; // one less than actual indent per tab
						column += iprint($"protected Set {text_get_string(temp.id)}_stop = new Set(");
						temp.follow.printIndented(rdp_enum_string, column, 120, rdp_indentation);
						println(");");
					}
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}

			rdp_indentation--;
			iprintln("}");
			rdp_indentation--;
			iprintln("}");
			text_redirect(Console.Out);
			file.Close();
		}

		private void printKeywords()
		{
			TextWriter file = createFile("Keywords");
			text_redirect(file);
			iprintln("namespace " + classPackage + ";");
			iprintln("{");
			rdp_indentation++;
			iprintln("internal class Keywords");
			iprintln("{");
			rdp_indentation++;

			int offset = 0;
			iprintln($"internal const int SCAN_P_EOF = {offset++};");
			iprintln($"internal const int SCAN_P_ID = {offset++};");
			iprintln($"internal const int SCAN_P_ERROR = {offset++};");
			iprintln($"internal const int SCAN_P_INTEGER = {offset++};");
			iprintln($"internal const int SCAN_P_REAL = {offset++};");

			RdpData temp = (RdpData)tokens.getScope().nextSymbolInScope();
			while (temp != null)
			{
				if (temp.kind == K_TOKEN || temp.kind == K_EXTENDED)
				{
					iprint("internal const int ");
					rdp_print_parser_production_name(temp);
					println($" = {offset++};");
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}

			iprintln();

			iprintln("internal string[] tokenNames = {");
			rdp_indentation++;

			iprintln("\"<EOF>\",");
			iprintln("\"<Ident\",");
			iprintln("\"<Error>\",");
			iprintln("\"<Integer>\",");
			iprintln("\"<Real>\",");

			temp = (RdpData)tokens.getScope().nextSymbolInScope();
			while (temp != null)
			{
				if (temp.kind == K_TOKEN || temp.kind == K_EXTENDED)
				{
					string tokenName = text_get_string(temp.token_enum);
					int pos = tokenName.IndexOf(' ');
					if (pos != -1)
					{
						tokenName = tokenName.Substring(pos + 4, tokenName.Length - pos - 7);
					}
					else
					{
						tokenName = tokenName.Substring(6);
					}
					string sanitized = "";
					foreach (char ch in tokenName)
					{
						if (ch == '\"')
						{
							sanitized += "\\\"";
						}
						else
						{
							sanitized += ch;
						}
					}
					if (sanitized == "\\\"")
					{
						sanitized = "<String>";
					}
					if (sanitized == "\'")
					{
						sanitized = "<Char>";
					}

					iprint($"\"{sanitized}\",\n");
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}

			rdp_indentation--;
			iprintln("};");
			rdp_indentation--;
			iprintln("}");
			rdp_indentation--;
			iprintln("}");
			text_redirect(Console.Out);
			file.Close();
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
