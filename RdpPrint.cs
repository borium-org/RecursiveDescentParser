using System;
using System.Collections.Generic;
using static Borium.RDP.CRT;
using static Borium.RDP.RdpAux;
using static Borium.RDP.Scan;
using static Borium.RDP.Text;

namespace Borium.RDP
{
	internal class RdpPrint
	{
		protected static string[] rdp_enum_string;

		internal static string[] rdp_token_string;

		internal static void rdp_make_token_string(SymbolScopeData scopeData)
		{
			RdpData p = (RdpData)scopeData.nextSymbolInScope();

			List<string> tokens = new List<string>();
			tokens.Add(text_get_string(text_insert_string("IGNORE")));
			tokens.Add(text_get_string(text_insert_string("ID")));
			tokens.Add(text_get_string(text_insert_string("INTEGER")));
			tokens.Add(text_get_string(text_insert_string("REAL")));
			tokens.Add(text_get_string(text_insert_string("CHAR")));
			tokens.Add(text_get_string(text_insert_string("CHAR_ESC")));
			tokens.Add(text_get_string(text_insert_string("STRING")));
			tokens.Add(text_get_string(text_insert_string("STRING_ESC")));
			tokens.Add(text_get_string(text_insert_string("COMMENT")));
			tokens.Add(text_get_string(text_insert_string("COMMENT_VISIBLE")));
			tokens.Add(text_get_string(text_insert_string("COMMENT_NEST")));
			tokens.Add(text_get_string(text_insert_string("COMMENT_NEST_VISIBLE")));
			tokens.Add(text_get_string(text_insert_string("COMMENT_LINE")));
			tokens.Add(text_get_string(text_insert_string("COMMENT_LINE_VISIBLE")));
			tokens.Add(text_get_string(text_insert_string("EOF")));
			tokens.Add(text_get_string(text_insert_string("EOLN")));

			while (p != null)
			{
				if (p.kind == K_TOKEN || p.kind == K_EXTENDED)
				{
					p.token_string = text_insert_char('\''); /* insert open quote */
					String str = text_get_string(p.id);
					foreach (char c in str)
					{
						if (c == '\"' || c == '\\' || c == '\'')
						{
							text_insert_char('\\');
						}
						text_insert_char(c);
					}
					text_insert_string("\'"); /* insert close quote */
					tokens.Add(text_get_string(p.token_string));
				}
				p = (RdpData)p.nextSymbolInScope();
			}

			p = (RdpData)scopeData.nextSymbolInScope();

			List<string> enums = new List<string>();
			int p_ignore = text_insert_string("SCAN_P_IGNORE");
			enums.Add(text_get_string(p_ignore));
			enums.Add(text_get_string(text_insert_string("SCAN_P_ID")));
			enums.Add(text_get_string(text_insert_string("SCAN_P_INTEGER")));
			enums.Add(text_get_string(text_insert_string("SCAN_P_REAL")));
			int p_char = text_insert_string("SCAN_P_CHAR");
			enums.Add(text_get_string(p_char));
			int p_char_esc = text_insert_string("SCAN_P_CHAR_ESC");
			enums.Add(text_get_string(p_char_esc));
			int p_string = text_insert_string("SCAN_P_STRING");
			enums.Add(text_get_string(p_string));
			int p_string_esc = text_insert_string("SCAN_P_STRING_ESC");
			enums.Add(text_get_string(p_string_esc));
			int p_comment = text_insert_string("SCAN_P_COMMENT");
			enums.Add(text_get_string(p_comment));
			int p_comment_visible = text_insert_string("SCAN_P_COMMENT_VISIBLE");
			enums.Add(text_get_string(p_comment_visible));
			int p_comment_nest = text_insert_string("SCAN_P_COMMENT_NEST");
			enums.Add(text_get_string(p_comment_nest));
			int p_comment_nest_visible = text_insert_string("SCAN_P_COMMENT_NEST_VISIBLE");
			enums.Add(text_get_string(p_comment_nest_visible));
			int p_comment_line = text_insert_string("SCAN_P_COMMENT_LINE");
			enums.Add(text_get_string(p_comment_line));
			int p_comment_line_visible = text_insert_string("SCAN_P_COMMENT_LINE_VISIBLE");
			enums.Add(text_get_string(p_comment_line_visible));
			enums.Add(text_get_string(text_insert_string("SCAN_P_EOF")));
			enums.Add(text_get_string(text_insert_string("SCAN_P_EOLN")));

			while (p != null)
			{
				if (p.kind == K_TOKEN || p.kind == K_EXTENDED)
				{
					p.token_enum = text_insert_characters("RDP_T_");

					if (text_is_valid_C_id(text_get_string(p.id)))
					{
						text_insert_string(text_get_string(p.id));
					}
					else
					{
						string str = text_get_string(p.id);
						foreach (char c in str)
						{
							text_insert_integer(c);
						}
						text_insert_characters(" /* ");
						if (strcmp(str, "/*") == 0) // special case: put a /* in the comment
						{
							text_insert_char('/');
							text_insert_char(' ');
							text_insert_char('*');
						}
						else if (strcmp(str, "*/") == 0) // special case: put a */ in the comment
						{
							text_insert_char('*');
							text_insert_char(' ');
							text_insert_char('/');
						}
						else
						{
							foreach (char c in str)
							{
								text_insert_char(c);
							}
						}

						text_insert_string(" */");
					}
					enums.Add(text_get_string(p.token_enum));
					if (p.kind == K_EXTENDED)
					{
						switch (p.extended_value)
						{
							case SCAN_P_CHAR:
								p.extended_enum = text_get_string(p_char);
								break;
							case SCAN_P_CHAR_ESC:
								p.extended_enum = text_get_string(p_char_esc);
								break;
							case SCAN_P_STRING:
								p.extended_enum = text_get_string(p_string);
								break;
							case SCAN_P_STRING_ESC:
								p.extended_enum = text_get_string(p_string_esc);
								break;
							case SCAN_P_COMMENT:
								p.extended_enum = text_get_string(p_comment);
								break;
							case SCAN_P_COMMENT_VISIBLE:
								p.extended_enum = text_get_string(p_comment_visible);
								break;
							case SCAN_P_COMMENT_NEST:
								p.extended_enum = text_get_string(p_comment_nest);
								break;
							case SCAN_P_COMMENT_NEST_VISIBLE:
								p.extended_enum = text_get_string(p_comment_nest_visible);
								break;
							case SCAN_P_COMMENT_LINE:
								p.extended_enum = text_get_string(p_comment_line);
								break;
							case SCAN_P_COMMENT_LINE_VISIBLE:
								p.extended_enum = text_get_string(p_comment_line_visible);
								break;
						}
					}
					else
					{
						p.extended_enum = text_get_string(p_ignore);
					}
				}
				p = (RdpData)p.nextSymbolInScope();
			}
			rdp_token_string = tokens.ToArray();
			rdp_enum_string = enums.ToArray();
		}

