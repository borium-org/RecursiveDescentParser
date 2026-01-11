using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using static Borium.RDP.Arg;
using static Borium.RDP.Arg.ArgKind;
using static Borium.RDP.RdpGram;
using static Borium.RDP.RdpPrint;
using static Borium.RDP.RdpProgram;
using static Borium.RDP.Scan;
using static Borium.RDP.Set;
using static Borium.RDP.Symbol;
using static Borium.RDP.Text;
using static Borium.RDP.Text.TextMessageType;

namespace Borium.RDP
{
	internal class RdpAux
	{
		internal class LocalsData : Symbol
		{
		}

		internal class RdpArgList
		{
			internal ArgKind kind;
			internal string var;
			internal string key;
			internal string desc;
			internal RdpArgList next;
		}

		internal class RdpData : Symbol
		{
			internal int token;

			/// <summary>
			/// Token value for tokens
			/// </summary>
			internal int token_value;

			/// <summary>
			/// Extended value for tokens
			/// </summary>
			internal int extended_value;

			internal int kind;

			/// <summary>
			/// Return_type name
			/// </summary>
			internal string return_type;

			/// <summary>
			/// Number of indirections in return type
			/// </summary>
			internal int return_type_stars;

			/// <summary>
			/// Pointer to token value as a string
			/// </summary>
			internal int token_string;

			/// <summary>
			/// Pointer to token value as enum element
			/// </summary>
			internal int token_enum;

			/// <summary>
			/// Pointer to extended value as enum element
			/// </summary>
			internal string extended_enum;

			/// <summary>
			/// Default promotion operator
			/// </summary>
			internal int promote_default;

			/// <summary>
			/// Promotion operator for inline calls
			/// </summary>
			internal int promote;

			/// <summary>
			/// Promotion operator for iterator delimiters
			/// </summary>
			internal int delimiter_promote;

			///Flag to suppress unused production warning if production contains only comments
			internal bool comment_only;

			/// <summary>
			/// For quick first calculation
			/// </summary>
			internal bool contains_null;

			/// <summary>
			/// Production has inherited attributes
			/// </summary>
			internal bool parameterised;

			/// <summary>
			/// Mark items that follow a code item
			/// </summary>
			internal int code_successor;

			/// <summary>
			/// Mark last code item in sequence
			/// </summary>
			internal int code_terminator;

			/// <summary>
			/// Primary production with code only
			/// </summary>
			internal int code_only;

			/// <summary>
			/// Has appeared on LHS of ::=
			/// </summary>
			internal int been_defined;

			/// <summary>
			/// Production being checked flag
			/// </summary>
			internal int in_use;

			/// <summary>
			/// ll(1) violation detected
			/// </summary>
			internal int ll1_violation;

			/// <summary>
			/// first() completed on this production
			/// </summary>
			internal int first_done;

#if false
			/** follow() completed on this production */
			int follow_done;
#endif
			/// <summary>
			/// Set of first symbols
			/// </summary>
			internal Set first = new Set();

			/// <summary>
			/// How many times production is called
			/// </summary>
			internal int call_count;

			/// <summary>
			/// Number of elements in first set
			/// </summary>
			internal int first_cardinality;

			/// <summary>
			/// set of follow symbols
			/// </summary>
			internal Set follow = new Set();

			/// <summary>
			/// Number of elements in follow set
			/// </summary>
			internal int follow_cardinality;

			/// <summary>
			/// Active parser pass for code element
			/// </summary>
			internal int code_pass;

			/// <summary>
			/// Minimum iteration count
			/// </summary>
			internal int lo;

			/// <summary>
			/// Maximum iteration count
			/// </summary>
			internal int hi;

			/// <summary>
			/// List of parameter names (and types)
			/// </summary>
			internal RdpParamList parameters;

			/// <summary>
			/// List of actuals filled in by item_ret
			/// </summary>
			internal RdpParamList actuals;

