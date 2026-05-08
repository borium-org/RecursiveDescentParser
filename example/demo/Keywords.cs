namespace Borium.Demo
{
	internal class Keywords
	{
		internal const int RDP_TT_BOTTOM = 16/* SCAN_P_TOP */;

		internal const int RDP_T_34 /* " */ = 16;
		internal const int RDP_T_39 /* ' */ = 17;
		internal const int RDP_T_44 /* , */ = 18;
		internal const int RDP_T_46 /* . */ = 19;
		internal const int RDP_T_47 /* / */ = 20;
		internal const int RDP_T_4742 /* / * */ = 21;
		internal const int RDP_T_4747 /* // */ = 22;
		internal const int RDP_T_58 /* : */ = 23;
		internal const int RDP_T_59 /* ; */ = 24;
		internal const int RDP_T_61 /* = */ = 25;
		internal const int RDP_T_64 /* @ */ = 26;
		internal const int RDP_T_class = 27;
		internal const int RDP_T_enum = 28;
		internal const int RDP_T_import = 29;
		internal const int RDP_T_interface = 30;
		internal const int RDP_T_internal = 31;
		internal const int RDP_T_package = 32;
		internal const int RDP_T_private = 33;
		internal const int RDP_T_protected = 34;
		internal const int RDP_T_public = 35;
		internal const int RDP_T_static = 36;
		internal const int RDP_T_typedef = 37;
		internal const int RDP_T_123 /* { */ = 38;
		internal const int RDP_T_125 /* } */ = 39;

		internal const int RDP_TT_TOP = 40;

		internal string[] tokenNames = {
			"<Ignore>",
			"<Identifier>",
			"<Integer>",
			"<Real>",
			"<Char>",
			"<Char_Esc>",
			"<String>",
			"<String_Esc>",
			"<Comment>",
			"<CommentVisible>",
			"<CommentNest>",
			"<CommentNestVisible>",
			"<CommentLine>",
			"<CommentLineVisible>",
			"<EOF>",
			"<EOLN>",
			"<String>",
			"<Char>",
			",",
			".",
			"/",
			"/ *",
			"//",
			":",
			";",
			"=",
			"@",
			"class",
			"enum",
			"import",
			"interface",
			"internal",
			"package",
			"private",
			"protected",
			"public",
			"static",
			"typedef",
			"{",
			"}",
		};
	}
}
