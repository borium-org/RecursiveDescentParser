using System;
using System.Globalization;
using static Borium.RDP.CRT;
using static Borium.RDP.RdpAux;
using static Borium.RDP.RdpPrint;
using static Borium.RDP.RdpProgram;
using static Borium.RDP.Scan;
using static Borium.RDP.Set;
using static Borium.RDP.Symbol;
using static Borium.RDP.Text;
using static Borium.RDP.Text.TextMessageType;

namespace Borium.RDP
{
	internal class RdpGram
	{
#if false
		private static final string[] RDP_RESERVED_WORDS = { "auto", "break", "case", "char", "const", "continue",
			"default", "do", "double", "else", "enum", "extern", "float", "for", "goto", "if", "int", "long",
			"register", "return", "short", "signed", "sizeof", "static", "struct", "switch", "typedef", "union",
			"unsigned", "void", "volatile", "while", "operator", "printf" };

	private static int rdp_follow_changed;

		internal static int rdp_bad_grammar(SymbolScopeData base)
		{
			int bad = 0;

			/* Check for empties */
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Checking for empty alternates\n");
			}
			bad |= rdp_check_empties(base);

			/* Count productions and produce statistics */
			rdp_count_productions(base);

			/* Check promotion operators on start production */
			if (rdp_start_prod.promote_default != PROMOTE_DONT)
			{
				text_message(TEXT_WARNING,
						"default promotion operator \'"
								+ (rdp_start_prod.promote_default == PROMOTE ? "^"
										: rdp_start_prod.promote_default == PROMOTE_AND_COPY ? "^^" : "??")
								+ "\' on start production \'" + text_get_string(rdp_start_prod.id)
								+ "\' will not be applied at top level\n");
			}

			/* find first sets */
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Generating first sets\n");
			}
			rdp_find_first(base);

			/* find follow sets */
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Generating follow sets\n");
			}
			rdp_find_follow(base);

			/* check for C reserved words */
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Checking for clashes with reserved words\n");
			}
			bad |= rdp_check_reserved_words();

			/* check that for each production, all alternates have unique start tokens */
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Checking for disjoint first sets\n");
			}
			bad |= rdp_check_disjoint(base);

			/* check nullable brackets don't contain nullable productions */
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Checking for nested nullable subrules\n");
			}
			bad |= rdp_check_nested_nullable(base);

			/* check that first(a) - follow (a) is empty for nullable a */
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Checking nullable rules\n");
			}
			bad |= rdp_check_nullable(base);

			/* add first() to follow() for iterations so that error handling doesn't just eat entire file! */
			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, "Updating follow sets\n");
			}
			rdp_update_follow_sets(base);
			/* re-close follow sets */
			rdp_find_follow(base);

			return bad;
		}

		internal static void rdp_check_eoln(string id)
		{
			if (id.equals("EOLN"))
			{
				rdp_dir_newline_visible = 1; /* Grammar contains an explicit EOLN */
			}
		}
#endif

		internal static void rdp_check_token_valid(string id)
		{
			if (id == null)
				return;

			if (id.Length == 0)
				text_message(TEXT_ERROR_ECHO, "empty tokens are not allowed: use [ ... ] instead\n");
			// Test for embedded spaces in token

			bool bad = false;

			foreach (char ch in id)
			{
				bad |= !isgraph(ch);
			}

			if (bad)
				text_message(TEXT_ERROR_ECHO, "tokens must not contain spaces or control characters\n");
		}

