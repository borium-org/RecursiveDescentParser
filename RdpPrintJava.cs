using System;
using System.IO;
using static Borium.RDP.CRT;
using static Borium.RDP.RdpAux;
using static Borium.RDP.RdpAux.RdpParamType;
using static Borium.RDP.RdpProgram;
using static Borium.RDP.Set;
using static Borium.RDP.Text;
using static Borium.RDP.Text.TextMessageType;

namespace Borium.RDP
{
	internal class RdpPrintJava : RdpPrint
	{
		/** Path where the files will be written */
		private string outputPath;
		/** Package for all generated classes */
		private string classPackage;

		/// <summary>
		/// Generate Java parser files.
		/// </summary>
		/// <param name="outputPath">Path where the files will be written.</param>
		/// <param name="classPackage">Package for all generated classes.</param>
		public RdpPrintJava(string outputPath, string classPackage)
		{
			this.outputPath = outputPath;
			this.classPackage = classPackage;
		}

		/// <summary>
		/// Actual generating of the output files (parser and tokens).
		/// </summary>
		/// <param name="rdp_base">The root of RDP data structures for the parser</param>
		/// <param name="parser_only">True to generate only the parser, false to generate AST classes</param>
		public void print(SymbolScopeData rdp_base, bool parser_only)
		{
			printKeywordJava();
			printCompilerJava(rdp_base);
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
				string classPath = classPackage.Replace('.', '/');
				Directory.CreateDirectory(outputPath + "/" + classPath);
				file = new StreamWriter(outputPath + "/" + classPath + "/" + className + ".java");
				rdp_indentation = 0;
			}
			catch (FileNotFoundException)
			{
				throw new Exception();
			}
			return file;
		}

		private void printCompilerJava(SymbolScopeData scopeData)
		{
			TextWriter file = createFile("Compiler");
			text_redirect(file);

			iprintln("package " + classPackage + ";");
			iprintln();
			iprintln("import static " + classPackage + ".Keyword.*;");
			iprintln("import static " + classPackage + ".Scanner.*;");
			iprintln("import static " + classPackage + ".Text.*;");
			iprintln("import static " + classPackage + ".Text.TextMessageType.*;");

			iprintln();
			iprintln("import org.borium.jrc.parser.ast.*;");
			iprintln();

			iprintln("public class Compiler extends CompilerBase");
			iprintln("{");
			rdp_indentation++;

			printDeclareAllSets(scopeData);
			printTokenNames();
			printStaticInit();
			printInitializeAllSets(scopeData);
			printConstructor();

			// From now on first approach is to declare all entities that are
			// equivalent to C output counterparts
			printFileNames();
			printIntVariables();
			printParserMethods(scopeData, true);
			printParserMethods(scopeData, false);
			printMainRoutine();

			printLoadKeywords();

			rdp_indentation--;
			iprintln("}");

			text_redirect(Console.Out);
			file.Close();
		}

		private void printConstructor()
		{
			iprintln("public Compiler()");
			iprintln("{");
			iprintln(1, "loadKeywords();");
			iprintln("}");
			iprintln();
		}

