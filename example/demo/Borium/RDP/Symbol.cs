using System;

namespace Borium.RDP
{
	internal class Symbol : IComparable<Symbol>
	{
		/** next symbol in hash list */
		internal Symbol next_hash;

		/** pointer to next pointer of last_symbol in hash list */
		internal Pointer<Symbol> last_hash = new Pointer<Symbol>();

		/** next symbol in scope list */
		internal Symbol next_scope;

		/** pointer to the scope symbol */
		internal Symbol scope;

		/** hash value for quick searching */
		internal int hash;

		internal int id;

		private Text text;

		public Symbol(Text text)
		{
			this.text = text;
		}

		public int CompareTo(Symbol other)
		{
			return string.Compare(text.text_get_string(id), text.text_get_string(other.id), StringComparison.Ordinal);
		}

		/// <summary>
		/// Return next symbol in scope chain. Return NULL if at end
		/// </summary>
		/// <returns></returns>
		internal Symbol nextSymbolInScope()
		{
			return next_scope;
		}

		internal void print(Text text)
		{
			text.text_printf(id == 0 ? "Null symbol" : text.text_get_string(id));
		}

		internal void unlinkSymbol()
		{
			Symbol s = this;

			s.last_hash.set(s.next_hash); /* point previous pointer to next symbol */
			if (s.next_hash != null)
			{
				s.next_hash.last_hash.set(s.last_hash.value());
			}
		}
	}
}
