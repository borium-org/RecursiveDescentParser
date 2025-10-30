using System;

namespace Borium.RDP
{
	internal class SymbolTable
	{
#if false
		/** an identifying string */
		String name;

		/** table of pointers to hash lists */
		Symbol[] table;

		/** pointers to chain of scope lists */
		SymbolScopeData current;

		/** pointer to first scope list */
		SymbolScopeData scopes;

		/** number of buckets in symbol table */
		int hash_size;
		/** hashing prime: hashsize and hashprime should be coprime */
		int hash_prime;
		CompareHashPrint compareHashPrint;

		/** pointer to last declared symbol table */
		SymbolTable next;

		int compare(String key, Symbol p)
		{
			return compareHashPrint.compare(key, p);
		}

		/** return current scope */
		SymbolScopeData getScope()
		{
			return current;
		}

		int hash(int prime, String key)
		{
			return compareHashPrint.hash(prime, key);
		}
#endif
	}
}