			/// <summary>
			/// List of alternatives or items
			/// </summary>
			internal RdpList list;

			/// <summary>
			/// Spare token pointer
			/// </summary>
			internal RdpData supplementary_token;

			/// <summary>
			/// Extended keyword close string
			/// </summary>
			internal string close;

			internal List<string> locals = new List<string>();

			internal void rdp_print_sub_item(bool expand)
			{
				switch (kind)
				{
					case K_INTEGER:
					case K_STRING:
					case K_REAL:
					case K_EXTENDED:
						text_printf(text_get_string(id) + " ");
						break;
					case K_TOKEN:
						text_printf("\'" + text_get_string(id) + "\' ");
						break;
					case K_CODE:
						/* Don't print anything */
						break;
					case K_PRIMARY:
						text_printf(text_get_string(id) + " ");
						break;
					case K_SEQUENCE:
						rdp_print_sub_sequence(expand);
						break;
					case K_LIST:
						if (expand)
						{
							/* first find special cases */
							/* All EBNF forms have no delimiter */
							if (supplementary_token == null)
							{
								if (lo == 0 && hi == 0)
								{
									text_printf("{ ");
									rdp_print_sub_alternate(expand);
									text_printf("} ");
								}
								else if (lo == 0 && hi == 1)
								{
									text_printf("[ ");
									rdp_print_sub_alternate(expand);
									text_printf("] ");
								}
								else if (lo == 1 && hi == 0)
								{
									text_printf("< ");
									rdp_print_sub_alternate(expand);
									text_printf("> ");
								}
								else if (lo == 1 && hi == 1)
								{
									text_printf("( ");
									rdp_print_sub_alternate(expand);
									text_printf(") ");
								}
							}
							else
							{ /* Now do general case */
								text_printf("( ");
								rdp_print_sub_alternate(expand);
								text_printf(")" + lo + "@" + hi);
								if (supplementary_token != null)
								{
									text_printf(" \'" + text_get_string(supplementary_token.id) + "\'");
								}
								else
								{
									text_printf(" #");
								}
							}
						}
						else
						{
							text_printf(text_get_string(id) + " ");
						}
						break;
					default:
						text_message(TEXT_FATAL, "internal error - unexpected kind found\n");
						break;
				}
			}

			private void rdp_print_sub_alternate(bool expand)
			{
				RdpList list = this.list;
				while (list != null)
				{
					list.production.rdp_print_sub_item(expand);
					if ((list = list.next) != null)
					{
						text_printf("| ");
					}
				}
			}

			private void rdp_print_sub_sequence(bool expand)
			{
				RdpList list = this.list;
				while (list != null)
				{
					list.production.rdp_print_sub_item(expand);
					list = list.next;
				}
			}
		}

		internal class RdpList
		{
			internal string return_name;

			internal RdpData production;

			/// <summary>
			/// List of actuals used by production call
			/// </summary>
			internal RdpParamList actuals;

			internal RdpList next;

			/// <summary>
			/// Promotion operator for this node
			/// </summary>
			internal int promote;

			/// <summary>
			/// Promotion operator for epsilons generated by this node
			/// </summary>
			internal int promote_epsilon;

			/// <summary>
			/// Action to be executed of lo=0 and body not taken
			/// </summary>
			internal string default_action;
		}

		internal class RdpParamList
		{
			internal string id;
			internal int n;
			internal double r;
			internal string type;
			internal int stars;
			internal RdpParamType flavour;
			internal RdpParamList next;
		}

		internal enum RdpParamType
		{
			PARAM_ID, PARAM_STRING, PARAM_REAL, PARAM_INTEGER
		}

		internal class RdpStringList
		{
			internal string str1;
			internal string str2;
			internal RdpStringList next;
		}

		internal class RdpTableList
		{
			internal string name;
			internal int size;
			internal int prime;
			internal string compare;
			internal string hash;
			internal string print;
			internal string data_fields;
			internal RdpTableList next;
		}

