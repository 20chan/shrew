using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace shrew
{
    public class SymbolTable : IEnumerable<KeyValuePair<string, Delegate>>
    {
        internal SymbolTypes _symbols;
        private Dictionary<string, Delegate> _variables;

        public SymbolTable Parent { get; private set; }

        public SymbolTable(SymbolTable parent = null, Dictionary<string, Delegate> args = null, SymbolTypes preTyped = null)
        {
            _symbols = new SymbolTypes(parent?._symbols);
            _variables = new Dictionary<string, Delegate>();
            Parent = parent;
            if (preTyped != null)
                SetType(preTyped);
            if (args != null)
                Setargs(args);
        }

        private void SetType(SymbolTypes types)
        {
            foreach (var t in types)
                _symbols.Add(t.Key, t.Value);
        }

        private void Setargs(Dictionary<string, Delegate> args)
        {
            foreach (var arg in args)
            {
                Set(arg.Key, arg.Value);
            }
        }

        public void Set(string name, Delegate value)
        {
            var paramTypes = value.Method.GetParameters().Select(p => p.ParameterType).ToArray();
            if (_symbols.ContainsKey(name))
            {
                var types = _symbols[name];
                if (types.Length != paramTypes.Length)
                    throw new Exception("파라미터 개수가 안맞음");
                bool something_bad = false;
                int i = 0;
                for (; i < types.Length; i++)
                    if (!types[i].Equals(paramTypes[i]))
                    {
                        something_bad = true;
                        break;
                    }

                if (something_bad)
                    throw new Exception($"{i}번째 파라미터 타입이 안맞음");
            }
            else
                _symbols.Add(name, paramTypes);

            if (_variables.ContainsKey(name))
                _variables[name] = value;
            else
                _variables.Add(name, value);
        }

        public Delegate Get(string name)
        {
            if (_variables.ContainsKey(name))
                return _variables[name];
            if (Parent != null)
                return Parent.Get(name);
            throw new KeyNotFoundException();
        }

        public IEnumerator<KeyValuePair<string, Delegate>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Delegate>>)_variables).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Delegate>>)_variables).GetEnumerator();
        }
    }
}
