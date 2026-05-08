using System;
using System.IO;
using static Borium.RDP.CRT;
using static Borium.RDP.Scanner;
using static Borium.RDP.Text.TextMessageType;

namespace Borium.RDP
{
	public class Text
	{
		public enum TextMessageType
		{
			TEXT_INFO, TEXT_WARNING, TEXT_ERROR, TEXT_FATAL, TEXT_INFO_ECHO, TEXT_WARNING_ECHO, TEXT_ERROR_ECHO,
			TEXT_FATAL_ECHO
		}

		private class SourceList
		{
			/// <summary>
			/// Copy of filename
			/// </summary>
			internal string name;

			/// <summary>
			/// Copy of total errors for this file
			/// </summary>
			internal int errors;

			/// <summary>
			/// Copy of current file handle
			/// </summary>
			internal TextReader file;

			/// <summary>
			/// Copy of first character of current source line
			/// </summary>
			internal int first_char;

			/// <summary>
			/// Copy of last character of current source line
			/// </summary>
			internal int last_char;

			/// <summary>
			/// Copy of current line in this file
			/// </summary>
			internal int linenumber;

			/// <summary>
			/// Copy of current text character
			/// </summary>
			internal int text_char;

			/// <summary>
			/// Copy of pointer to current source character
			/// </summary>
			internal int text_current;

			/// <summary>
			/// Copy of pointer to the last thing read by the scanner
			/// </summary>
			internal ScanData text_scan_data;

			/// <summary>
			/// copy of first character of this symbol
			/// </summary>
			internal int symbol_first_char;

			/// <summary>
			/// Copy of total warnings for this file
			/// </summary>
			internal int warnings;

			/// <summary>
			/// Previous file descriptor
			/// </summary>
			internal SourceList previous;

			internal SourceList(Text text)
			{
				text_scan_data = new ScanData(text);
			}
		}

		/// <summary>
		/// Maximum number of error markers per line
		/// </summary>
		private const int MAX_ECHO = 9;

		private const int EXIT_FAILURE = 1;

		/// <summary>
		/// Total number of errors this run
		/// </summary>
		private int totalerrors = 0;

		/// <summary>
		/// Total number of warnings this run
		/// </summary>
		private int totalwarnings = 0;

		/// <summary>
		/// Total errors for this file
		/// </summary>
		private int errors = 0;

		/// <summary>
		/// Crash if error count exceeds this value
		/// </summary>
		private int maxerrors = 25;

		/// <summary>
		/// Total warnings for this file
		/// </summary>
		private int warnings = 0;

		/// <summary>
		/// Crash if warning count exceeds this value
		/// </summary>
		private int maxwarnings = 100;

		/// <summary>
		/// Filename
		/// </summary>
		private string name = null;

		/// <summary>
		/// Current line in this file
		/// </summary>
		private int linenumber = 0;

		/// <summary>
		/// Cumulative line_number
		/// </summary>
		private int sequence_number = 0;

		/// <summary>
		/// TEXT_MESSAGES
		/// </summary>
		private TextWriter messages = Console.Out;

		/// <summary>
		/// Array of error positions
		/// </summary>
		private int[] echo_pos = new int[MAX_ECHO];

		/// <summary>
		/// current error number this line
		/// </summary>
		private int echo_num = -1;

		/// <summary>
		/// Current text character
		/// </summary>
		internal int text_char = ' ';

		/// <summary>
		/// First character of current source line
		/// </summary>
		private int first_char;

		/// <summary>
		/// Last character of current source line
		/// </summary>
		private int last_char;

		/// <summary>
		/// Pointer to current source character
		/// </summary>
		internal int text_current;

		/// <summary>
		/// Text array for storing id's and strings
		/// </summary>
		internal char[] text_bot = null;

		/// <summary>
		/// Top of text character
		/// </summary>
		internal int text_top = 1;

		/// <summary>
		/// Size of text buffer
		/// </summary>
		private int maxtext;

		/// <summary>
		/// Tab expansion width
		/// </summary>
		private int tabwidth;

		/// <summary>
		/// Pointer to the last thing read by the scanner
		/// </summary>
		internal ScanData text_scan_data;

		/// <summary>
		/// Enable line echoing
		/// </summary>
		private readonly bool echo = false;

		/// <summary>
		/// Current file handle
		/// </summary>
		private TextReader file;

		/// <summary>
		/// Head of file descriptor list
		/// </summary>
		private SourceList source_descriptor_list;

		/// <summary>
		/// First character in this symbol
		/// </summary>
		private int symbol_first_char;

		internal int text_column_number()
		{
			return first_char - text_current;
		}

