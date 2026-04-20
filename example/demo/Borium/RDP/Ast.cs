using System.Collections.Generic;

namespace Borium.RDP
{
	public class Ast
	{
		internal string fileName;
		internal int line;
		internal int column;
		internal int token;

		private readonly List<Ast> astList = new List<Ast>();

		internal Ast()
		{
			fileName = Scanner.rdp_sourcefilename;
			line = Scanner.last_line_number;
			column = Scanner.last_column;
		}

		internal void Add(int token)
		{
			Ast astSymbol = new Ast
			{
				token = token
			};
			astList.Add(astSymbol);
		}

		internal void Add(Ast ast)
		{
			astList.Add(ast);
		}
	}
}
