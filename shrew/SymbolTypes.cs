using System;
using System.Collections;
using System.Collections.Generic;

namespace shrew
{
    public class SymbolTypes : IEnumerable<KeyValuePair<string, Type[]>>
    {
        internal SymbolTypes _parent;
        private Dictionary<string, Type[]> _symbols;

        public SymbolTypes(SymbolTypes parent = null)
        {
            _symbols = new Dictionary<string, Type[]>();
            _parent = parent;
        }

        public bool ContainsKey(string key)
        {
            if (_symbols.ContainsKey(key))
                return true;
            if (_parent != null)
                if (_parent.ContainsKey(key))
                    return true;
            return false;
        }

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
        {
            get
            {
                if (_symbols.ContainsKey(name))
                    return _symbols[name];
                if (_parent != null)
                    return _parent[name];
                else
                    throw new KeyNotFoundException();
            }
        }
    }
}
