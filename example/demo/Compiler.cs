using Borium.RDP;

using System;
using System.IO;
using static Borium.RDP.Text.TextMessageType;

namespace Borium.Demo
{
	public partial class DemoCompiler : CompilerBase
	{
		private const int RDP_TT_BOTTOM = SCAN_P_TOP;
		private const int RDP_T_34 /* " */ = SCAN_P_TOP + 0;
		private const int RDP_T_39 /* ' */ = SCAN_P_TOP + 1;
		private const int RDP_T_40 /* ( */ = SCAN_P_TOP + 2;
		private const int RDP_T_41 /* ) */ = SCAN_P_TOP + 3;
		private const int RDP_T_44 /* , */ = SCAN_P_TOP + 4;
		private const int RDP_T_46 /* . */ = SCAN_P_TOP + 5;
		private const int RDP_T_47 /* / */ = SCAN_P_TOP + 6;
		private const int RDP_T_4742 /* / * */ = SCAN_P_TOP + 7;
		private const int RDP_T_4747 /* // */ = SCAN_P_TOP + 8;
		private const int RDP_T_58 /* : */ = SCAN_P_TOP + 9;
		private const int RDP_T_59 /* ; */ = SCAN_P_TOP + 10;
		private const int RDP_T_61 /* = */ = SCAN_P_TOP + 11;
		private const int RDP_T_64 /* @ */ = SCAN_P_TOP + 12;
		private const int RDP_T_class = SCAN_P_TOP + 13;
		private const int RDP_T_enum = SCAN_P_TOP + 14;
		private const int RDP_T_import = SCAN_P_TOP + 15;
		private const int RDP_T_interface = SCAN_P_TOP + 16;
		private const int RDP_T_internal = SCAN_P_TOP + 17;
		private const int RDP_T_package = SCAN_P_TOP + 18;
		private const int RDP_T_private = SCAN_P_TOP + 19;
		private const int RDP_T_protected = SCAN_P_TOP + 20;
		private const int RDP_T_public = SCAN_P_TOP + 21;
		private const int RDP_T_static = SCAN_P_TOP + 22;
		private const int RDP_T_typedef = SCAN_P_TOP + 23;
		private const int RDP_T_123 /* { */ = SCAN_P_TOP + 24;
		private const int RDP_T_125 /* } */ = SCAN_P_TOP + 25;
		private const int RDP_TT_TOP = SCAN_P_TOP + 26;

		private string[] rdp_tokens = { "IGNORE","ID", "INTEGER", "REAL", "CHAR", "CHAR_ESC",
			"STRING", "STRING_ESC", "COMMENT", "COMMENT_VISIBLE", "COMMENT_NEST", "COMMENT_NEST_VISIBLE",
			"COMMENT_LINE", "COMMENT_LINE_VISIBLE", "EOF", "EOLN", "'\"'", "'\''", "'('", "')'", "','",
			"'.'", "'/'", "'/*'", "'//'", "':'", "';'", "'='", "'@'", "'class'", "'enum'", "'import'",
			"'interface'", "'internal'", "'package'", "'private'", "'protected'", "'public'", "'static'",
			"'typedef'", "'{'", "'}'" };

