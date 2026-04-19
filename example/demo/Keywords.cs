namespace Borium.Demo
{
	internal class Keywords
	{
		internal const int RDP_TT_BOTTOM = 16/* SCAN_P_TOP */;

		internal const int RDP_T_46 /* . */ = 16;
		internal const int RDP_T_59 /* ; */ = 17;
		internal const int RDP_T_61 /* = */ = 18;
		internal const int RDP_T_package = 19;

		internal const int RDP_TT_TOP = 20;

		internal static string[] tokenNames = {
			"<EOF>",
			"<Ident",
			"<Error>",
			"<Integer>",
			"<Real>",
			".",
			";",
			"=",
			"package",
		};
	}
}
