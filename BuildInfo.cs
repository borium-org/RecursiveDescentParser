using System;
using System.IO;
using System.Reflection;

namespace Borium.RDP
{
	/// <summary>
	/// Works if deterministic builds are turned off
	/// </summary>
	internal class BuildInfo
	{

		internal static DateTime GetLinkerTimestampLocal(Assembly assembly)
		{
			var location = assembly.Location;
			return GetLinkerTimestampLocal(location);
		}

		internal static DateTime GetLinkerTimestampLocal(string filePath)
		{
			const int peHeaderOffset = 60;
			const int linkerTimestampOffset = 8;

			var bytes = new byte[2048];
			using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				file.Read(bytes, 0, bytes.Length);
			}

			int headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
			uint secondsSince1970 = BitConverter.ToUInt32(bytes, headerPos + linkerTimestampOffset);

			var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return dt.AddSeconds(secondsSince1970).ToLocalTime();
		}
	}
}