		private readonly Set AccessModifier_first = new Set();
		private readonly Set AccessModifier_stop = new Set();
		private readonly Set Annotation_stop = new Set();
		private readonly Set Character_stop = new Set();
		private readonly Set Class_stop = new Set();
		private readonly Set CommentLine_stop = new Set();
		private readonly Set CommentNest_stop = new Set();
		private readonly Set CompilationUnit_first = new Set();
		private readonly Set CompilationUnit_stop = new Set();
		private readonly Set Entity_first = new Set();
		private readonly Set Entity_stop = new Set();
		private readonly Set Enum_stop = new Set();
		private readonly Set Import_stop = new Set();
		private readonly Set Interface_stop = new Set();
		private readonly Set InterfaceMethodDeclaration_first = new Set();
		private readonly Set InterfaceMethodDeclaration_stop = new Set();
		private readonly Set MethodOrFieldDeclaration_first = new Set();
		private readonly Set MethodOrFieldDeclaration_stop = new Set();
		private readonly Set PackageDeclaration_stop = new Set();
		private readonly Set Parameters_stop = new Set();
		private readonly Set QualifiedIdentifier_stop = new Set();
		private readonly Set String_stop = new Set();
		private readonly Set Typedef_stop = new Set();
		private readonly Set rdp_Class_4_first = new Set();
		private readonly Set rdp_Class_5_first = new Set();
		private readonly Set rdp_CompilationUnit_4_first = new Set();
		private readonly Set rdp_CompilationUnit_5_first = new Set();
		private readonly Set rdp_CompilationUnit_6_first = new Set();
		private readonly Set rdp_Entity_0_first = new Set();
		private readonly Set rdp_Entity_1_first = new Set();
		private readonly Set rdp_Entity_7_first = new Set();
		private readonly Set rdp_Entity_8_first = new Set();
		private readonly Set rdp_InterfaceMethodDeclaration_0_first = new Set();
		private readonly Set rdp_InterfaceMethodDeclaration_1_first = new Set();
		private readonly Set rdp_InterfaceMethodDeclaration_4_first = new Set();
		private readonly Set rdp_Interface_0_first = new Set();
		private readonly Set rdp_Interface_1_first = new Set();
		private readonly Set rdp_MethodOrFieldDeclaration_0_first = new Set();
		private readonly Set rdp_MethodOrFieldDeclaration_1_first = new Set();
		private readonly Set rdp_MethodOrFieldDeclaration_6_first = new Set();
		private readonly Set rdp_MethodOrFieldDeclaration_7_first = new Set();

		private readonly int rdp_tabwidth = 4;
		private readonly int rdp_textsize = 1000000;
		private readonly bool rdp_symbol_echo = false;

		public DemoCompiler(string fileName, TextReader textReader, TextWriter textWriter)
		{
			SourceFileName = fileName;
			TextInit(rdp_textsize, 25, 100, rdp_tabwidth);
			if (textWriter != null)
				TextRedirect(textWriter);

			ScannerInit(false, false, false, rdp_symbol_echo, rdp_tokens);
			rdp_set_initialise();
			rdp_load_keywords();
			if (TextOpen(fileName, textReader) == null)
				throw new Exception("unable to open source file");
			TextGetChar();
			Scan();
		}

		internal void Compile()
		{
			RuleCompilationUnit();
			if (TextTotalErrors() != 0)
				TextMessage(TEXT_FATAL, $"error{(TextTotalErrors() == 1 ? "" : "s")} detected in source file '{SourceFileName}'\n");
		}

		private void RuleAccessModifier()
		{
			TextMessage(TEXT_INFO, "Entered 'AccessModifier'\n");

			if (ScanTest("rdp_AccessModifier_0", RDP_T_public, null))
			{
				ScanTest("AccessModifier", RDP_T_public, AccessModifier_stop);
				Scan();
			}
			else if (ScanTest("rdp_AccessModifier_1", RDP_T_internal, null))
			{
				ScanTest("AccessModifier", RDP_T_internal, AccessModifier_stop);
				Scan();
			}
			else if (ScanTest("rdp_AccessModifier_2", RDP_T_protected, null))
			{
				ScanTest("AccessModifier", RDP_T_protected, AccessModifier_stop);
				Scan();
			}
			else if (ScanTest("rdp_AccessModifier_3", RDP_T_private, null))
			{
				ScanTest("AccessModifier", RDP_T_private, AccessModifier_stop);
				Scan();
			}
			else
				ScanTestSet("AccessModifier", AccessModifier_first, AccessModifier_stop);
			ScanTestSet("AccessModifier", AccessModifier_stop, AccessModifier_stop);

			TextMessage(TEXT_INFO, "Exited  'AccessModifier'\n");
		}