		protected int rdp_indentation;

		protected int indent()
		{
			for (int temp = 0; temp < rdp_indentation; temp++)
			{
				text_printf("  ");
			}
			return rdp_indentation * 2;
		}

		protected int indent(int extraIndent)
		{
			for (int temp = 0; temp < rdp_indentation + extraIndent; temp++)
			{
				text_printf("  ");
			}
			return rdp_indentation * 2;
		}

		protected int iprint(String text)
		{
			return text_iprintf(text);
		}

		protected int iprintln()
		{
			return text_printf("\n");
		}

		protected int iprintln(int extraIndent, String text)
		{
			return text_iprintf(extraIndent, text + "\n");
		}

		protected int iprintln(String text)
		{
			return text_iprintf(text + "\n");
		}

		protected int print(String fmt)
		{
			return text_printf(fmt);
		}

		protected int println()
		{
			return text_printf("\n");
		}

		protected int println(String text)
		{
			return text_printf(text + "\n");
		}

		protected void rdp_print_parser_production_name(RdpData n)
		{
			rdp_print_parser_production_name(n, true);
		}

		protected void rdp_print_parser_production_name_no_comment(RdpData n)
		{
			rdp_print_parser_production_name(n, false);
		}

		protected void rdp_print_parser_string(string str)
		{
			foreach (char ch in str)
			{
				if (ch == '\"' || ch == '\\' || ch == '\'')
				{
					text_printf("\\");
				}
				text_printf("" + ch);
			}
		}

		private void rdp_print_parser_production_name(RdpData n, bool printComment)
		{
			switch (n.kind)
			{
				case K_CODE:
					text_printf("[*" + text_get_string(n.id) + "*]");
					break;
				case K_EXTENDED:
				case K_TOKEN:
					{
						String tokenName = text_get_string(n.token_enum);
						if (!printComment)
						{
							int pos = tokenName.IndexOf(' ');
							if (pos != -1)
								tokenName = tokenName.Substring(0, pos);
						}
						text_printf(tokenName);
					}
					break;
				case K_INTEGER:
				case K_REAL:
				case K_STRING:
					text_printf("SCAN_P_" + text_get_string(n.id));
					break;
				default:
					text_printf(text_get_string(n.id));
					if (text_get_string(n.id).Length == 0)
					{
						Console.WriteLine("Empty string");
					}
					break;
			}
		}

		private int text_iprintf(int extraIndent, String fmt)
		{
			int i = 0;
			// In some cases we just iprintf("\n") and it does not need to be
			// indented
			if (!fmt.Equals("\n"))
			{
				i = indent(extraIndent);
			}
			i += text_printf(fmt);
			return i; /* return number of characters printed */
		}

		private int text_iprintf(String fmt)
		{
			int i = 0;
			// In some cases we just iprintf("\n") and it does not need to be
			// indented
			if (!fmt.Equals("\n"))
			{
				i = indent();
			}
			i += text_printf(fmt);
			return i; /* return number of characters printed */
		}
	}
}
