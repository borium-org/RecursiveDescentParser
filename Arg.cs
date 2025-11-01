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

		private class arg_data
		{
			internal ArgKind kind;
			internal char key;
			internal string description;
			/** Booleans were in RDP 1.5 stored in an integer field, so there's the source for this mismatch. */
			internal Pointer<bool> intvalue;
			/** All truly integer values are unsigned, no negative integers were used in making this app. */
			internal Pointer<int> unsignedvalue;
			internal Pointer<string> str;
			internal arg_data next;
		}

		private const int EXIT_FAILURE = 1;

		private static arg_data argBase = null;

		internal static void arg_boolean(char key, string description, Pointer<bool> intvalue)
		{
			add_node(ARG_BOOLEAN, key, description, intvalue, null, null);
		}

		internal static void arg_help(string msg)
		{
			Console.Write("\n\nFatal - " + (msg == null ? "" : msg) + "\n\n");
			print(argBase);
			Environment.Exit(EXIT_FAILURE);
		}

		internal static void arg_message(string description)
		{
			add_node(ARG_BLANK, '\0', description, null, null, null);
		}

		internal static void arg_numeric(char key, string description, Pointer<int> unsignedvalue)
		{
			add_node(ARG_NUMERIC, key, description, null, unsignedvalue, null);
		}

		internal static string[] arg_process(string[] args)
		{
			List<string> ret = new List<string>();
			foreach (string arg in args)
			{
				if (arg[0] == '-')
				{
					if (arg.Length < 2)
					{
						arg_help("bad command line argument");
					}
					arg_data temp = argBase;
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
							temp.intvalue.set(!temp.intvalue.value());
							break;
						case ARG_NUMERIC:
							temp.unsignedvalue.set(Convert.ToInt32(arg.Substring(2)));
							break;
						case ARG_STRING:
							temp.str.set(arg.Substring(2));
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

		internal static void arg_string(char key, string description, Pointer<string> str)
		{
			add_node(ARG_STRING, key, description, null, null, str);
		}

		private static void add_node(ArgKind kind, char key, string description, Pointer<bool> intvalue,
				Pointer<int> unsignedvalue, Pointer<string> str)
		{
			arg_data temp = new arg_data();
			temp.kind = kind;
			temp.key = key;
			temp.description = description;
			temp.intvalue = intvalue;
			temp.unsignedvalue = unsignedvalue;
			temp.str = str;
			temp.next = argBase;
			argBase = temp;
		}

		private static void print(arg_data p)
		{
			if (p != null)
			{
				print(p.next);
				if (p.kind != ARG_BLANK)
				{
					Console.Write("-" + p.key + (p.kind == ARG_NUMERIC ? "<n>" : p.kind == ARG_STRING ? "<s>" : "   ") + " ");
				}
				Console.WriteLine(p.description);
			}
		}
	}
}
