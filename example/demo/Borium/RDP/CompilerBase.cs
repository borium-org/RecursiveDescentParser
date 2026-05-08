using System.IO;
using static Borium.RDP.Text;

namespace Borium.RDP
{
	public class CompilerBase
	{
		internal const int SCAN_P_IGNORE = Scanner.SCAN_P_IGNORE;
		internal const int SCAN_P_ID = Scanner.SCAN_P_ID;
		internal const int SCAN_P_INTEGER = Scanner.SCAN_P_INTEGER;
		internal const int SCAN_P_REAL = Scanner.SCAN_P_REAL;
		internal const int SCAN_P_CHAR = Scanner.SCAN_P_CHAR;
		internal const int SCAN_P_CHAR_ESC = Scanner.SCAN_P_CHAR_ESC;
		internal const int SCAN_P_STRING = Scanner.SCAN_P_STRING;
		internal const int SCAN_P_STRING_ESC = Scanner.SCAN_P_STRING_ESC;
		internal const int SCAN_P_COMMENT = Scanner.SCAN_P_COMMENT;
		internal const int SCAN_P_COMMENT_VISIBLE = Scanner.SCAN_P_COMMENT_VISIBLE;
		internal const int SCAN_P_COMMENT_NEST = Scanner.SCAN_P_COMMENT_NEST;
		internal const int SCAN_P_COMMENT_NEST_VISIBLE = Scanner.SCAN_P_COMMENT_NEST_VISIBLE;
		internal const int SCAN_P_COMMENT_LINE = Scanner.SCAN_P_COMMENT_LINE;
		internal const int SCAN_P_COMMENT_LINE_VISIBLE = Scanner.SCAN_P_COMMENT_LINE_VISIBLE;
		internal const int SCAN_P_EOF = Scanner.SCAN_P_EOF;
		internal const int SCAN_P_EOLN = Scanner.SCAN_P_EOLN;
		internal const int SCAN_P_TOP = Scanner.SCAN_P_TOP;

		private readonly Text text;

		private readonly Scanner scanner;

		protected CompilerBase()
		{
			text = new Text();
			scanner = new Scanner(text);
		}

		public string SourceFileName
		{
			get { return scanner.rdp_sourcefilename; }
			set { scanner.rdp_sourcefilename = value; }
		}

		protected void ScannerInit(bool case_insensitive, bool newline_visible, bool show_skips,
				bool symbol_echo, string[] token_names)
		{
			scanner.scan_init(case_insensitive, newline_visible, show_skips, symbol_echo, token_names);
		}

		protected void Scan()
		{
			scanner.scan_();
		}

		protected bool ScanTest(string production, int valid, Set stop)
		{
			return scanner.scan_test(production, valid, stop);
		}

		protected bool ScanTestSet(string production, Set valid, Set stop)
		{
			return scanner.scan_test_set(production, valid, stop);
		}

		protected void ScanLoadKeyword(string id1, string id2, int token, int extended)
		{
			scanner.scan_load_keyword(id1, id2, token, extended);
		}

		protected void TextInit(int max_text, int max_errors, int max_warnings, int tab_width)
		{
			text.text_init(max_text, max_errors, max_warnings, tab_width);
		}

		protected void TextRedirect(TextWriter file)
		{
			text.text_redirect(file);
		}

		protected TextReader TextOpen(string fileName, TextReader textReader)
		{
			return text.text_open(fileName, textReader);
		}

		protected void TextGetChar()
		{
			text.text_get_char();
		}

		protected int TextTotalErrors()
		{
			return text.text_total_errors();
		}

		protected int TextMessage(TextMessageType type, string message)
		{
			return text.text_message(type, message);
		}
	}
}