		private void RuleAnnotation()
		{
			TextMessage(TEXT_INFO, "Entered 'Annotation'\n");

			ScanTest("Annotation", RDP_T_64 /* @ */, Annotation_stop);
			Scan();
			ScanTest("Annotation", RDP_T_interface, Annotation_stop);
			Scan();
			ScanTest("Annotation", SCAN_P_ID, Annotation_stop);
			Scan();
			ScanTest("Annotation", RDP_T_123 /* { */, Annotation_stop);
			Scan();
			ScanTest("Annotation", RDP_T_125 /* } */, Annotation_stop);
			Scan();
			ScanTestSet("Annotation", Annotation_stop, Annotation_stop);

			TextMessage(TEXT_INFO, "Exited  'Annotation'\n");
		}

		private void RuleClass()
		{
			TextMessage(TEXT_INFO, "Entered 'Class'\n");

			ScanTest("Class", RDP_T_class, Class_stop);
			Scan();
			ScanTest("Class", SCAN_P_ID, Class_stop);
			Scan();
			if (ScanTest("rdp_Class_3", RDP_T_58 /* : */, null))
			{ /* Start of rdp_Class_3 */
				while (true)
				{
					ScanTest("Class", RDP_T_58 /* : */, Class_stop);
					Scan();
					RuleQualifiedIdentifier();
					if (ScanTest("rdp_Class_1", RDP_T_44 /* , */, null))
					{ /* Start of rdp_Class_1 */
						while (true)
						{
							{
								ScanTest("Class", RDP_T_44 /* , */, Class_stop);
								Scan();
								RuleQualifiedIdentifier();
							}
							if (!ScanTest("rdp_Class_1", RDP_T_44 /* , */, null))
								break;
						}
					} /* end of rdp_Class_1 */
					break;   /* hi limit is 1! */
				}
			} /* end of rdp_Class_3 */
			ScanTest("Class", RDP_T_123 /* { */, Class_stop);
			Scan();
			if (ScanTestSet("rdp_Class_5", rdp_Class_5_first, null))
			{ /* Start of rdp_Class_5 */
				while (true)
				{
					RuleMethodOrFieldDeclaration();
					if (!ScanTestSet("rdp_Class_5", rdp_Class_5_first, null))
						break;
				}
			} /* end of rdp_Class_5 */
			ScanTest("Class", RDP_T_125 /* } */, Class_stop);
			Scan();
			ScanTestSet("Class", Class_stop, Class_stop);

			TextMessage(TEXT_INFO, "Exited  'Class'\n");
		}

		private void RuleCompilationUnit()
		{
			TextMessage(TEXT_INFO, "Entered 'CompilationUnit'\n");

			if (ScanTest("rdp_CompilationUnit_1", RDP_T_package, null))
			{ /* Start of rdp_CompilationUnit_1 */
				while (true)
				{
					RulePackageDeclaration();
					break;   /* hi limit is 1! */
				}
			} /* end of rdp_CompilationUnit_1 */
			if (ScanTest("rdp_CompilationUnit_3", RDP_T_import, null))
			{ /* Start of rdp_CompilationUnit_3 */
				while (true)
				{
					RuleImport();
					if (!ScanTest("rdp_CompilationUnit_3", RDP_T_import, null))
						break;
				}
			} /* end of rdp_CompilationUnit_3 */
			if (ScanTestSet("rdp_CompilationUnit_5", rdp_CompilationUnit_5_first, null))
			{ /* Start of rdp_CompilationUnit_5 */
				while (true)
				{
					RuleEntity();
					if (!ScanTestSet("rdp_CompilationUnit_5", rdp_CompilationUnit_5_first, null))
						break;
				}
			} /* end of rdp_CompilationUnit_5 */
			ScanTestSet("CompilationUnit", CompilationUnit_stop, CompilationUnit_stop);

			TextMessage(TEXT_INFO, "Exited  'CompilationUnit'\n");
		}

