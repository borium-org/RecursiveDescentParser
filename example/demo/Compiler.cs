using Borium.RDP;

using System;

using static Borium.Demo.Keywords;

using static Borium.RDP.Scanner;
using static Borium.RDP.Text;
using static Borium.RDP.Text.TextMessageType;

namespace Borium.Demo
{
	public partial class Compiler : CompilerBase
	{
		private readonly Set CompilationUnit_stop = new Set();
		private readonly Set PackageDeclaration_stop = new Set();
		private readonly Set rdp_PackageDeclaration_6_first = new Set();

		public Compiler(string fileName)
		{
			rdp_sourcefilename = fileName;
			text_init(1000000, 50, 120, 4);
			scan_init(true, false, false, false, Keywords.tokenNames);
			rdp_set_initialise();
			rdp_load_keywords();

			if (text_open(rdp_sourcefilename) == null)
				throw new Exception("unable to open source file");
			text_get_char();
			scan_();
		}

		public AstCompilationUnit RuleCompilationUnit()
		{
			AstCompilationUnit ast = new AstCompilationUnit();

			text_message(TEXT_INFO, "Entered 'CompilationUnit'\n");

			ast.Add(RulePackageDeclaration());
			scan_test_set("CompilationUnit", CompilationUnit_stop, CompilationUnit_stop);

			text_message(TEXT_INFO, "Exited 'CompilationUnit'\n");

			return ast;
		}

		private AstPackageDeclaration RulePackageDeclaration()
		{
			AstPackageDeclaration ast = new AstPackageDeclaration();

			text_message(TEXT_INFO, "Entered 'PackageDeclaration'\n");

			scan_test("PackageDeclaration", RDP_T_package, PackageDeclaration_stop);
			ast.Add(lastsym);
			scan_();
			scan_test("PackageDeclaration", SCAN_P_ID, PackageDeclaration_stop);
			ast.Add(lastsym);
			scan_();
			{
				// Start of rdp_PackageDeclaration_6
				while (true)
				{
					if (scan_test("rdp_PackageDeclaration_2", RDP_T_61 /* = */, null))
					{
						scan_test("PackageDeclaration", RDP_T_61 /* = */, PackageDeclaration_stop);
						ast.Add(lastsym);
						scan_();
						scan_test("PackageDeclaration", SCAN_P_ID, PackageDeclaration_stop);
						ast.Add(lastsym);
						scan_();
						if (scan_test("rdp_PackageDeclaration_1", RDP_T_46 /* . */, null))
						{
							// Start of rdp_PackageDeclaration_1
							while (true)
							{
								scan_test("PackageDeclaration", RDP_T_46 /* . */, PackageDeclaration_stop);
								ast.Add(lastsym);
								scan_();
								scan_test("PackageDeclaration", SCAN_P_ID, PackageDeclaration_stop);
								ast.Add(lastsym);
								scan_();
								if (!scan_test("rdp_PackageDeclaration_1", RDP_T_46 /* . */, null))
									break;
							}
							// End of rdp_PackageDeclaration_1
						}
					}
					else if (scan_test("rdp_PackageDeclaration_5", RDP_T_46 /* . */, null))
					{
						if (scan_test("rdp_PackageDeclaration_4", RDP_T_46 /* . */, null))
						{
							// Start of rdp_PackageDeclaration_4
							while (true)
							{
								scan_test("PackageDeclaration", RDP_T_46 /* . */, PackageDeclaration_stop);
								ast.Add(lastsym);
								scan_();
								scan_test("PackageDeclaration", SCAN_P_ID, PackageDeclaration_stop);
								ast.Add(lastsym);
								scan_();
								if (!scan_test("rdp_PackageDeclaration_4", RDP_T_46 /* . */, null))
									break;
							}
							// End of rdp_PackageDeclaration_4
						}
					}
					break; // hi limit is 1
				}
				// End of rdp_PackageDeclaration_6
			}
			scan_test("PackageDeclaration", RDP_T_59 /* ; */, PackageDeclaration_stop);
			ast.Add(lastsym);
			scan_();
			scan_test_set("PackageDeclaration", PackageDeclaration_stop, PackageDeclaration_stop);

			text_message(TEXT_INFO, "Exited 'PackageDeclaration'\n");

			return ast;
		}

		private void rdp_load_keywords()
		{
			scan_load_keyword(".", null, RDP_T_46 /* . */, SCAN_P_IGNORE);
			scan_load_keyword(";", null, RDP_T_59 /* ; */, SCAN_P_IGNORE);
			scan_load_keyword("=", null, RDP_T_61 /* = */, SCAN_P_IGNORE);
			scan_load_keyword("package", null, RDP_T_package, SCAN_P_IGNORE);
		}

		private void rdp_set_initialise()
		{
			CompilationUnit_stop.assignList(SCAN_P_EOF);
			PackageDeclaration_stop.assignList(SCAN_P_EOF);
			rdp_PackageDeclaration_6_first.assignList(SCAN_P_EOF, RDP_T_59);
		}
	}

	public partial class AstCompilationUnit : Ast
	{
	}

	internal partial class AstPackageDeclaration : Ast
	{
	}
}