		internal const int K_EXTENDED = 0;
		internal const int K_INTEGER = 1;
		internal const int K_REAL = 2;
		internal const int K_STRING = 3;
		internal const int K_CODE = 4;
		internal const int K_TOKEN = 5;
		internal const int K_PRIMARY = 6;
		internal const int K_SEQUENCE = 7;
		internal const int K_LIST = 8;

		internal const int RDP_OLD = 0;
		internal const int RDP_NEW = 1;
		internal const int RDP_ANY = 2;

		internal const int PROMOTE_DONT = 0;
		internal const int PROMOTE_DEFAULT = 1;
		internal const int PROMOTE = 2;
		internal const int PROMOTE_AND_COPY = 3;
		internal const int PROMOTE_ABOVE = 4;

		/// <summary>
		/// Force output files flag
		/// </summary>
		public static Pointer<bool> rdp_force = new Pointer<bool>(false);

		/// <summary>
		/// Flag to force writing of production name into error messages
		/// </summary>
		public static Pointer<bool> rdp_error_production_name = new Pointer<bool>(false);

		/// <summary>
		/// Flag to generate expanded bnf listing
		/// </summary>
		public static Pointer<bool> rdp_expanded = new Pointer<bool>(false);

		/// <summary>
		/// Omit semantic actions flag
		/// </summary>
		public static Pointer<bool> rdp_parser_only = new Pointer<bool>(false);

		/// <summary>
		/// Add trace messages flag
		/// </summary>
		public static Pointer<bool> rdp_trace = new Pointer<bool>(false);

		/// <summary>
		/// Symbol table for the parser
		/// </summary>
		private static SymbolScopeData rdp_base;

		/** string from OUTPUT_FILE directive */
		internal static string rdp_dir_output_file = null;

		internal static Set rdp_production_set = new Set();

		/** data from ARG_* directives */
		internal static RdpArgList rdp_dir_args = null;

		/// <summary>
		/// Convert symbols flag
		/// </summary>
		private static bool rdp_undeclared_symbols_are_tokens;

		/// <summary>
		/// Sub-production component number
		/// </summary>
		internal static int rdp_component;

		/// <summary>
		/// Number of rules declared * 2
		/// </summary>
		internal static int rdp_rule_count = 0;

		/// <summary>
		/// Flag to track productions that contain only comments
		/// </summary>
		internal static bool rdp_comment_only;

		/// <summary>
		/// Identifier for parent production
		/// </summary>
		internal static string rdp_primary_id;

		/// <summary>
		/// Starting production
		/// </summary>
		internal static RdpData rdp_start_prod;

		/// <summary>
		/// Data from SYMBOL_TABLE directives
		/// </summary>
		internal static RdpTableList rdp_dir_symbol_table = null;

		/// <summary>
		/// Strings from INCLUDE directives
		/// </summary>
		internal static RdpStringList rdp_dir_include = null;

		/// <summary>
		/// String from PRE_PARSE directive
		/// </summary>
		internal static string rdp_dir_pre_parse = null;

		/// <summary>
		/// String from POST_PARSE directive
		/// </summary>
		internal static string rdp_dir_post_parse = null;

		/// <summary>
		/// string from GLOBAL directive
		/// </summary>
		internal static string rdp_dir_global = null;

		/// <summary>
		/// String from TITLE directive
		/// </summary>
		internal static string rdp_dir_title = "rdparser";

		/// <summary>
		/// String from SUFFIX directive
		/// </summary>
		internal static string rdp_dir_suffix = "";

		/// <summary>
		/// MAX_ERRORS directive
		/// </summary>
		internal static int rdp_dir_max_errors = 25;

		/// <summary>
		/// MAX_WARNINGS directive
		/// </summary>
		internal static int rdp_dir_max_warnings = 100;

		/// <summary>
		/// PASSES directive
		/// </summary>
		internal static int rdp_dir_passes = 1;

