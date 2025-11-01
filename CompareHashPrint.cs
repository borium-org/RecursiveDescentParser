using System;
using static Borium.RDP.Text;

namespace Borium.RDP
{
	internal class CompareHashPrint
	{
		internal int compare(string key, Symbol p)
		{
			string r = text_get_string(p.id);
			return key.CompareTo(r);
		}

#if false
		internal int compare(Symbol left, Symbol right)
		{
			string l = text_get_string(left.id);
			string r = text_get_string(right.id);
			return l.CompareTo(r);
		}
#endif

		internal int hash(int hash_prime, string str)
		{
			int hashnumber = 0;
			if (str != null)
			{
				for (int i = 0; i < str.Length; i++)
				{
					hashnumber = str[i] + hash_prime * hashnumber;
				}
			}
			return hashnumber & 0x7FFFFFFF;
		}

#if false
		void print(Symbol s)
		{
			s.print();
		}
#endif
	}
}
