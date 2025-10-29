using System;
using System.Collections.Generic;

using static Borium.RDP.Arg.ArgKind;

namespace Borium.RDP
{
	internal class Arg
	{
		internal enum ArgKind
		{
			ARG_BLANK, ARG_BOOLEAN, ARG_NUMERIC, ARG_STRING
		}

		private class ArgData
		{
			internal ArgKind kind;
			internal char key;
			internal string description;
			/** Booleans were in RDP 1.5 stored in an integer field, so there's the source for this mismatch. */
			internal bool[] boolvalue;
			/** All truly integer values are unsigned, no negative integers were used in making this app. */
			internal int[] unsignedvalue;
			internal string[] str;
			internal ArgData next;
		}

		const int EXIT_FAILURE = 1;

		private static ArgData argsBase;

		internal static void arg_boolean(char key, string description, bool[] intvalue)
		{
			AddNode(ARG_BOOLEAN, key, description, intvalue, null, null);
		}

		internal static void arg_help(string msg)
		{
			Console.Write("\n\nFatal - " + (msg ?? "") + "\n\n");
			Print(argsBase);
			Environment.Exit(EXIT_FAILURE);
		}

		internal static void arg_message(string description)
		{
			AddNode(ARG_BLANK, '\0', description, null, null, null);
		}

		internal static void arg_numeric(char key, string description, int[] unsignedvalue)
		{
			AddNode(ARG_NUMERIC, key, description, null, unsignedvalue, null);
		}

		internal static string[] arg_process(string[] argStrings)
		{
			List<string> ret = new List<string>();
			foreach (string arg in argStrings)
			{
				if (arg[0] == '-')
				{
					if (arg.Length < 2)
					{
						arg_help("bad command line argument");
					}
					ArgData temp = argsBase;
					while (temp.next != null && temp.key != arg[1])
					{
						temp = temp.next;
					}
					if (temp.key != arg[1])
					{
						arg_help("unknown command line argument");
					}
					switch (temp.kind)
					{
						case ARG_BOOLEAN:
							temp.boolvalue[0] = !temp.boolvalue[0];
							break;

						case ARG_NUMERIC:
							temp.unsignedvalue[0] = Convert.ToInt32(arg.Substring(2));
							break;

						case ARG_STRING:
							temp.str[0] = arg.Substring(2);
							break;

						default:
							break;
					}
				}
				else
				{
					ret.Add(arg);
				}
			}
			return ret.ToArray();
		}

		internal static void arg_string(char key, string description, string[] str)
		{
			AddNode(ARG_STRING, key, description, null, null, str);
		}

		private static void AddNode(ArgKind kind, char key, string description, bool[] boolvalue,
				int[] unsignedvalue, string[] str)
		{
			ArgData temp = new ArgData
			{
				kind = kind,
				key = key,
				description = description,
				boolvalue = boolvalue,
				unsignedvalue = unsignedvalue,
				str = str,
				next = argsBase
			};
			argsBase = temp;
		}

		private static void Print(ArgData p)
		{
			if (p != null)
			{
				Print(p.next);
				if (p.kind != ARG_BLANK)
				{
					Console.Write("-" + p.key + (p.kind == ARG_NUMERIC ? "<n>" : p.kind == ARG_STRING ? "<s>" : "   ") + " ");
				}
				Console.WriteLine(p.description);
			}
		}
	}
}
