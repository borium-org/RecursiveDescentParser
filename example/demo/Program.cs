using Borium.RDP;

namespace Borium.Demo
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Compiler compiler = new Compiler("demo.icp");
			Ast ast = compiler.RuleCompilationUnit();
		}
	}
}
