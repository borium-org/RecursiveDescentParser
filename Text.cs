using System;
using System.IO;
using static Borium.RDP.CRT;
using static Borium.RDP.Scan;
using static Borium.RDP.Text.TextMessageType;

namespace Borium.RDP
{
	internal class Text
	{
		internal enum TextMessageType
		{
			TEXT_INFO, TEXT_WARNING, TEXT_ERROR, TEXT_FATAL, TEXT_INFO_ECHO, TEXT_WARNING_ECHO, TEXT_ERROR_ECHO,
			TEXT_FATAL_ECHO
		}

#if false
		private static class SourceList
		{
			/** copy of filename */
			String name;

			/** copy of total errors for this file */
			int errors;

			/** copy of current file handle */
			InputStream file;

			/** copy of first character of current source line */
			int first_char;

			/** copy of last character of current source line */
			int last_char;

			/** copy of current line in this file */
			int linenumber;

			/** copy of current text character */
			int text_char;

			/** copy of pointer to current source character */
			int text_current;

			/** copy of pointer to the last thing read by the scanner */
			ScanData text_scan_data = new ScanData();

			/** copy of first character of this symbol */
			int symbol_first_char;

			/** copy of total warnings for this file */
			int warnings;

			/** previous file descriptor */
			SourceList previous;
		}
#endif

		/// <summary>
		/// Maximum number of error markers per line
		/// </summary>
		private const int MAX_ECHO = 9;

		private const int EXIT_FAILURE = 1;

		/// <summary>
		/// Total number of errors this run
		/// </summary>
		private static int totalerrors = 0;

		/// <summary>
		/// Total number of warnings this run
		/// </summary>
		private static int totalwarnings = 0;

		/// <summary>
		/// Total errors for this file
		/// </summary>
		private static int errors = 0;

		/// <summary>
		/// Crash if error count exceeds this value
		/// </summary>
		private static int maxerrors = 25;

		/// <summary>
		/// Total warnings for this file
		/// </summary>
		private static int warnings = 0;

		/// <summary>
		/// Crash if warning count exceeds this value
		/// </summary>
		private static int maxwarnings = 100;

		/// <summary>
		/// Filename
		/// </summary>
		private static string name = null;

		/// <summary>
		/// Current line in this file
		/// </summary>
		private static int linenumber = 0;

#if false
		/** cumulative line_number */
		private static int sequence_number = 0;
#endif

		/// <summary>
		/// TEXT_MESSAGES
		/// </summary>
		private static TextWriter messages = Console.Out;

		/// <summary>
		/// Array of error positions
		/// </summary>
		private static int[] echo_pos = new int[MAX_ECHO];

		/// <summary>
		/// current error number this line
		/// </summary>
		private static int echo_num = -1;

#if false
		/** current text character */
		static int text_char = ' ';
#endif

		/// <summary>
		/// First character of current source line
		/// </summary>
		private static int first_char;

#if false
		/** last character of current source line */
		private static int last_char;
#endif

		/// <summary>
		/// Pointer to current source character
		/// </summary>
		static int text_current;

#if false
		/** text array for storing id's and strings */
		static char[] text_bot = null;

		/** top of text character */
		static int text_top = 1;

		/** size of text buffer */
		private static int maxtext;

		/** tab expansion width */
		private static int tabwidth;

		static ScanData text_scan_data; // pointer to the last thing read by the scanner

		/** enable line echoing */
		private static boolean echo;

		/** current file handle */
		private static InputStream file;

		/** head of file descriptor list */
		private static SourceList source_descriptor_list;

		/** first character in this symbol */
		private static int symbol_first_char;

		internal static int text_column_number()
		{
			return first_char - text_current;
		}
#endif

		internal static string text_default_filetype(string fname, string ftype)
		{
			if (ftype.Length == 0)
			{
				return fname;
			}
			string fullname = fname;
			if (fullname.IndexOf('.') == -1)
			{
				fullname += "." + ftype;
			}
			return fullname;
		}

#if false
		internal static void text_echo(boolean i)
		{
			echo = i;
		}

		internal static String text_extract_filename(String fname)
		{
			String name = fname;
			// search backwards for '.' and terminate the string there
			int temp = name.length();
			while (--temp > 0)
			{
				if (name.charAt(temp) == '.')
				{
					name = name.substring(0, temp);
					break;
				}
			}
			// we didn't find a dot, so start again at the end
			if (temp != name.length())
			{
				temp = fname.length();
			}
			// search backwards for '/' or '\' and start the string there
			while (--temp > 0)
			{
				if (name.charAt(temp) == '/' || name.charAt(temp) == '\\')
				{
					name = name.substring(temp + 1);
					break;
				}
			}
			return name;
		}

