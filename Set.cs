using System.Collections.Generic;
using static Borium.RDP.Text;

namespace Borium.RDP
{
	internal class Set
	{
#if false
	internal interface Indent
	{
		int indent();
	}
#endif

		internal static int set_cardinality(Set src)
		{
			return src == null ? 0 : src.cardinality();
		}

		internal static int set_print_element(int element, string[] element_names, bool comments)
		{
			if (element_names == null)
			{
				return text_printf($"{element}");
			}
			else
			{
				string elementString = element_names[element];
				if (!comments)
				{
					elementString = elementString.Split(' ')[0];
				}
				return text_printf(elementString);
			}
		}

		private uint[] data = new uint[10];


		/// <summary>
		/// Clear a dst and then set only those bits specified by src
		/// </summary>
		/// <param name="element"></param>
		internal void assign(int element)
		{
			clear();
			set(element);
		}

		internal void assignList(params int[] bits)
		{
			clear();
			foreach (int bit in bits)
			{
				set(bit);
			}
		}

#if false
		/** assign one set to another */
		internal void assignSet(Set src)
	{
		clear();
		unite(src);
	}
#endif

		internal void clear()
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = 0;
			}
		}

		internal bool includes(int element)
		{
			grow(element);
			int index = element / 32;
			element &= 0x1F;
			return (data[index] & (1 << element)) != 0;
		}

#if false
	internal void intersect(Set src)
	{
		/* only iterate over shortest set */
		int length = length() < src.length() ? length() : src.length();
		for (int i = 0; i < length; i++)
		{
			data[i] &= src.data[i];
		}
		/* Now clear rest of dst */
		while (length < length())
		{
			data[length++] = 0;
		}
	}
#endif

		internal void print(string[] element_names, int line_length)
		{
			int column = 0;
			bool not_first = false;
			int[] elements = array();
			foreach (int element in elements)
			{
				if (not_first)
				{
					column += text_printf(", ");
				}
				else
				{
					not_first = true;
				}

				if (line_length != 0 && column >= line_length)
				{
					text_printf("\n");
					column = 0;
				}
				column += set_print_element(element, element_names, true);
			}
		}

#if false
		internal void print(String[] element_names, int initialOffset, Indent indent, int line_length, boolean comments)
	{
		int column = initialOffset;
		boolean not_first = false;
		Integer[] elements = array();
		for (int element : elements)
		{
			if (not_first)
			{
				column += text_printf(", ");
			}
			else
			{
				not_first = true;
			}

			if (line_length != 0 && column >= line_length)
			{
				text_printf("\n");
				column = indent.indent();
			}
			column += set_print_element(element, element_names, comments);
		}
	}
#endif

		internal void set(int element)
		{
			grow(element);
			int index = element / 32;
			element &= 0x1F;
			data[index] |= (uint)(1 << element);
		}

#if false
		internal void unite(Set src)
	{
		grow(src.length());
		for (int i = 0; i < data.length; i++)
		{
			data[i] |= src.data[i];
		}
	}
#endif

		private int[] array()
		{
			List<int> elements = new List<int>();
			for (int word = 0; word < data.Length; word++)
			{
				for (int bit = 0; bit < 32; bit++)
				{
					if ((data[word] & 1 << bit) != 0)
					{
						elements.Add(word * 32 + bit);
					}
				}
			}
			return elements.ToArray();
		}

		private int cardinality()
		{
			int[] bitCounts = new int[] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };

			int cardinality = 0;
			foreach (uint bits in data)
			{
				uint bits1 = bits;
				for (int i = 0; i < 8; i++)
				{
					cardinality += bitCounts[(int)(bits1 & 0xF)];
					bits1 >>= 4;
				}
			}
			return cardinality;
		}

		private void grow(int bits)
		{
			int index = (bits + 31) / 32;
			if (index >= data.Length)
			{
				uint[] newData = new uint[index + 5];
				for (int i = 0; i < newData.Length; i++)
				{
					newData[i] = 0;
				}
				for (int i = 0; i < data.Length; i++)
				{
					newData[i] = data[i];
				}
				data = newData;
			}
		}

#if false
		private int length()
	{
		return data.length;
	}
#endif
	}
}