		private void RuleEntity()
		{
			TextMessage(TEXT_INFO, "Entered 'Entity'\n");

			if (ScanTestSet("rdp_Entity_1", rdp_Entity_1_first, null))
			{ /* Start of rdp_Entity_1 */
				while (true)
				{
					RuleAccessModifier();
					break;   /* hi limit is 1! */
				}
			} /* end of rdp_Entity_1 */
			{ /* Start of rdp_Entity_7 */
				while (true)
				{
					ScanTestSet("rdp_Entity_7", rdp_Entity_7_first, Entity_stop);
					{
						if (ScanTest("rdp_Entity_2", RDP_T_class, null))
						{
							RuleClass();
						}
						else if (ScanTest("rdp_Entity_3", RDP_T_interface, null))
						{
							RuleInterface();
						}
						else if (ScanTest("rdp_Entity_4", RDP_T_64 /* @ */, null))
						{
							RuleAnnotation();
						}
						else if (ScanTest("rdp_Entity_5", RDP_T_enum, null))
						{
							RuleEnum();
						}
						else if (ScanTest("rdp_Entity_6", RDP_T_typedef, null))
						{
							RuleTypedef();
						}
						else
							ScanTestSet("rdp_Entity_7", rdp_Entity_7_first, Entity_stop);
					}
					break;   /* hi limit is 1! */
				}
			} /* end of rdp_Entity_7 */
			ScanTestSet("Entity", Entity_stop, Entity_stop);

			TextMessage(TEXT_INFO, "Exited  'Entity'\n");
		}

		private void RuleEnum()
		{
			TextMessage(TEXT_INFO, "Entered 'Enum'\n");

			ScanTest("Enum", RDP_T_enum, Enum_stop);
			Scan();
			ScanTest("Enum", SCAN_P_ID, Enum_stop);
			Scan();
			ScanTest("Enum", RDP_T_123 /* { */, Enum_stop);
			Scan();
			ScanTest("Enum", RDP_T_125 /* } */, Enum_stop);
			Scan();
			ScanTestSet("Enum", Enum_stop, Enum_stop);

			TextMessage(TEXT_INFO, "Exited  'Enum'\n");
		}

		private void RuleImport()
		{
			TextMessage(TEXT_INFO, "Entered 'Import'\n");

			ScanTest("Import", RDP_T_import, Import_stop);
			Scan();
			if (ScanTest("rdp_Import_1", RDP_T_static, null))
			{ /* Start of rdp_Import_1 */
				while (true)
				{
					{
						ScanTest("Import", RDP_T_static, Import_stop);
						Scan();
					}
					break;   /* hi limit is 1! */
				}
			} /* end of rdp_Import_1 */
			ScanTest("Import", SCAN_P_ID, Import_stop);
			Scan();
			if (ScanTest("rdp_Import_3", RDP_T_61 /* = */, null))
			{ /* Start of rdp_Import_3 */
				while (true)
				{
					ScanTest("Import", RDP_T_61 /* = */, Import_stop);
					Scan();
					ScanTest("Import", SCAN_P_ID, Import_stop);
					Scan();
					break;   /* hi limit is 1! */
				}
			} /* end of rdp_Import_3 */
			if (ScanTest("rdp_Import_5", RDP_T_46 /* . */, null))
			{ /* Start of rdp_Import_5 */
				while (true)
				{
					ScanTest("Import", RDP_T_46 /* . */, Import_stop);
					Scan();
					ScanTest("Import", SCAN_P_ID, Import_stop);
					Scan();
					if (!ScanTest("rdp_Import_5", RDP_T_46 /* . */, null))
						break;
				}
			} /* end of rdp_Import_5 */
			ScanTest("Import", RDP_T_59 /* ; */, Import_stop);
			Scan();
			ScanTestSet("Import", Import_stop, Import_stop);

			TextMessage(TEXT_INFO, "Exited  'Import'\n");
		}

