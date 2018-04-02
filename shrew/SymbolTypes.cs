using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace shrew
{
    public class SymbolTypes : IEnumerable<KeyValuePair<string, Type[]>>
    {
        private Dictionary<string, Type[]> _symbols;

        public SymbolTypes()
        {
            _symbols = new Dictionary<string, Type[]>();
        }

        public bool ContainsKey(string key)
            => _symbols.ContainsKey(key);

        public void Add(string name, params Type[] parameters)
            => _symbols.Add(name, parameters);

        public IEnumerator<KeyValuePair<string, Type[]>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Type[]>>)_symbols).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Type[]>>)_symbols).GetEnumerator();
        }

        public Type[] this[string name]
            => _symbols[name];
    }
}
