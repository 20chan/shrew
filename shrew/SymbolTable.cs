using System;
using System.Collections.Generic;
using System.Linq;

namespace shrew
{
    public class SymbolTable
    {
        private SymbolTypes _symbols;
        private Dictionary<string, Delegate> _variables;

        private SymbolTable _parent;

        public SymbolTable(SymbolTable parent = null, Dictionary<string, Delegate> args = null)
        {
            _symbols = new SymbolTypes();
            _variables = new Dictionary<string, Delegate>();
            parent = _parent;
            if (args != null)
                Setargs(args);
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
            if (_parent != null)
                return _parent.Get(name);
            throw new KeyNotFoundException();
        }
    }
}
