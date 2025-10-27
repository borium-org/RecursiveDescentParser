namespace Borium.RDP
{
	internal class SymbolTable
	{
		/// <summary>
		/// An identifying string
		/// </summary>
		internal string name;

		/// <summary>
		/// Table of pointers to hash lists
		/// </summary>
		internal Symbol[] table;

		/// <summary>
		/// Pointers to chain of scope lists
		/// </summary>
		internal SymbolScopeData current;

		/// <summary>
		/// Pointer to first scope list
		/// </summary>
		internal SymbolScopeData scopes;

		/// <summary>
		/// Number of buckets in symbol table
		/// </summary>
		internal int hash_size;

		/// <summary>
		/// Hashing prime: hashsize and hashprime should be coprime
		/// </summary>
		internal int hash_prime;

		internal CompareHashPrint compareHashPrint;

		/// <summary>
		/// Pointer to last declared symbol table
		/// </summary>
		internal SymbolTable next;

		int compare(string key, Symbol p)
		{
			return compareHashPrint.compare(key, p);
		}

		/** return current scope */
		SymbolScopeData getScope()
		{
			return current;
		}

		int hash(int prime, string key)
		{
			return compareHashPrint.hash(prime, key);
		}
	}
}