		/// <summary>
		/// Advance text_current, reading another line if necessary
		/// </summary>
		internal void text_get_char()
		{
			if (text_current <= last_char)
			{
				if (file != null)
				{
					if (feof(file))
					{
						text_close();
						// pre-increment ready for pre-decrement!
						text_current++;
					}
				}
				if (file == null)
				{
					text_char = EOF;
					return;
				}
				while (text_current <= last_char)
				{
					if ((echo || echo_num >= 0) && linenumber > 0)
					{
						text_echo_line();
					}
					sequence_number++;
					linenumber++;
					// initialise pointers to empty line
					last_char = text_current = first_char;
					do
					{
						text_char = getc(file);
						text_bot[--last_char] = (char)text_char;
						if (text_char == EOF)
						{
							text_bot[last_char] = ' ';
						}
						else if (text_char == '\t' && tabwidth != 0)
						{
							// expand tabs to next tabstop
							text_bot[last_char] = ' '; // make tab a space
							while ((text_current - last_char) % tabwidth != 0)
							{
								text_bot[--last_char] = ' ';
							}
						}
					}
					// kludge to ensure delayed echoing of lines
					while (text_char != '\n' && text_char != EOF);
					text_bot[--last_char] = ' ';
				}
			}
			text_char = text_bot[--text_current];
		}

		internal string text_get_string(int start)
		{
			string s = "";
			while (text_bot[start] != 0)
			{
				s += text_bot[start++];
			}
			return s;
		}

		internal void text_init(int max_text, int max_errors, int max_warnings, int tab_width)
		{
			tabwidth = tab_width;
			maxtext = max_text;
			maxerrors = max_errors;
			maxwarnings = max_warnings;

			text_bot = new char[maxtext];
			text_top = 1;
			text_current = last_char = first_char = maxtext;
		}

		internal int text_insert_char(char c)
		{
			int start = text_top;
			if (text_top >= last_char)
			{
				text_message(TEXT_FATAL, "Ran out of text space\n");
			}
			else
			{
				text_bot[text_top++] = c;
			}
			return start;
		}

		internal int text_insert_characters(string str)
		{
			int start = text_top;
			foreach (char ch in str)
			{
				text_insert_char(ch);
			}
			return start;
		}

		internal int text_insert_integer(int n)
		{
			int start = text_top;
			if (n > 9)
			{
				// recursively handle multi-digit numbers
				text_insert_integer(n / 10);
			}
			text_insert_char((char)(n % 10 + '0'));
			return start;
		}

		internal int text_insert_string(string str)
		{
			int start = text_top;
			foreach (char ch in str)
			{
				text_insert_char(ch);
			}
			text_insert_char((char)0);
			return start;
		}

		/// <summary>
		/// Put an id_number into text buffer
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="str"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		internal int text_insert_substring(string prefix, string str, int n)
		{
			int start = text_top;

			text_insert_characters(prefix);
			text_insert_char('_');
			text_insert_characters(str);
			text_insert_char('_');
			text_insert_integer(n);
			text_insert_char('\0');
			return start;
		}

		internal static bool text_is_valid_C_id(string s)
		{
			bool temp = true;
			foreach (char ch in s)
			{
				temp = temp && (isalnum(ch) || ch == '_');
			}
			return temp;
		}

		internal int text_line_number()
		{
			return linenumber;
		}

		internal int text_message(TextMessageType type, string message)
		{
			if (message == null)
			{
				return 0;
			}
			if (type == TEXT_INFO_ECHO || type == TEXT_WARNING_ECHO || type == TEXT_ERROR_ECHO || type == TEXT_FATAL_ECHO)
			{
				if (++echo_num < MAX_ECHO)
				{
					echo_pos[echo_num] = first_char - text_current;
				}
			}
			text_echo_line_number();
			switch (type)
			{
				case TEXT_INFO:
				case TEXT_INFO_ECHO:
					break;
				case TEXT_WARNING:
				case TEXT_WARNING_ECHO:
					warnings++;
					totalwarnings++;
					messages.Write("Warning ");
					break;
				case TEXT_ERROR:
				case TEXT_ERROR_ECHO:
					errors++;
					totalerrors++;
					messages.Write("Error ");
					break;
				case TEXT_FATAL:
				case TEXT_FATAL_ECHO:
					messages.Write("Fatal ");
					break;
				default:
					messages.Write("Unknown ");
					break;
			}
			if (type == TEXT_WARNING_ECHO || type == TEXT_ERROR_ECHO)
			{
				messages.Write(echo_num + 1);
			}
			if (name != null && linenumber != 0)
			{
				messages.Write("(" + name + ") ");
			}
			else if (type != TEXT_INFO && type != TEXT_INFO_ECHO)
			{
				messages.Write("- ");
			}
			messages.Write(message);
			if (type == TEXT_FATAL || type == TEXT_FATAL_ECHO)
			{
				Environment.Exit(EXIT_FAILURE);
			}
			if (errors > maxerrors && maxerrors > 0)
			{
				messages.WriteLine("Fatal (" + (name == null ? "null file" : name) + "): too many errors");
				Environment.Exit(EXIT_FAILURE);
			}
			if (warnings > maxwarnings && maxwarnings > 0)
			{
				messages.WriteLine("Fatal (" + (name == null ? "null file" : name) + "): too many warnings");
				Environment.Exit(EXIT_FAILURE);
			}
			return message.Length + 1;
		}