#if false
		private static int rdp_check_disjoint(SymbolScopeData base)
		{
			int bad = 0;
			Set work = new Set();

			RdpData temp = (RdpData)base.nextSymbolInScope();

			while (temp != null)
			{
				if (rdp_production_set.includes(temp.kind) && temp.kind != K_SEQUENCE)
				{
					RdpList left = temp.list;
					while (left != null)
					{
						RdpList right = left.next;
						while (right != null)
						{
							/* First check for disjoint on epsilon */
							if (left.production.contains_null && right.production.contains_null)
							{
								text_message(TEXT_ERROR, "LL(1) violation - rule \'" + text_get_string(temp.id) + "\'\n");
								text_printf(" productions " + text_get_string(left.production.id) + " ::= ");
								left.production.rdp_print_sub_item(true);
								text_printf(".\n and " + text_get_string(right.production.id) + " ::= ");
								right.production.rdp_print_sub_item(true);
								text_printf(".\n are both nullable \n");
								left.production.ll1_violation = 1;
								right.production.ll1_violation = 1;
								bad = 1;
							}
							work.assignSet(left.production.first);
							work.intersect(right.production.first);

							if (set_cardinality(work) != 0)
							{
								text_message(TEXT_ERROR, "LL(1) violation - rule \'" + text_get_string(temp.id) + "\'\n");
								text_printf(" productions " + text_get_string(left.production.id) + " ::= ");
								left.production.rdp_print_sub_item(true);
								text_printf(".\n and " + text_get_string(right.production.id) + " ::= ");
								right.production.rdp_print_sub_item(true);
								text_printf(".\n share these start tokens: ");
								work.print(rdp_token_string, 78);
								text_printf("\n");
								left.production.ll1_violation = 1;
								right.production.ll1_violation = 1;
								bad = 1;
							}
							right = right.next;
						}
						left = left.next;
					}
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
			return bad;
		}

		private static int rdp_check_empties(SymbolScopeData base)
		{
			int bad = 0;
			RdpData temp = (RdpData)base.nextSymbolInScope();
			while (temp != null)
			{
				RdpList list = temp.list;
				int k = temp.kind, bad_alternate = 1;

				if (k == K_PRIMARY && temp.call_count == 0 && !temp.comment_only)
				{
					text_message(TEXT_WARNING, "rule \'" + text_get_string(temp.id) + "\' never called so deleted\n");
				}

				if (list == null && k == K_PRIMARY)
				{
					text_message(TEXT_ERROR, "rule \'" + text_get_string(temp.id) + "\' is empty\n");
					bad = 1;
				}

				if (k == K_SEQUENCE) /* check for empty alternates and mark up code */
				{
					while (list != null)
					{
						if (list.production.kind == K_CODE)
						{
							if (list.next == null)
							{
								list.production.code_terminator = 1;
							}
							else if (list.next.production.kind == K_CODE)
							{
								list.next.production.code_successor = 1; /* next one is code successor */
							}
							else
							{
								list.production.code_terminator = 1; /* this one is code terminator */
							}
						}

						if (list.production.kind != K_CODE)
						{
							bad_alternate = 0;
						}
						list = list.next;
					}
				}
				else
				{
					bad_alternate = 0;
				}

				if (bad_alternate != 0)
				{
					if (temp.list == null)
					{
						text_message(TEXT_ERROR,
								"LL(1) violation - alternate \'" + text_get_string(temp.id) + "\' is empty\n");
						temp.ll1_violation = 1;
					}
					else
					{
						temp.code_only = 1;
					}
					bad = 1;
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}

			/* Now go over again updating primaries to mark code only productions */
			temp = (RdpData)base.nextSymbolInScope();

			while (temp != null)
			{
				if (temp.kind == K_PRIMARY && temp.list != null)
				{
					if (temp.list.next == null && temp.list.production.code_only != 0)
					{
						temp.code_only = 1;
					}
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
			return bad;
		}

		private static int rdp_check_identifier(string id)
		{
			RdpData s = (RdpData)symbol_lookup_key(rdp, id, null);

			if (s != null)
			{
				if (s.kind == K_PRIMARY)
				{
					text_message(TEXT_ERROR, "identifier \'" + id + "\' is a C++ reserved word or library identifier\n");
					return 1;
				}
			}
			return 0;
		}

		private static int rdp_check_nested_nullable(SymbolScopeData base)
		{
			int bad = 0;
			RdpData temp = (RdpData)base.nextSymbolInScope();

			while (temp != null)
			{
				if (rdp_production_set.includes(temp.kind) && temp.kind != K_SEQUENCE)
				{
					RdpList inner = temp.list;
					while (inner != null)
					{
						if (temp.lo == 0 && inner.production.contains_null)
						{
							text_message(TEXT_ERROR, "LL(1) violation - rule \'" + text_get_string(temp.id)
									+ "\'\n is nullable but contains the nullable subrule\n");
							text_printf(" " + text_get_string(inner.production.id) + " ::= ");
							inner.production.rdp_print_sub_item(true);
							text_printf(".\n");
							bad = 1;
							temp.ll1_violation = 1;
							inner.production.ll1_violation = 1;
						}
						inner = inner.next;
					}
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
			return bad;
		}

		private static int rdp_check_nullable(SymbolScopeData base)
		{
			int bad = 0;
			Set work = new Set();
			RdpData temp = (RdpData)base.nextSymbolInScope();
			while (temp != null)
			{
				if (temp.contains_null && temp.kind != K_CODE)
				{
					work.assignSet(temp.first);
					work.intersect(temp.follow);
					if (set_cardinality(work) != 0)
					{
						text_message(TEXT_ERROR, "LL(1) violation - rule\n " + text_get_string(temp.id) + " ::= ");
						temp.rdp_print_sub_item(true);
						text_printf(".\n contains null but first and follow sets both include: ");
						work.print(rdp_token_string, 78);
						text_printf("\n");
						temp.ll1_violation = 1;
						bad = 1;
					}
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
			return bad;
		}

		private static int rdp_check_reserved_words()
		{
			int bad = 0;
			for (string reserved : RDP_RESERVED_WORDS)
			{
				bad |= rdp_check_identifier(reserved);
			}
			return bad;
		}

		private static void rdp_count_productions(SymbolScopeData base)
		{
			int primaries = 0, internals = 0, codes = 0;

			RdpData temp = (RdpData)base.nextSymbolInScope();

			while (temp != null)
			{
				if (temp.kind == K_PRIMARY)
				{
					primaries++;
				}
				else if (temp.kind == K_CODE)
				{
					codes++;
				}
				else
				{
					internals++;
				}

				temp = (RdpData)temp.nextSymbolInScope();
			}

			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO, primaries + " rules, " + (rdp_token_count - SCAN_P_TOP + 1) + " tokens, " + codes
						+ " actions, " + internals + " subrules\n");
			}
		}

		private static void rdp_find_first(SymbolScopeData base)
		{
			RdpData temp = (RdpData)base.nextSymbolInScope();

			while (temp != null)
			{
				rdp_first(temp);
				temp = (RdpData)temp.nextSymbolInScope();
			}
		}

		private static void rdp_find_follow(SymbolScopeData base)
		{
			RdpData temp;
			int follow_pass = 0;

			do
			{
				follow_pass++;
				rdp_follow_changed = 0;
				temp = (RdpData)base.nextSymbolInScope();
				while (temp != null)
				{
					if (temp.kind == K_SEQUENCE)
					{
						rdp_follow_sequence(temp);
					}
					else
					{
						rdp_follow_alternate(temp);
					}
					temp = (RdpData)temp.nextSymbolInScope();
				}
			} while (rdp_follow_changed != 0);

			if (rdp_verbose.value())
			{
				text_message(TEXT_INFO,
						"Follow sets stabilised after " + follow_pass + " pass" + (follow_pass == 1 ? "" : "es") + "\n");
			}
		}

		private static void rdp_first(RdpData prod)
		{
			if (prod.in_use != 0) /* something has gone wrong */
			{
				text_message(TEXT_ERROR, "LL(1) violation - rule \'" + text_get_string(prod.id) + "\' is left recursive\n");
				prod.ll1_violation = 1;
				return;
			}

			if (prod.first_done == 0) /* something to do */
			{
				RdpList list = prod.list; /* set up alternates pointer */

				prod.in_use = 1; /* mark this production as being processed */

				if (prod.kind == K_SEQUENCE) /* sequences are treated differently */
				{
					prod.contains_null = true; /* set up list flag */
					// scan until non-empty alternate is found
					while (list != null && prod.contains_null)
					{
						if (list.production.first_done == 0)
						{
							rdp_first(list.production);
						}
						// add alternate first set to production first set
						prod.first.unite(list.production.first);
						// set contains_null flag
						prod.contains_null = list.production.contains_null;
						list = list.next;
					}
				}
				else
				{
					while (list != null) /* scan all alternates */
					{
						if (list.production.first_done == 0)
						{
							rdp_first(list.production);
						}
						// add alternate first set to production first set
						prod.first.unite(list.production.first);
						// OR in contains_null flag
						prod.contains_null |= list.production.contains_null;
						list = list.next;
					}
				}
				prod.in_use = 0; /* production is no longer in use */
				prod.first_done = 1; /* first set is now complete */
				/* and set cardinality */
				prod.first_cardinality = set_cardinality(prod.first);
			}
		}

		private static void rdp_follow_alternate(RdpData prod)
		{
			RdpList check = prod.list; /* pointer to alternate list */

			while (check != null)
			{
				int old_cardinality = check.production.follow_cardinality;

				check.production.follow.unite(prod.follow);

				rdp_follow_changed |= (check.production.follow_cardinality = set_cardinality(
						check.production.follow)) != old_cardinality ? 1 : 0;
				check = check.next;
			}
		}

		private static void rdp_follow_sequence(RdpData prod)
		{
			RdpList check = prod.list; /* pointer to sequence list */

			while (check != null) /* scan entire sequence and add to follow sets */
			{
				RdpList following = check; /* temporary to look at followers */
				int old_cardinality = check.production.follow_cardinality;

				do /* scan up list adding first sets of trailing productions */
				{
					following = following.next;
					if (following == null)
					{
						check.production.follow.unite(prod.follow);
					}
					else
					{
						check.production.follow.unite(following.production.first);
					}
				} while (following != null && following.production.contains_null);

				/* Update cardinality changed flag */
				rdp_follow_changed |= (check.production.follow_cardinality = set_cardinality(
						check.production.follow)) != old_cardinality ? 1 : 0;

				check = check.next; /* step to next item in sequence */
			}
		}

		private static void rdp_update_follow_sets(SymbolScopeData base)
		{
			RdpData temp = (RdpData)base.nextSymbolInScope();
			while (temp != null)
			{
				if (temp.kind == K_LIST && temp.hi != 1 && temp.supplementary_token == null)
				{
					temp.follow.unite(temp.first);
					temp.follow_cardinality = set_cardinality(temp.follow);
				}
				temp = (RdpData)temp.nextSymbolInScope();
			}
		}
#endif
	}
}