		/// <summary>
		/// MULTIPLE_SOURCE_FILES flag
		/// </summary>
		internal static int rdp_dir_multiple_source_files = 0;

		/// <summary>
		/// TAB_WIDTH directive
		/// </summary>
		internal static int rdp_dir_tab_width = 8;

		/// <summary>
		/// TEXT_SIZE directive
		/// </summary>
		internal static int rdp_dir_text_size = 350000;

		/// <summary>
		/// DERIVATION_TREE flag
		/// </summary>
		internal static int rdp_dir_derivation_tree = 0;

		/// <summary>
		/// EPSILON_TREE tree flag
		/// </summary>
		internal static int rdp_dir_epsilon_tree = 0;

		/// <summary>
		/// ANNOTATED_EPSILON_TREE tree flag
		/// </summary>
		internal static int rdp_dir_annotated_epsilon_tree = 0;

		/// <summary>
		/// TREE flag
		/// </summary>
		internal static int rdp_dir_tree = 0;

		/// <summary>
		/// Field names for tree edge from TREE directive
		/// </summary>
		internal static string rdp_dir_tree_edge_fields = "";

		/// <summary>
		/// Field names for tree node from TREE directive
		/// </summary>
		internal static string rdp_dir_tree_node_fields = "";

		/// <summary>
		/// CASE_INSENSITIVE flag
		/// </summary>
		internal static int rdp_dir_case_insensitive = 0;

		/// <summary>
		/// SHOW_SKIPS flag
		/// </summary>
		internal static int rdp_dir_show_skips = 0;

		/// <summary>
		/// RETAIN_COMMENTS directive
		/// </summary>
		internal static int rdp_dir_retain_comments;

		/// <summary>
		/// NEWLINE_VISIBLE flag
		/// </summary>
		internal static int rdp_dir_newline_visible = 0;

		/// <summary>
		/// number of tokens + extendeds
		/// </summary>
		internal static int rdp_token_count = SCAN_P_TOP;

		internal static void rdp_add_arg(ArgKind kind, string key, string var, string desc)
		{
			RdpArgList temp = new RdpArgList();

			temp.kind = kind;
			temp.key = key;
			temp.var = var;
			temp.desc = desc;
			temp.next = rdp_dir_args;
			rdp_dir_args = temp;
		}

		internal static RdpData rdp_find(int id, int kind, int symbol)
		{
			return rdp_find(text_get_string(id), kind, symbol);
		}

		internal static RdpData rdp_find(string id, int kind, int symbol)
		{
			SymbolTable table;
			// Figure out which table to use
			switch (kind)
			{
				case K_CODE:
					table = codes;
					break;
				case K_TOKEN:
				case K_EXTENDED:
					table = tokens;
					break;
				default:
					table = rdp;
					break;
			}
			RdpData temp = null;
			if ((temp = (RdpData)symbol_lookup_key(table, id, null)) == null)
			{
				if (symbol == RDP_OLD && rdp_undeclared_symbols_are_tokens)
				{
					text_message(TEXT_WARNING_ECHO, "Undeclared symbol \'" + id + "\' converted to token\n");
					rdp_process_token(id);
				}
				else
				{
					if (symbol == RDP_OLD)
					{
						text_message(TEXT_ERROR_ECHO, "Undeclared symbol \'" + id + "\'\n");
					}
					temp = new RdpData();
					temp.id = text_insert_string(id);
					symbol_insert_symbol(table, temp);
					temp.token = SCAN_P_ID;
					temp.kind = kind;
					temp.hi = temp.lo = 1; /* set instance numbers to one */
					temp.first_cardinality = 0;
					temp.follow.assign(SCAN_P_EOF);
					temp.follow_cardinality = 1;
					temp.return_type_stars = 0;
					switch (kind)
					{
						case K_INTEGER:
							temp.return_type = "long int";
							break;
						case K_REAL:
							temp.return_type = "double";
							break;
						case K_TOKEN:
						case K_STRING:
							temp.return_type = "char";
							temp.return_type_stars = 1;
							break;
						default:
							temp.return_type = "void";
							break;
					}
				}
			}
			else if (symbol == RDP_NEW)
			{
				text_message(TEXT_ERROR_ECHO, "Doubly declared symbol \'" + id + "\'\n");
			}
			return temp;
		}