		internal TextReader text_open(string fileName, TextReader textReader)
		{
			TextReader handle = null;
			try
			{
				handle = textReader;
			}
			catch (FileNotFoundException)
			{
				handle = null;
			}
			TextReader old = file;
			if (handle != null) // we found a file
			{
				if (old != null) // save current file context
				{
					SourceList temp = new SourceList(this);
					// load descriptor block
					temp.errors = errors;
					temp.file = file;
					temp.first_char = first_char;
					temp.last_char = last_char;
					temp.linenumber = linenumber;
					temp.name = name;
					temp.text_char = text_char;
					temp.text_current = text_current;
					temp.text_scan_data.memcpy(text_scan_data);
					temp.symbol_first_char = symbol_first_char;
					temp.warnings = warnings;
					// link descriptor block into head of list
					temp.previous = source_descriptor_list;
					source_descriptor_list = temp;
				}
				// re-initialise file context
				errors = 0;
				file = handle;
				linenumber = 0;
				name = fileName;
				warnings = 0;
				if (echo)
				{
					text_message(TEXT_INFO, "\n");
				}
				// make new buffer region below current line
				text_current = last_char = first_char = last_char - 1;
			}
			return handle;
		}

		internal void text_print_time()
		{
			// string __DATE__ = new SimpleDateFormat("dd-MM-yyyy").format(new Date());
			// string __TIME__ = new SimpleDateFormat("HH:mm:ss").format(new Date());
			// text_printf(__DATE__ + " " + __TIME__);
			text_printf("Sep 19 2015 11:45:00");
		}

		internal int text_printf(string str)
		{
			if (str != null)
			{
				foreach (char ch in str)
				{
					if (ch == '\n')
					{
						messages.Write('\r');
					}
					messages.Write("" + ch);
				}
			}
			return str == null ? 0 : str.Length;
		}

		internal void text_redirect(TextWriter file)
		{
			messages = file;
		}

		internal int text_sequence_number()
		{
			return sequence_number;
		}

		internal int text_total_errors()
		{
			return totalerrors;
		}

		internal static string text_uppercase_string(string str)
		{
			return str.ToUpper();
		}

		private void text_close()
		{
			if (file == null)
				return;

			linenumber = 0;
			fclose(file);
			file = null;
			// unload next file if there is one
			if (source_descriptor_list != null)
			{
				SourceList temp = source_descriptor_list;
				source_descriptor_list = source_descriptor_list.previous;
				errors = temp.errors;
				file = temp.file;
				first_char = temp.first_char;
				last_char = temp.last_char;
				linenumber = temp.linenumber;
				name = temp.name;
				text_char = temp.text_char;
				text_current = temp.text_current;
				text_scan_data.memcpy(temp.text_scan_data);
				symbol_first_char = temp.symbol_first_char;
				warnings = temp.warnings;
				if (echo)
				{
					text_message(TEXT_INFO, "\n");
					text_echo_line();
				}
			}
		}

		private void text_echo_line()
		{
			text_echo_line_number();
			// current input line is stored in reverse order at top of text buffer:
			// print backwards from last character of text buffer
			for (int temp = first_char - 1; temp > last_char; temp--)
			{
				messages.Write(text_bot[temp]);
			}
			// now print out the echo number line
			if (echo_num >= 0)
			{
				int num_count = -1, char_count = 1;
				// only the first MAX_ECHO errors have pointers
				if (echo_num >= MAX_ECHO)
					echo_num = MAX_ECHO - 1;
				text_echo_line_number();
				while (++num_count <= echo_num)
				{
					while (char_count++ < echo_pos[num_count] - 1)
					{
						messages.Write('-');
					}
					messages.Write((char)('1' + num_count));
				}
				messages.WriteLine();
			}
			// reset echo numbering array pointer
			echo_num = -1;
		}

		private void text_echo_line_number()
		{
			if (linenumber != 0)
			{
				string s = $"{linenumber,6}: ";
				messages.Write(s);
			}
			else
			{
				messages.Write("******: ");
			}
		}
	}
}
