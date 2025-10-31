using System;
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

		internal static int set_cardinality(Set src)
		{
			return src == null ? 0 : src.cardinality();
		}

		internal static int set_print_element(int element, String[] element_names, boolean comments)
		{
			if (element_names == null)
			{
				return text_printf(Integer.toString(element));
			}
			else
			{
				String elementString = element_names[element];
				if (!comments)
					elementString = elementString.split(" ")[0];
				return text_printf(elementString);
			}
		}
#endif

		private uint[] data = new uint[10];

#if false
		/** clear a dst and then set only those bits specified by src */
		internal void assign(int element)
		{
			clear();
			set(element);
		}
#endif

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

#if false
		internal boolean includes(int element)
		{
			grow(element);
			int index = element / 32;
			element &= 0x1F;
			return (data[index] & 1 << element) != 0;
		}

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

		internal void print(String[] element_names, int line_length)
		{
			int column = 0;
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
					column = 0;
				}
				column += set_print_element(element, element_names, true);
			}
		}

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

		private Integer[] array()
		{
			ArrayList<Integer> elements = new ArrayList<>();
			for (int word = 0; word < data.length; word++)
			{
				for (int bit = 0; bit < 32; bit++)
				{
					if ((data[word] & 1 << bit) != 0)
					{
						elements.add(word * 32 + bit);
					}
				}
			}
			return elements.toArray(new Integer[0]);
		}

		private int cardinality()
		{
			int[] bitCounts = new int[] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };

			int cardinality = 0;
			for (long bits : data)
			{
				for (int i = 0; i < 8; i++)
				{
					cardinality += bitCounts[(int)(bits & 0xF)];
					bits >>= 4;
				}
			}
			return cardinality;
		}
#endif

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