		/** add a new filetype. If ftype is NULL, return just filename */
		internal static String text_force_filetype(String fname, String ftype)
		{
			// work backwards from end of filename looking for a dot, or a directory separator
			int length = fname.length() - 1;
			while (fname.charAt(length) != '.' && fname.charAt(length) != '/' && fname.charAt(length) != '\\' && length > 0)
			{
				length--;
			}
			if (fname.charAt(length) != '.')
			{
				length = fname.length();
			}
			String fullname = null;
			if (ftype == null)
			{
				fullname = fname;
			}
			else
			{
				fullname = fname.substring(0, length) + "." + ftype;
			}
			return fullname;
		}

		/** advance text_current, reading another line if necessary */
		internal static void text_get_char()
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

		internal static String text_get_string(int start)
		{
			String s = "";
			while (text_bot[start] != 0)
			{
				s += text_bot[start++];
			}
			return s;
		}

		internal static void text_init(int max_text, int max_errors, int max_warnings, int tab_width)
		{
			tabwidth = tab_width;
			maxtext = max_text;
			maxerrors = max_errors;
			maxwarnings = max_warnings;

			text_bot = new char[maxtext];
			text_top = 1;
			text_current = last_char = first_char = maxtext;
		}

		internal static int text_insert_char(char c)
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

		internal static int text_insert_characters(String str)
		{
			int start = text_top;
			for (char ch : str.toCharArray())
			{
				text_insert_char(ch);
			}
			return start;
		}

		internal static int text_insert_integer(int n)
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

		internal static int text_insert_string(String str)
		{
			int start = text_top;
			for (char ch : str.toCharArray())
			{
				text_insert_char(ch);
			}
			text_insert_char((char)0);
			return start;
		}

		/** put an id_number into text buffer */
		internal static int text_insert_substring(String prefix, String str, int n)
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

		internal static boolean text_is_valid_C_id(String s)
		{
			boolean temp = true;
			for (char ch : s.toCharArray())
			{
				temp = temp && (isalnum(ch) || ch == '_');
			}
			return temp;
		}

		internal static int text_line_number()
		{
			return linenumber;
		}
#endif

		internal static int text_message(TextMessageType type, String message)
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

#if false
		internal static InputStream text_open(String s)
		{
			InputStream handle = null;
			try
			{
				handle = s.equals("-") ? System.in : new FileInputStream(s);
			}
			catch (FileNotFoundException e)
			{
				handle = null;
			}
			InputStream old = file;
			if (handle != null) // we found a file
			{
				if (old != null) // save current file context
				{
					SourceList temp = new SourceList();
					// load descriptor block
					temp.errors = errors;
					temp.file = file;
					temp.first_char = first_char;
					temp.last_char = last_char;
					temp.linenumber = linenumber;
					temp.name = name;
					temp.text_char = text_char;
					temp.text_current = text_current;
					memcpy(temp.text_scan_data, text_scan_data);
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
				name = s;
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

		internal static void text_print_statistics()
		{
			long symbolcount = text_top,

					linecount = -last_char + maxtext;

			if (text_bot == null)
			{
				text_message(TEXT_INFO, "Text buffer uninitialised\n");
			}
			else
			{
				text_message(TEXT_INFO, "Text buffer size " + maxtext + " bytes with " + (maxtext - symbolcount - linecount)
						+ " bytes free\n");
			}
		}

		internal static void text_print_time()
		{
			// String __DATE__ = new SimpleDateFormat("dd-MM-yyyy").format(new Date());
			// String __TIME__ = new SimpleDateFormat("HH:mm:ss").format(new Date());
			// text_printf(__DATE__ + " " + __TIME__);
			text_printf("Sep 19 2015 11:45:00");
		}

		internal static int text_printf(String str)
		{
			if (str != null)
			{
				for (char ch : str.toCharArray())
				{
					if (ch == '\n')
					{
						messages.print('\r');
					}
					messages.print("" + ch);
				}
			}
			return str == null ? 0 : str.length();
		}

		internal static void text_redirect(PrintStream file)
		{
			messages = file;
		}

		internal static int text_sequence_number()
		{
			return sequence_number;
		}

		internal static int text_total_errors()
		{
			return totalerrors;
		}

		internal static String text_uppercase_string(String str)
		{
			return str.toUpperCase();
		}

		private static void text_close()
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
				memcpy(text_scan_data, temp.text_scan_data);
				symbol_first_char = temp.symbol_first_char;
				warnings = temp.warnings;
				if (echo)
				{
					text_message(TEXT_INFO, "\n");
					text_echo_line();
				}
			}
		}

		private static void text_echo_line()
		{
			text_echo_line_number();
			// current input line is stored in reverse order at top of text buffer:
			// print backwards from last character of text buffer
			for (int temp = first_char - 1; temp > last_char; temp--)
			{
				messages.print(text_bot[temp]);
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
						messages.print('-');
					}
					messages.print((char)('1' + num_count));
				}
				messages.println();
			}
			// reset echo numbering array pointer
			echo_num = -1;
		}
#endif

		private static void text_echo_line_number()
		{
			if (linenumber != 0)
			{
				string s = $"{linenumber:N6}: ";
				messages.Write(s);
			}
			else
			{
				messages.Write("******: ");
			}
		}
	}
}