		internal static RdpData rdp_find_extended(string open, string close, int token)
		{

			rdp_check_token_valid(open);
			rdp_check_token_valid(close);
			RdpData result = rdp_find(open, K_EXTENDED, RDP_ANY);
			result.token_value = token;
			result.close = close;
			result.return_type = "char";
			result.return_type_stars = 1;
			return result;
		}

		internal static void rdp_post_parse(string outputfilename, bool force)
		{
			LocalsData local = new LocalsData();
			local.id = text_insert_string("result");
			symbol_insert_symbol(locals, local);
			// sort productions into alphabetical order
			SymbolScopeData tokens_base = tokens.getScope();
			tokens_base.sort();
			// scan through tokens and add any necessary continuations
			rdp_add_continuations(tokens_base);
			// re-sort productions into alphabetical order
			tokens_base.sort();
			// apply token numbers to token productions
			rdp_order_tokens(tokens_base);

			rdp_base.assign(rdp.getScope());
			// apply token numbers to token productions
			rdp_order_tokens(rdp_base);
			// make a string with all token names in it
			rdp_make_token_string(tokens_base);
			// sort productions into alphabetical order
			rdp_base.sort();
			// find the non-LL(1)-isms
			rdp_bad_grammar(rdp_base);
			if (rdp_expanded.value())
			{
				if (rdp_c_path.value() != null)
				{
					RdpPrintC print = new RdpPrintC();
					print.rdp_dump_extended(rdp_base);
				}
			}
			if (text_total_errors() > 0)
			{
				if (force)
					text_message(TEXT_WARNING, "Grammar is not LL(1) but -F set: writing files\n");
				else
					text_message(TEXT_FATAL, "Run aborted without creating output files - rerun with -F to override\n");
			}
			if (rdp_c_path.value() != null)
			{
				RdpPrintC print = new RdpPrintC();
				print.printHeader(text_force_filetype(outputfilename, "h"));
				print.printParser(text_force_filetype(outputfilename, "c"), rdp_base);
			}
#if false
			// if (rdp_cpp_path.value() != null)
			// {
			// // TODO C++
			// }
#endif
			if (rdp_java_path.value() != null && rdp_java_prefix.value() != null)
			{
				RdpPrintJava print = new RdpPrintJava(rdp_java_path.value(), rdp_java_prefix.value());
				print.print(rdp_base, rdp_parser_only.value());
			}
			if (rdp_csharp_path.value() != null && rdp_csharp_package.value() != null)
			{
				RdpPrintCsharp print = new RdpPrintCsharp(rdp_csharp_path.value(), rdp_csharp_package.value());
				print.print(rdp_base, rdp_parser_only.value());
			}
#if false
			if (rdp_verbose.value() || true)
			{
				text_print_statistics();
			}
#endif
		}

