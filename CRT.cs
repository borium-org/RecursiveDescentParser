using System;
using System.IO;

namespace Borium.RDP
{
	internal class CRT
	{
		internal static long CurrentTimeMillis()
		{
			DateTime nowUtc = DateTime.UtcNow;

			// Define the Unix Epoch
			DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			// Calculate the TimeSpan between the DateTime and the Unix Epoch
			TimeSpan timeSinceEpoch = nowUtc - unixEpoch;

			// Get the total milliseconds
			long milliseconds = (long)timeSinceEpoch.TotalMilliseconds;

			return milliseconds;
		}
#if false
		internal static final int EOF = -1;

		internal static String capitalizeFirst(String text)
		{
			return Character.toUpperCase(text.charAt(0)) + text.substring(1);
		}

		internal static void fclose(InputStream file)
		{
			try
			{
				file.close();
			}
			catch (IOException e)
			{
			}
		}

		internal static boolean feof(InputStream file)
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

		internal static int getc(InputStream file)
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

		internal static boolean isalnum(int ch)
		{
			return isalpha(ch) || isdigit(ch);
		}

		internal static boolean isalpha(int ch)
		{
			return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".indexOf(ch) >= 0;
		}

		internal static boolean isdigit(int ch)
		{
			return "0123456789".indexOf(ch) >= 0;
		}
#endif

		internal static bool isgraph(char ch)
		{
			return ch > ' ';
		}

#if false
		internal static boolean isprint(int ch)
		{
			return ch >= ' ';
		}

		internal static boolean isspace(int ch)
		{
			return ch <= ' ';
		}

		internal static boolean isxdigit(int ch)
		{
			return "0123456789ABCDEFabcdef".indexOf(ch) >= 0;
		}

		internal static int strcmp(String str1, String str2)
		{
			return str1.compareTo(str2);
		}

		internal static long strtol(String nptr, Pointer<String> endptr, int base)
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
