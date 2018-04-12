using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace shrew
{
    public class SymbolTable
    {
        private List<Pattern> _patterns;

        public SymbolTable Parent { get; private set; }

        public SymbolTable(SymbolTable parent = null,
            Dictionary<string, Delegate> args = null,
            Dictionary<string, object[]> pattern = null)
        {
            _patterns = new List<Pattern>();
            foreach (var p in pattern)
            {
                Add(p.Key, p.Value);
            }
            foreach (var p in args)
            {
                Add(p.Key, p.Value);
            }
        }

        public void Add(string name, Delegate func)
        {
            if (ContainsKeyHere(name))
                GetHere(name).Add(func);
            else
                _patterns.Add(new Pattern(name, func));
        }

        public void Add(string name, object[] pattern, Delegate func = null)
        {
            if (ContainsKeyHere(name))
                GetHere(name).Add(pattern, func);
            else
                _patterns.Add(new Pattern(name, Tuple.Create(pattern, func)));
        }

        private bool ContainsKeyHere(string name)
            => _patterns.Any(p => p.Name == name);

        public bool ContainsKey(string name)
        {
            if (ContainsKeyHere(name))
                return true;
            else if (Parent != null)
                return Parent.ContainsKey(name);
            else
                return false;
        }

        private Pattern GetHere(string name)
            => _patterns.FirstOrDefault(p => p.Name == name);

        public Pattern Get(string name)
        {
            if (ContainsKeyHere(name))
                return GetHere(name);
            else if (Parent != null)
                return Parent.Get(name);
            else
                return null;
        }
    }
}