		internal static void rdp_pre_parse()
		{
			rdp_dir_output_file = text_force_filetype(rdp_sourcefilename, "out");
			rdp_base = symbol_new_scope(rdp, "parser");
			rdp_production_set.assignList(K_PRIMARY, K_SEQUENCE, K_LIST);

			rdp_add_arg(ARG_BLANK, null, null, "");
			rdp_add_arg(ARG_BOOLEAN, "f", "rdp_filter", "Filter mode (read from stdin and write to stdout)");
			rdp_add_arg(ARG_BOOLEAN, "l", "rdp_line_echo", "Make a listing");
			rdp_add_arg(ARG_BOOLEAN, "L", "rdp_lexicalise", "Print lexicalised source file");
			rdp_add_arg(ARG_STRING, "o", "rdp_outputfilename", "Write output to filename");
			rdp_add_arg(ARG_BOOLEAN, "s", "rdp_symbol_echo", "Echo each scanner symbol as it is read");
			rdp_add_arg(ARG_BOOLEAN, "S", "rdp_symbol_statistics", "Print summary symbol table statistics");
			rdp_add_arg(ARG_NUMERIC, "t", "rdp_tabwidth", "Tab expansion width (default 8)");
			rdp_add_arg(ARG_NUMERIC, "T", "rdp_textsize", "Text buffer size in bytes for scanner (default 20000)");
			rdp_add_arg(ARG_BOOLEAN, "v", "rdp_verbose", "Set verbose mode");
			rdp_add_arg(ARG_STRING, "V", "rdp_vcg_filename", "Write derivation tree to filename in VCG format");

			/* add predefined primitive productions */
			rdp_find("ID", K_STRING, RDP_ANY).token_value = SCAN_P_ID;
			rdp_find("INTEGER", K_INTEGER, RDP_ANY).token_value = SCAN_P_INTEGER;
			rdp_find("REAL", K_REAL, RDP_ANY).token_value = SCAN_P_REAL;
			rdp_find("EOLN", K_STRING, RDP_ANY).token_value = SCAN_P_EOLN;
		}

		internal static RdpData rdp_process_token(string name)
		{
			rdp_check_token_valid(name);
			RdpData result = rdp_find(name, K_TOKEN, RDP_ANY);
			result.call_count++;

			return result;
		}

		private static void rdp_add_continuations(SymbolScopeData scopeData)
		{
			RdpData temp = (RdpData)scopeData.nextSymbolInScope();
			string last_token = " "; /* remember most recent token name */
			bool tokens_added = false;
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Checking for continuation tokens\n");
			}
			while (temp != null) /* scan over all productions */
			{
				if (temp.kind == K_TOKEN || temp.kind == K_EXTENDED)
				{
					string lo = last_token;
					string hi = text_get_string(temp.id);
					if (!text_is_valid_C_id(hi)) /* ignore identifiers */
					{
						if (hi.StartsWith(lo))
						{
							for (int length = lo.Length + 1; length < hi.Length; length++)
							{
								string continuation_name = hi.Substring(0, length);
								text_insert_string(continuation_name);
								if (rdp_verbose.value())
								{
									text_message(TEXT_INFO, "Adding continuation token \'" + continuation_name + "\'\n");
								}
								tokens_added = true;
								rdp_find(continuation_name, K_TOKEN, RDP_ANY);
							}
						}
					}
					last_token = text_get_string(temp.id);
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
			if (rdp_verbose.value() && !tokens_added)
			{
				text_message(TEXT_INFO, "No continuation tokens needed\n");
			}
		}

		private static void rdp_order_tokens(SymbolScopeData scopeData)
		{
			RdpData temp = (RdpData)scopeData.nextSymbolInScope();

			while (temp != null)
			{
				if (temp.kind == K_TOKEN || temp.kind == K_EXTENDED)
				{
					temp.extended_value = temp.token_value;
					temp.token_value = rdp_token_count++;
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}

			/* now set up start sets for tokens, code and primitives */
			temp = (RdpData)scopeData.nextSymbolInScope();
			while (temp != null)
			{
				if (temp.kind == K_TOKEN || temp.kind == K_INTEGER || temp.kind == K_REAL || temp.kind == K_STRING
						|| temp.kind == K_EXTENDED)
				{
					temp.first.set(temp.token_value);
					temp.first_cardinality = set_cardinality(temp.first);
					temp.follow.set(SCAN_P_EOF);
					temp.follow_cardinality = set_cardinality(temp.follow);
					temp.first_done = 1;
				}
				else if (temp.kind == K_LIST && temp.supplementary_token != null)
				{
					temp.follow.set(temp.supplementary_token.token_value);
					temp.follow_cardinality = set_cardinality(temp.follow);
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
		}
	}
}
