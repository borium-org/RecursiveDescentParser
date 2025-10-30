using System;
using System.Collections.Generic;

//using static Borium.RDP.Arg.ArgKind;

namespace Borium.RDP
{
	internal class Arg
	{
#if false
		protected enum ArgKind
		{
			ARG_BLANK, ARG_BOOLEAN, ARG_NUMERIC, ARG_STRING
		}

		private static class arg_data
		{
			ArgKind kind;
			char key;
			String description;
			/** Booleans were in RDP 1.5 stored in an integer field, so there's the source for this mismatch. */
			Pointer<Boolean> intvalue;
			/** All truly integer values are unsigned, no negative integers were used in making this app. */
			Pointer<Integer> unsignedvalue;
			Pointer<String> str;
			arg_data next;
		}

		private static final int EXIT_FAILURE = 1;

		private static arg_data base = null;

	public static void arg_boolean(char key, String description, Pointer<Boolean> intvalue)
		{
			add_node(ARG_BOOLEAN, key, description, intvalue, null, null);
		}

		public static void arg_help(String msg)
		{
			System.out.print("\n\nFatal - " + (msg == null ? "" : msg) + "\n\n");
			print(base);
			System.exit(EXIT_FAILURE);
		}

		public static void arg_message(String description)
		{
			add_node(ARG_BLANK, '\0', description, null, null, null);
		}

		public static void arg_numeric(char key, String description, Pointer<Integer> unsignedvalue)
		{
			add_node(ARG_NUMERIC, key, description, null, unsignedvalue, null);
		}

		public static String[] arg_process(String[] args)
		{
			ArrayList<String> ret = new ArrayList<>();
			for (String arg : args)
			{
				if (arg.charAt(0) == '-')
				{
					if (arg.length() < 2)
					{
						arg_help("bad command line argument");
					}
					arg_data temp = base;
					while (temp.next != null && temp.key != arg.charAt(1))
					{
						temp = temp.next;
					}
					if (temp.key != arg.charAt(1))
					{
						arg_help("unknown command line argument");
					}
					switch (temp.kind)
					{
						case ARG_BOOLEAN:
							temp.intvalue.set(!temp.intvalue.value());
							break;
						case ARG_NUMERIC:
							temp.unsignedvalue.set(Integer.parseInt(arg.substring(2)));
							break;
						case ARG_STRING:
							temp.str.set(arg.substring(2));
							break;
						default:
							break;
					}
				}
				else
				{
					ret.add(arg);
				}
			}
			return ret.toArray(new String[ret.size()]);
		}

		public static void arg_string(char key, String description, Pointer<String> str)
		{
			add_node(ARG_STRING, key, description, null, null, str);
		}

		private static void add_node(ArgKind kind, char key, String description, Pointer<Boolean> intvalue,
				Pointer<Integer> unsignedvalue, Pointer<String> str)
		{
			arg_data temp = new arg_data();
			temp.kind = kind;
			temp.key = key;
			temp.description = description;
			temp.intvalue = intvalue;
			temp.unsignedvalue = unsignedvalue;
			temp.str = str;
			temp.next = base;
			base = temp;
		}

		private static void print(arg_data p)
		{
			if (p != null)
			{
				print(p.next);
				if (p.kind != ARG_BLANK)
				{
					System.out.print(
							"-" + p.key + (p.kind == ARG_NUMERIC ? "<n>" : p.kind == ARG_STRING ? "<s>" : "   ") + " ");
				}
				System.out.println(p.description);
			}
		}
#endif
	}
}