		private void RuleInterface()
		{
			TextMessage(TEXT_INFO, "Entered 'Interface'\n");

			ScanTest("Interface", RDP_T_interface, Interface_stop);
			Scan();
			ScanTest("Interface", SCAN_P_ID, Interface_stop);
			Scan();
			ScanTest("Interface", RDP_T_123 /* { */, Interface_stop);
			Scan();
			if (ScanTestSet("rdp_Interface_1", rdp_Interface_1_first, null))
			{ /* Start of rdp_Interface_1 */
				while (true)
				{
					RuleInterfaceMethodDeclaration();
					if (!ScanTestSet("rdp_Interface_1", rdp_Interface_1_first, null))
						break;
				}
			} /* end of rdp_Interface_1 */
			ScanTest("Interface", RDP_T_125 /* } */, Interface_stop);
			Scan();
			ScanTestSet("Interface", Interface_stop, Interface_stop);

			TextMessage(TEXT_INFO, "Exited  'Interface'\n");
		}

		private void RuleInterfaceMethodDeclaration()
		{
			TextMessage(TEXT_INFO, "Entered 'InterfaceMethodDeclaration'\n");

			if (ScanTestSet("rdp_InterfaceMethodDeclaration_1", rdp_InterfaceMethodDeclaration_1_first, null))
			{ /* Start of rdp_InterfaceMethodDeclaration_1 */
				while (true)
				{
					RuleAccessModifier();
					if (!ScanTestSet("rdp_InterfaceMethodDeclaration_1", rdp_InterfaceMethodDeclaration_1_first, null))
						break;
				}
			} /* end of rdp_InterfaceMethodDeclaration_1 */
			ScanTest("InterfaceMethodDeclaration", SCAN_P_ID, InterfaceMethodDeclaration_stop);
			Scan();
			ScanTest("InterfaceMethodDeclaration", SCAN_P_ID, InterfaceMethodDeclaration_stop);
			Scan();
			ScanTest("InterfaceMethodDeclaration", RDP_T_40 /* ( */, InterfaceMethodDeclaration_stop);
			Scan();
			if (ScanTest("rdp_InterfaceMethodDeclaration_3", SCAN_P_ID, null))
			{ /* Start of rdp_InterfaceMethodDeclaration_3 */
				while (true)
				{
					RuleParameters();
					break;   /* hi limit is 1! */
				}
			} /* end of rdp_InterfaceMethodDeclaration_3 */
			ScanTest("InterfaceMethodDeclaration", RDP_T_41 /* ) */, InterfaceMethodDeclaration_stop);
			Scan();
			ScanTest("InterfaceMethodDeclaration", RDP_T_59 /* ; */, InterfaceMethodDeclaration_stop);
			Scan();
			ScanTestSet("InterfaceMethodDeclaration", InterfaceMethodDeclaration_stop, InterfaceMethodDeclaration_stop);

			TextMessage(TEXT_INFO, "Exited  'InterfaceMethodDeclaration'\n");
		}