		private void printDeclareAllSets(SymbolScopeData scopeData)
		{
			RdpData temp = (RdpData)scopeData.nextSymbolInScope();
			while (temp != null)
			{
				if (rdp_production_set.includes(temp.kind) && temp.code_only == 0)
				{
					if (temp.first_cardinality > 1)
					{
						iprintln("private static final TokenSet " + text_get_string(temp.id) + "_first = new TokenSet();");
					}

					if (temp.kind == K_PRIMARY)
					{
						iprintln("private static final TokenSet " + text_get_string(temp.id) + "_stop = new TokenSet();");
					}
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
			iprintln();
		}

		private void printFileNames()
		{
			// TODO Auto-generated method stub

		}

		private void printInitializeAllSets(SymbolScopeData scopeData)
		{
			iprintln("private static void setInitialize()");
			iprintln("{");
			rdp_indentation++;

			RdpData temp = (RdpData)scopeData.nextSymbolInScope();
			while (temp != null)
			{
				if (rdp_production_set.includes(temp.kind) && temp.code_only == 0)
				{
					if (temp.first_cardinality > 1)
					{
						int initial = iprint(text_get_string(temp.id) + "_first.assignList(");
						temp.first.print(rdp_enum_string, initial, indent, 120, false);
						println(");");
					}

					if (temp.kind == K_PRIMARY)
					{
						int initial = iprint(text_get_string(temp.id) + "_stop.assignList(");
						temp.follow.print(rdp_enum_string, initial, indent, 120, false);
						println(");");
					}
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}

			rdp_indentation--;
			iprintln("}");
			iprintln();
		}

		private void printIntVariables()
		{
			// TODO Auto-generated method stub

		}

		private void printKeywordJava()
		{
			TextWriter file = createFile("Keyword");
			text_redirect(file);

			iprintln("package " + classPackage + ";");
			iprintln();

			iprintln("import static " + classPackage + ".Scanner.*;");
			iprintln();

			iprintln("public interface Keyword");
			iprintln("{");
			rdp_indentation++;
			// iprintln("SCAN_P_EOF(\"<EOF>\"),");
			// iprintln("SCAN_P_ID(\"<Ident>\"),");
			// iprintln("SCAN_P_ERROR(\"<Error>\"),");
			// iprintln("SCAN_P_INTEGER(\"<Integer>\"),");
			// iprintln("SCAN_P_REAL(\"<Float>\"),");
			// RdpData temp = (RdpData) tokens.getScope().nextSymbolInScope();
			// while (temp != null)
			// {
			// if (temp.kind == K_TOKEN || temp.kind == K_EXTENDED)
			// {
			// iprint("");
			// rdp_print_parser_production_name_no_comment(temp);
			// string id = "";
			// string tempId = text_get_string(temp.id);
			// for (char ch : tempId.toCharArray())
			// {
			// if (ch == '"')
			// id += "\\\"";
			// else
			// id += ch;
			// }
			// if (id.equals("\\\""))
			// id = "<string>";
			// if (id.equals("\'"))
			// id = "<Char>";
			// text_printf("(\"" + id + "\"),");
			// iprintln();
			// }
			// temp = (RdpData) temp.nextSymbolInScope();
			// }
			// iprintln("kw__last(null);");
			// iprintln();
			// iprintln("private string text;");
			// iprintln();
			// iprintln("private Keyword(string string)");
			// iprintln("{");
			// iprintln("\ttext = string;");
			// iprintln("}");
			// iprintln();
			// iprintln("@Override");
			// iprintln("public string toString()");
			// iprintln("{");
			// iprintln("\treturn text;");
			// iprintln("}");

			////// C
			RdpData temp = (RdpData)tokens.getScope().nextSymbolInScope();
			iprintln("int RDP_TT_BOTTOM = SCAN_P_TOP;");
			int offset = 0;
			while (temp != null)
			{
				if (temp.kind == K_TOKEN || temp.kind == K_EXTENDED)
				{
					iprint("int ");
					rdp_print_parser_production_name_no_comment(temp);
					println(" = SCAN_P_TOP + " + offset + ";");
					offset++;
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
			iprintln("int RDP_TT_TOP = SCAN_P_TOP + " + offset + ";");
			////// C

			rdp_indentation--;
			iprintln("}");
			text_redirect(Console.Out);
			file.Close();
		}

		private void printLoadKeywords()
		{
			iprintln("private void loadKeywords()");
			iprintln("{");
			rdp_indentation++;

			RdpData temp = (RdpData)tokens.getScope().nextSymbolInScope();
			while (temp != null)
			{
				if (temp.kind == K_TOKEN || temp.kind == K_EXTENDED)
				{
					iprint("scanLoadKeyword(\"");
					rdp_print_parser_string(text_get_string(temp.id));
					print("\", ");

					if (temp.close != null)
					{
						print("\"");
						rdp_print_parser_string(temp.close);
						print("\", ");
					}
					else
					{
						print("null, ");
					}
					string token = text_get_string(temp.token_enum).Split(' ')[0];
					print(token + ", ");
					print(temp.extended_enum);
					println(");");
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}

			rdp_indentation--;
			iprintln("}");
		}

		private void printMainRoutine()
		{
			// TODO Auto-generated method stub

		}

		private void printParserMethods(SymbolScopeData scopeData, bool start)
		{
			// print parser definitions using rdp_print_parser_primaries(base);

			for (RdpData temp = (RdpData)scopeData.nextSymbolInScope(); temp != null; temp = (RdpData)temp.nextSymbolInScope())
			{
				if (temp.kind == K_PRIMARY && temp.call_count > 0 && temp.code_only == 0)
				{
					if (temp == rdp_start_prod && !start)
						continue;
					if (temp != rdp_start_prod && start)
						continue;
					bool is_void = temp.return_type == "void" && temp.return_type_stars == 0;
					if (!is_void)
						throw new Exception("Non-void " + text_get_string(temp.id));

					iprint(temp == rdp_start_prod ? "public " : "protected ");
					bool parserOnly = rdp_parser_only.value();
					string astType = "Ast" + capitalizeFirst(text_get_string(temp.id));
					print(parserOnly ? "void" : astType);
					text_printf(" " + text_get_string(temp.id));

					rdp_print_parser_param_list(null, temp.parameters, 1, 0);
					println();
					iprintln("{");
					rdp_indentation++;

					if (temp.ll1_violation != 0)
					{
						iprintln("// WARNING - an LL(1) violation was detected at this point in the grammar");
					}

					if (!parserOnly)
					{
						iprintln(astType + " ast = new " + astType + "();");
						iprintln();
					}

					// In trace mode, add an entry message
					if (rdp_trace.value())
					{
						iprintln("text_message(TEXT_INFO, \"Entered \'" + text_get_string(temp.id) + "\'\\n\");");
						iprintln();
					}

					rdp_print_parser_alternate(temp, temp);

					// add error handling on exit
					iprintln("scan_test_set("
							+ (rdp_error_production_name.value() ? "\"" + text_get_string(temp.id) + "\"" : "null") + ", "
							+ text_get_string(temp.id) + "_stop, " + text_get_string(temp.id) + "_stop);");

					iprintln();

					// In trace mode, add an exit message
					if (rdp_trace.value())
					{
						iprintln("text_message(TEXT_INFO, \"Exited \'" + text_get_string(temp.id) + "\'\\n\");");
						iprintln();
					}

					if (!parserOnly)
					{
						iprintln("return ast;");
					}

					rdp_indentation--;
					iprintln("}");
					println();
				}
			}
		}

		private void printStaticInit()
		{
			iprintln("static");
			iprintln("{");
			iprintln(1, "setInitialize();");
			iprintln("}");
			iprintln();
		}

		private void printTokenNames()
		{
			int column = iprint("public static final String[] rdp_tokens = {");

			for (int token_count = 0; token_count < rdp_token_count; token_count++)
			{
				string tokenString = rdp_token_string[token_count];
				if (column + tokenString.Length + 4 >= 119)
				{
					println();
					column = indent(2);
				}
				else
					column += print(" ");
				column += print("\"" + tokenString + "\",");
			}
			println(" };");
			println();
		}

		private void rdp_print_parser_alternate(RdpData production, RdpData primary)
		{
			RdpList list = production.list;
			if (list.next == null)
			{
				rdp_print_parser_sequence(list.production, primary);
			}
			else
			{
				bool elsePrinted = false;
				while (list != null)
				{
					if (list.production.kind != K_SEQUENCE)
					{
						text_message(TEXT_FATAL, "internal error - expecting alternate\n");
					}
					if (elsePrinted)
						print(" if (");
					else
						iprint("if (");
					rdp_print_parser_test(list.production.id, list.production.first, null);
					println(")");
					iprintln("{");
					rdp_indentation++;

					rdp_print_parser_sequence(list.production, primary);

					rdp_indentation--;
					iprintln("}");

					if ((list = list.next) != null)
					{
						iprint("else");
						elsePrinted = true;
					}
					else
					/* tail test at end of alternates */
					if (!(production.contains_null && production.lo != 0))
					{
						iprintln("else");
						indent(1);
						rdp_print_parser_test(production.id, production.first, text_get_string(primary.id));
						println(";");
					}
				}
			}
		}

		private void rdp_print_parser_item(RdpData prod, RdpData primary, string return_name, RdpParamList actuals,
				int promote_epsilon, int promote, string default_action)
		{
			if (promote == PROMOTE_DEFAULT)
			{
				promote = prod.promote_default;
			}

			if (!(prod.kind == K_CODE && prod.code_successor != 0))
			{
				indent(); /* Don't indent code sequence-internal or inline items */
			}

			switch (prod.kind)
			{
				case K_INTEGER:
				case K_REAL:
				case K_STRING:
				case K_EXTENDED:
				case K_TOKEN:
					if (rdp_dir_tree != 0)
					{
						if (promote == PROMOTE_DONT)
						{
							/* add a tree node for this scanner item as child of current parent */
							text_printf("if (rdp_tree_update) rdp_add_child(null, rdp_tree);\n");
							indent();
						}
						else if (promote == PROMOTE_AND_COPY)
						{
							/* copy scanner data to current tree parent */
							text_printf("if (rdp_tree_update) memcpy(rdp_tree, text_scan_data, sizeof(scan_data));\n");
							indent();
						}
						else if (promote == PROMOTE_ABOVE)
						{
							/* add a tree node for this scanner item as parent of current parent */
							text_printf("if (rdp_tree_update) rdp_add_parent(null, rdp_tree);\n");
							indent();
						}
					}
					text_printf("scan_test("
							+ (rdp_error_production_name.value() ? "\"" + text_get_string(primary.id) + "\"" : "null") + ", ");
					rdp_print_parser_production_name(prod);
					text_printf(", " + text_get_string(primary.id) + "_stop);\n");
					indent();
					/* disable if -p option used */
					if (return_name != null && !rdp_parser_only.value())
					{
						text_printf(return_name + " = text_scan_data."
								+ (prod.kind == K_REAL ? "data.r" : prod.kind == K_INTEGER ? "data.i" : "id") + ";\n");
						indent();
					}
					text_printf("scan_();\n");
					break;
				case K_CODE:
					if (!rdp_parser_only.value()) /* disabled by -p option */
					{
						if (prod.code_pass != 0)
						{
							text_printf("if (rdp_pass == " + prod.code_pass + ") { \\\n");
						}
						string temp = text_get_string(prod.id);
						foreach (char ch in temp)
						{
							if (ch == '\n')
							{
								text_printf("\\\n");
							}
							else if (isprint(ch))
							{
								text_printf("" + ch);
							}
						}

						if (prod.code_pass != 0)
						{
							text_printf(" \\\n}");
						}

						if (prod.kind == K_CODE && prod.code_terminator != 0)
						{
							text_printf("\n"); /* terminate semantic actions tidily */
						}
					}
					break;
				case K_PRIMARY:
					if (rdp_dir_tree != 0 && promote == PROMOTE_AND_COPY)
					{
						text_printf("if(rdp_tree_update) {rdp_tree.id = \"" + text_get_string(prod.id)
								+ "\"; rdp_tree.token = 0;}\n");
					}
					if (return_name != null && !rdp_parser_only.value())
					{
						text_printf(return_name + " = ");
					}
					text_printf(text_get_string(prod.id));
					if (prod.code_only == 0 && actuals == null)
					{
						rdp_print_parser_param_list(promote == PROMOTE_DONT ? text_get_string(prod.id) : null, actuals, 0, 0);
					}
					text_printf(";\n");
					break;
				case K_SEQUENCE:
					text_message(TEXT_FATAL, "internal error - unexpected alternate in sequence\n");
					break;
				case K_LIST:
					rdp_print_parser_subproduction(prod, primary, promote_epsilon, default_action);
					break;
				default:
					text_message(TEXT_FATAL, "internal error - unexpected kind found\n");
					break;
			}
		}

		private void rdp_print_parser_param_list(string first, RdpParamList parameters, int definition, int start_rule)
		{
			// print("(");
			//
			// /* processing for tree parameter */
			// if (rdp_dir_tree != 0)
			// throw new RuntimeException("rdp_dir_tree not 0");
			//
			// if (params == null && definition != 0 && rdp_dir_tree == 0)
			// {
			// }
			// else
			// {
			// throw new RuntimeException("param list not null");
			// }
			//
			// print(")");
			text_printf("(");

			/* processing for tree parameter */
			if (rdp_dir_tree != 0)
			{
				if (definition != 0)
				{
					text_printf("rdp_tree_node_data* rdp_tree");
				}
				else
				{
					if (first == null)
					{
						text_printf("rdp_tree");
					}
					else
					{
						text_printf((start_rule != 0 ? "rdp_tree_root = " : "") + "rdp_add_"
								+ (start_rule != 0 ? "node" : "child") + "(\"" + first + "\", rdp_tree)");
					}
				}

				if (parameters != null)
				{
					text_printf(", "); /* put in separator for rest of parameters */
				}
			}

			if (parameters == null && definition != 0 && rdp_dir_tree == 0)
			{
				text_printf("");
			}
			else
			{
				rdp_print_parser_param_list_sub(parameters, 1, definition);
			}

			text_printf(")");
		}

		private void rdp_print_parser_param_list_sub(RdpParamList param, int last, int definition)
		{
			if (param != null)
			{
				rdp_print_parser_param_list_sub(param.next, 0, definition);
				text_printf(definition != 0 ? param.type : "");

				if (definition != 0)
				{
					for (int count = 0; count < param.stars; count++)
					{
						text_printf("*");
					}
				}

				text_printf(definition != 0 ? " " : "");
				switch (param.flavour)
				{
					case PARAM_INTEGER:
						text_printf($"{param.n}");
						break;
					case PARAM_REAL:
						text_printf($"{param.r}");
						break;
					case PARAM_STRING:
						text_printf("\"" + param.id + "\"");
						break;
					case PARAM_ID:
						text_printf(param.id);
						break;
				}
				text_printf(last != 0 ? "" : ", ");
			}
		}

		private void rdp_print_parser_sequence(RdpData production, RdpData primary)
		{
			RdpList list = production.list;

			while (list != null)
			{
				rdp_print_parser_item(list.production, primary, list.return_name, list.actuals, list.promote_epsilon,
						list.promote, list.default_action);
				list = list.next;
			}
		}

		private void rdp_print_parser_subproduction(RdpData prod, RdpData primary, int promote_epsilon,
				string default_action)
		{
			if (prod.lo == 0) /* this can be an optional body */
			{
				text_printf("if (");
				rdp_print_parser_test(prod.id, prod.first, null);
				text_printf(")\n");
				indent();
			}

			println("{");
			rdp_indentation++;
			iprintln("// Start of " + text_get_string(prod.id));

			if (prod.ll1_violation != 0)
			{
				iprintln("// FIXME - an LL(1) violation was detected at this point in the grammar");
			}

			/* We don't need to instantiate count if hi is infinity and lo is 0 or 1 */
			if (!((prod.hi == 0 || prod.hi == 1) && (prod.lo == 1 || prod.lo == 0)))
			{
				indent();
				text_printf("unsigned long rdp_count = 0;\n");
			}

			iprintln("while (true)");
			iprintln("{");
			rdp_indentation++;

			/* Put in test that first element of body matches if iterator low count > 0 and prod isn't nullable */
			if (prod.lo != 0 && !prod.contains_null)
			{
				indent();
				rdp_print_parser_test(prod.id, prod.first, text_get_string(primary.id));
				println(";");
			}

			rdp_print_parser_alternate(prod, primary);

			if (!((prod.hi == 0 || prod.hi == 1) && (prod.lo == 1 || prod.lo == 0)))
			{
				iprintln("rdp_count++;");
			}

			if (prod.hi > 1) /* Don't bother testing rdp_count of hi is zero or infty */
			{
				iprintln("if (rdp_count == " + prod.hi + ")");
				iprintln(1, "break;");
			}

			if (prod.supplementary_token != null)
			{
				iprintln("if (text_scan_data.token != " + text_get_string(prod.supplementary_token.token_enum) + ")");
				iprintln(1, "break;");

				if (rdp_dir_tree != 0)
				{
					if (prod.delimiter_promote == PROMOTE_DONT)
					{
						/* add a tree node for this scanner item */
						text_printf("if (rdp_tree_update) rdp_add_child(null, rdp_tree);\n");
						indent();
					}
					else if (prod.delimiter_promote == PROMOTE_AND_COPY)
					{
						/* copy scanner data to current tree parent */
						text_printf("if (rdp_tree_update) memcpy(rdp_tree, text_scan_data, sizeof(scan_data));\n");
						indent();
					}
				}

				iprintln("scan_();"); /* skip list token */
			}
			else if (prod.hi != 1)
			{
				iprint("if (!");
				rdp_print_parser_test(prod.id, prod.first, null);
				println(")");
				iprintln(1, "break;");
			}

			if (prod.hi == 1)
			{
				iprintln("break; // hi limit is 1");
			}

			rdp_indentation--;
			iprintln("}");

			if (prod.lo > 1) /* test rdp_count on way out */
			{
				indent();
				text_printf("if (rdp_count < " + prod.lo + ")");
				text_printf("  text_message(TEXT_ERROR_ECHO, \"iteration count too low\\n\");\n");
			}

			iprintln("// End of " + text_get_string(prod.id));
			rdp_indentation--;
			iprintln("}");

			if (prod.lo == 0 && (rdp_dir_tree != 0 || default_action != null))
			{
				iprintln("else");
				indent();
				iprintln("{");
				rdp_indentation++;
				indent();
				text_printf("/* default action processing for " + text_get_string(prod.id) + "*/\n");
				if (rdp_dir_tree != 0)
				{
					/* First do tree node handling */
					if (promote_epsilon == PROMOTE_DONT)
					{
						/* add an epsilon tree node */
						indent();
						if (rdp_dir_annotated_epsilon_tree != 0)
						{
							text_printf(
									"if (rdp_tree_update) {rdp_tree_node_data *temp = rdp_add_child(null, rdp_tree); temp->id = \"#: "
											+ text_get_string(prod.id + 4) + "\"; temp.token = SCAN_P_ID;}\n");
						}
						else
						{
							text_printf(
									"if (rdp_tree_update) {rdp_tree_node_data *temp = rdp_add_child(null, rdp_tree); temp->id = null; temp->token = SCAN_P_ID;}\n");
						}
					}
					else if (promote_epsilon == PROMOTE_AND_COPY)
					{
						/* copy epsilon to current tree parent */
						indent();
						if (rdp_dir_annotated_epsilon_tree != 0)
						{
							text_printf("if (rdp_tree_update) {rdp_tree->id = \"#: " + text_get_string(prod.id + 4)
									+ "\"; rdp_tree.token = SCAN_P_ID;}\n");
						}
						else
						{
							text_printf("if (rdp_tree_update) {rdp_tree->id = null; rdp_tree->token = SCAN_P_ID;}\n");
						}
					}
				}

				/* Now copy out default action */
				/* disabled by -p option */
				if (!rdp_parser_only.value() && default_action != null)
				{
					foreach (char ch in default_action)
					{
						if (ch == '\n')
						{
							text_printf(" \\\n");
						}
						else
						{
							text_printf("" + ch);
						}
					}
					println(); /* terminate semantic actions tidily */
				}
				rdp_indentation--;
				iprintln("}");
			}
		}

		private void rdp_print_parser_test(int first_name, Set first, string follow_name)
		{
			text_printf("scan_test");

			switch (set_cardinality(first))
			{
				default:
					text_printf(
							"_set(" + (rdp_error_production_name.value() ? "\"" + text_get_string(first_name) + "\"" : "null")
									+ ", " + text_get_string(first_name) + "_first");
					break;
				case 1:
					text_printf("(" + (rdp_error_production_name.value() ? "\"" + text_get_string(first_name) + "\"" : "null")
							+ ", ");
					first.print(rdp_enum_string, 120);
					break;
				case 0:
					Console.Error.WriteLine("Set " + first + " is empty");
					break;
			}

			if (follow_name == null)
			{
				text_printf(", null)");
			}
			else
			{
				text_printf(", " + follow_name + "_stop)");
			}
		}
	}
}
