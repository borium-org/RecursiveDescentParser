using System;
using static Borium.RDP.Text;

namespace Borium.RDP
{
	internal class CompareHashPrint
	{
#if false
		int compare(String key, Symbol p)
		{
			String r = text_get_string(p.id);
			return key.compareTo(r);
		}

		int compare(Symbol left, Symbol right)
		{
			String l = text_get_string(left.id);
			String r = text_get_string(right.id);
			return l.compareTo(r);
		}

		int hash(int hash_prime, String str)
		{
			int hashnumber = 0;
			if (str != null)
			{
				for (int i = 0; i < str.length(); i++)
				{
					hashnumber = str.charAt(i) + hash_prime * hashnumber;
				}
			}
			return hashnumber & 0x7FFFFFFF;
		}

		void print(Symbol s)
		{
			s.print();
		}
#endif
	}
}