		private void RuleMethodOrFieldDeclaration()
		{
			TextMessage(TEXT_INFO, "Entered 'MethodOrFieldDeclaration'\n");

			if (ScanTestSet("rdp_MethodOrFieldDeclaration_1", rdp_MethodOrFieldDeclaration_1_first, null))
			{ /* Start of rdp_MethodOrFieldDeclaration_1 */
				while (true)
				{
					RuleAccessModifier();
					if (!ScanTestSet("rdp_MethodOrFieldDeclaration_1", rdp_MethodOrFieldDeclaration_1_first, null))
						break;
				}
			} /* end of rdp_MethodOrFieldDeclaration_1 */
			ScanTest("MethodOrFieldDeclaration", SCAN_P_ID, MethodOrFieldDeclaration_stop);
			Scan();
			ScanTest("MethodOrFieldDeclaration", SCAN_P_ID, MethodOrFieldDeclaration_stop);
			Scan();
			{ /* Start of rdp_MethodOrFieldDeclaration_6 */
				while (true)
				{
					ScanTestSet("rdp_MethodOrFieldDeclaration_6", rdp_MethodOrFieldDeclaration_6_first, MethodOrFieldDeclaration_stop);
					{
						if (ScanTest("rdp_MethodOrFieldDeclaration_4", RDP_T_40 /* ( */, null))
						{
							ScanTest("MethodOrFieldDeclaration", RDP_T_40 /* ( */, MethodOrFieldDeclaration_stop);
							Scan();
							if (ScanTest("rdp_MethodOrFieldDeclaration_3", SCAN_P_ID, null))
							{ /* Start of rdp_MethodOrFieldDeclaration_3 */
								while (true)
								{
									RuleParameters();
									break;   /* hi limit is 1! */
								}
							} /* end of rdp_MethodOrFieldDeclaration_3 */
							ScanTest("MethodOrFieldDeclaration", RDP_T_41 /* ) */, MethodOrFieldDeclaration_stop);
							Scan();
							ScanTest("MethodOrFieldDeclaration", RDP_T_123 /* { */, MethodOrFieldDeclaration_stop);
							Scan();
							ScanTest("MethodOrFieldDeclaration", RDP_T_125 /* } */, MethodOrFieldDeclaration_stop);
							Scan();
						}
						else if (ScanTest("rdp_MethodOrFieldDeclaration_5", RDP_T_59 /* ; */, null))
						{
							ScanTest("MethodOrFieldDeclaration", RDP_T_59 /* ; */, MethodOrFieldDeclaration_stop);
							Scan();
						}
						else
							ScanTestSet("rdp_MethodOrFieldDeclaration_6", rdp_MethodOrFieldDeclaration_6_first, MethodOrFieldDeclaration_stop);
					}
					break;   /* hi limit is 1! */
				}
			} /* end of rdp_MethodOrFieldDeclaration_6 */
			ScanTestSet("MethodOrFieldDeclaration", MethodOrFieldDeclaration_stop, MethodOrFieldDeclaration_stop);

			TextMessage(TEXT_INFO, "Exited  'MethodOrFieldDeclaration'\n");
		}

		private void RulePackageDeclaration()
		{
			TextMessage(TEXT_INFO, "Entered 'PackageDeclaration'\n");

			ScanTest("PackageDeclaration", RDP_T_package, PackageDeclaration_stop);
			Scan();
			RuleQualifiedIdentifier();
			ScanTest("PackageDeclaration", RDP_T_59 /* ; */, PackageDeclaration_stop);
			Scan();
			ScanTestSet("PackageDeclaration", PackageDeclaration_stop, PackageDeclaration_stop);

			TextMessage(TEXT_INFO, "Exited  'PackageDeclaration'\n");
		}

		private void RuleParameters()
		{
			TextMessage(TEXT_INFO, "Entered 'Parameters'\n");

			ScanTest("Parameters", SCAN_P_ID, Parameters_stop);
			Scan();
			ScanTest("Parameters", SCAN_P_ID, Parameters_stop);
			Scan();
			if (ScanTest("rdp_Parameters_1", RDP_T_44 /* , */, null))
			{ /* Start of rdp_Parameters_1 */
				while (true)
				{
					ScanTest("Parameters", RDP_T_44 /* , */, Parameters_stop);
					Scan();
					ScanTest("Parameters", SCAN_P_ID, Parameters_stop);
					Scan();
					ScanTest("Parameters", SCAN_P_ID, Parameters_stop);
					Scan();
					if (!ScanTest("rdp_Parameters_1", RDP_T_44 /* , */, null))
						break;
				}
			} /* end of rdp_Parameters_1 */
			ScanTestSet("Parameters", Parameters_stop, Parameters_stop);

			TextMessage(TEXT_INFO, "Exited  'Parameters'\n");
		}

		private void RuleQualifiedIdentifier()
		{
			TextMessage(TEXT_INFO, "Entered 'QualifiedIdentifier'\n");

			ScanTest("QualifiedIdentifier", SCAN_P_ID, QualifiedIdentifier_stop);
			Scan();
			if (ScanTest("rdp_QualifiedIdentifier_1", RDP_T_46 /* . */, null))
			{ /* Start of rdp_QualifiedIdentifier_1 */
				while (true)
				{
					ScanTest("QualifiedIdentifier", RDP_T_46 /* . */, QualifiedIdentifier_stop);
					Scan();
					ScanTest("QualifiedIdentifier", SCAN_P_ID, QualifiedIdentifier_stop);
					Scan();
					if (!ScanTest("rdp_QualifiedIdentifier_1", RDP_T_46 /* . */, null))
						break;
				}
			} /* end of rdp_QualifiedIdentifier_1 */
			ScanTestSet("QualifiedIdentifier", QualifiedIdentifier_stop, QualifiedIdentifier_stop);

			TextMessage(TEXT_INFO, "Exited  'QualifiedIdentifier'\n");
		}

