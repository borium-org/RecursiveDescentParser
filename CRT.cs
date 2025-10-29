using System;
using System.IO;

namespace Borium.RDP
{
	internal class CRT
	{
		internal const int EOF = -1;

#if false
		internal static String capitalizeFirst(String text)
		{
			return Character.toUpperCase(text.charAt(0)) + text.substring(1);
		}
#endif

		internal static void fclose(TextReader file)
		{
			try
			{
				file.Close();
			}
			catch (IOException)
			{
			}
		}

		internal static bool feof(TextReader file)
		{
			try
			{
				return file.Peek() == EOF;
			}
			catch (IOException)
			{
			}
			return true;
		}

		internal static int getc(TextReader file)
		{
			try
			{
				return file.Read();
			}
			catch (IOException)
			{
			}
			return EOF;
		}

		internal static bool isalnum(int ch)
		{
			return isalpha(ch) || isdigit(ch);
		}

		internal static bool isalpha(int ch)
		{
			return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".IndexOf((char)ch) >= 0;
		}

		internal static bool isdigit(int ch)
		{
			return "0123456789".IndexOf((char)ch) >= 0;
		}

		internal static bool isgraph(char ch)
		{
			return ch > ' ';
		}

		internal static bool isprint(int ch)
		{
			return ch >= ' ';
		}

		internal static bool isspace(int ch)
		{
			return ch <= ' ';
		}

		internal static bool isxdigit(int ch)
		{
			return "0123456789ABCDEFabcdef".IndexOf((char)ch) >= 0;
		}

		internal static int strcmp(String str1, String str2)
		{
			return str1.CompareTo(str2);
		}

		internal static long strtol(string nptr, string[] endptr, int fromBase)
		{
			if (endptr != null)
			{
				throw new NotImplementedException("strtol: endptr is not implemented");
			}
			long result = Convert.ToInt64(nptr, fromBase);
			return result;
		}
	}
}
