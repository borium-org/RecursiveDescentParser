using System;
using System.IO;

namespace Borium.RDP
{
	internal class CRT
	{
#if false
		public static final int EOF = -1;

		public static String capitalizeFirst(String text)
		{
			return Character.toUpperCase(text.charAt(0)) + text.substring(1);
		}

		public static void fclose(InputStream file)
		{
			try
			{
				file.close();
			}
			catch (IOException e)
			{
			}
		}

		public static boolean feof(InputStream file)
		{
			try
			{
				return file.available() == 0;
			}
			catch (IOException e)
			{
			}
			return true;
		}

		public static int getc(InputStream file)
		{
			try
			{
				return file.read();
			}
			catch (IOException e)
			{
			}
			return EOF;
		}

		public static boolean isalnum(int ch)
		{
			return isalpha(ch) || isdigit(ch);
		}

		public static boolean isalpha(int ch)
		{
			return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".indexOf(ch) >= 0;
		}

		public static boolean isdigit(int ch)
		{
			return "0123456789".indexOf(ch) >= 0;
		}

		public static boolean isgraph(char ch)
		{
			return ch > ' ';
		}

		public static boolean isprint(int ch)
		{
			return ch >= ' ';
		}

		public static boolean isspace(int ch)
		{
			return ch <= ' ';
		}

		public static boolean isxdigit(int ch)
		{
			return "0123456789ABCDEFabcdef".indexOf(ch) >= 0;
		}

		public static int strcmp(String str1, String str2)
		{
			return str1.compareTo(str2);
		}

		public static long strtol(String nptr, Pointer<String> endptr, int base)
		{
			if (endptr != null)
			{
				System.err.println("strtol: endptr is not implemented");
			}
			long result = Long.parseLong(nptr, base);
			return result;
		}
#endif
	}
}