		private void RuleTypedef()
		{
			TextMessage(TEXT_INFO, "Entered 'Typedef'\n");

			ScanTest("Typedef", RDP_T_typedef, Typedef_stop);
			Scan();
			ScanTest("Typedef", SCAN_P_ID, Typedef_stop);
			Scan();
			ScanTest("Typedef", RDP_T_61 /* = */, Typedef_stop);
			Scan();
			ScanTest("Typedef", SCAN_P_ID, Typedef_stop);
			Scan();
			ScanTest("Typedef", RDP_T_59 /* ; */, Typedef_stop);
			Scan();
			ScanTestSet("Typedef", Typedef_stop, Typedef_stop);

			TextMessage(TEXT_INFO, "Exited  'Typedef'\n");
		}

		private void rdp_load_keywords()
		{
			ScanLoadKeyword("\"", "\\", RDP_T_34 /* " */, SCAN_P_STRING_ESC);
			ScanLoadKeyword("\'", "\\", RDP_T_39 /* ' */, SCAN_P_STRING_ESC);
			ScanLoadKeyword("(", null, RDP_T_40 /* ( */, SCAN_P_IGNORE);
			ScanLoadKeyword(")", null, RDP_T_41 /* ) */, SCAN_P_IGNORE);
			ScanLoadKeyword(",", null, RDP_T_44 /* , */, SCAN_P_IGNORE);
			ScanLoadKeyword(".", null, RDP_T_46 /* . */, SCAN_P_IGNORE);
			ScanLoadKeyword("/", null, RDP_T_47 /* / */, SCAN_P_IGNORE);
			ScanLoadKeyword("/*", "*/", RDP_T_4742 /* / * */, SCAN_P_COMMENT_VISIBLE);
			ScanLoadKeyword("//", null, RDP_T_4747 /* // */, SCAN_P_COMMENT_LINE);
			ScanLoadKeyword(":", null, RDP_T_58 /* : */, SCAN_P_IGNORE);
			ScanLoadKeyword(";", null, RDP_T_59 /* ; */, SCAN_P_IGNORE);
			ScanLoadKeyword("=", null, RDP_T_61 /* = */, SCAN_P_IGNORE);
			ScanLoadKeyword("@", null, RDP_T_64 /* @ */, SCAN_P_IGNORE);
			ScanLoadKeyword("class", null, RDP_T_class, SCAN_P_IGNORE);
			ScanLoadKeyword("enum", null, RDP_T_enum, SCAN_P_IGNORE);
			ScanLoadKeyword("import", null, RDP_T_import, SCAN_P_IGNORE);
			ScanLoadKeyword("interface", null, RDP_T_interface, SCAN_P_IGNORE);
			ScanLoadKeyword("internal", null, RDP_T_internal, SCAN_P_IGNORE);
			ScanLoadKeyword("package", null, RDP_T_package, SCAN_P_IGNORE);
			ScanLoadKeyword("private", null, RDP_T_private, SCAN_P_IGNORE);
			ScanLoadKeyword("protected", null, RDP_T_protected, SCAN_P_IGNORE);
			ScanLoadKeyword("public", null, RDP_T_public, SCAN_P_IGNORE);
			ScanLoadKeyword("static", null, RDP_T_static, SCAN_P_IGNORE);
			ScanLoadKeyword("typedef", null, RDP_T_typedef, SCAN_P_IGNORE);
			ScanLoadKeyword("{", null, RDP_T_123 /* { */, SCAN_P_IGNORE);
			ScanLoadKeyword("}", null, RDP_T_125 /* } */, SCAN_P_IGNORE);
		}

