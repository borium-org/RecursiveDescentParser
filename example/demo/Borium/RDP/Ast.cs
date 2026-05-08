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

		internal Ast(Scanner scanner)
		{
			fileName = scanner.rdp_sourcefilename;
			line = scanner.last_line_number;
			column = scanner.last_column;
		}

		internal void Add(Scanner scanner, int token)
		{
			Ast astSymbol = new Ast(scanner)
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