		protected void rdp_set_initialise()
		{
			AccessModifier_first.assignList(RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			AccessModifier_stop.assignList(SCAN_P_ID, SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum,
				RDP_T_interface, RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			Annotation_stop.assignList(SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface,
				RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			Character_stop.assignList(SCAN_P_EOF);
			Class_stop.assignList(SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface, RDP_T_internal,
				RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			CommentLine_stop.assignList(SCAN_P_EOF);
			CommentNest_stop.assignList(SCAN_P_EOF);
			CompilationUnit_first.assignList(RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_import, RDP_T_interface,
				RDP_T_internal, RDP_T_package, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			CompilationUnit_stop.assignList(SCAN_P_EOF);
			Entity_first.assignList(RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface, RDP_T_internal,
				RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			Entity_stop.assignList(SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface,
				RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			Enum_stop.assignList(SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface,
				RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			Import_stop.assignList(SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_import,
				RDP_T_interface, RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			Interface_stop.assignList(SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface,
				RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			InterfaceMethodDeclaration_first.assignList(SCAN_P_ID, RDP_T_internal, RDP_T_private, RDP_T_protected,
				RDP_T_public);
			InterfaceMethodDeclaration_stop.assignList(SCAN_P_ID, SCAN_P_EOF, RDP_T_internal, RDP_T_private,
				RDP_T_protected, RDP_T_public, RDP_T_125 /* } */);
			MethodOrFieldDeclaration_first.assignList(SCAN_P_ID, RDP_T_internal, RDP_T_private, RDP_T_protected,
				RDP_T_public);
			MethodOrFieldDeclaration_stop.assignList(SCAN_P_ID, SCAN_P_EOF, RDP_T_internal, RDP_T_private,
				RDP_T_protected, RDP_T_public, RDP_T_125 /* } */);
			PackageDeclaration_stop.assignList(SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_import,
				RDP_T_interface, RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			Parameters_stop.assignList(SCAN_P_EOF, RDP_T_41 /* ) */);
			QualifiedIdentifier_stop.assignList(SCAN_P_EOF, RDP_T_44 /* , */, RDP_T_59 /* ; */, RDP_T_123 /* { */);
			String_stop.assignList(SCAN_P_EOF);
			Typedef_stop.assignList(SCAN_P_EOF, RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface,
				RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			rdp_Class_4_first.assignList(SCAN_P_ID, RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_Class_5_first.assignList(SCAN_P_ID, RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_CompilationUnit_4_first.assignList(RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface,
				RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			rdp_CompilationUnit_5_first.assignList(RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface,
				RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			rdp_CompilationUnit_6_first.assignList(RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_import,
				RDP_T_interface, RDP_T_internal, RDP_T_package, RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			rdp_Entity_0_first.assignList(RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_Entity_1_first.assignList(RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_Entity_7_first.assignList(RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface, RDP_T_typedef);
			rdp_Entity_8_first.assignList(RDP_T_64 /* @ */, RDP_T_class, RDP_T_enum, RDP_T_interface, RDP_T_internal,
				RDP_T_private, RDP_T_protected, RDP_T_public, RDP_T_typedef);
			rdp_InterfaceMethodDeclaration_0_first.assignList(RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_InterfaceMethodDeclaration_1_first.assignList(RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_InterfaceMethodDeclaration_4_first.assignList(SCAN_P_ID, RDP_T_internal, RDP_T_private, RDP_T_protected,
				RDP_T_public);
			rdp_Interface_0_first.assignList(SCAN_P_ID, RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_Interface_1_first.assignList(SCAN_P_ID, RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_MethodOrFieldDeclaration_0_first.assignList(RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_MethodOrFieldDeclaration_1_first.assignList(RDP_T_internal, RDP_T_private, RDP_T_protected, RDP_T_public);
			rdp_MethodOrFieldDeclaration_6_first.assignList(RDP_T_40 /* ( */, RDP_T_59 /* ; */);
			rdp_MethodOrFieldDeclaration_7_first.assignList(SCAN_P_ID, RDP_T_internal, RDP_T_private, RDP_T_protected,
				RDP_T_public);
		}
	}
}
