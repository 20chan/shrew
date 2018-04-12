using System;
using System.Collections.Generic;
using System.Linq;

namespace shrew
{
    public class Pattern
    {
        public string Name { get; private set; }
        private List<object[]> _patterns;
        private List<Delegate> _functions;
        public List<bool> _initialized;

        public int ParameterCount;

        public Pattern(string name, int paramCount = -1)
        {
            Name = name;
            _patterns = new List<object[]>();
            _functions = new List<Delegate>();
            _initialized = new List<bool>();
            ParameterCount = paramCount;
        }

        public Pattern(string name, params Delegate[] functions) : this(name)
        {
            foreach (var f in functions)
                Add(f);
        }

        public Pattern(string name, params Tuple<object[], Delegate>[] patterns) : this(name)
        {
            foreach (var p in patterns)
                Add(p.Item1, p.Item2);
        }

        public void Add(Delegate func)
            => Add(GetTypes(func), func);

        public void Add(object[] pattern, Delegate func = null)
        {
            if (ParameterCount == -1)
                ParameterCount = pattern.Length;

            if (ParameterCount != pattern.Length ||
                ParameterCount != func.Method.GetParameters().Length)
                throw new Exception("Parameters count doesn't match");

            for (int i = 0; i < _patterns.Count; i++)
            {
                var p = _patterns[i];
                if (Enumerable.SequenceEqual(p, pattern))
                {
                    if (_initialized[i] || func == null)
                        throw new Exception("Pattern already initialized");
                    else
                    {
                        _functions[i] = func;
                        return;
                    }
                }
            }

            _initialized.Add(func != null);
            _patterns.Add(pattern);
            _functions.Add(func);
        }
        
        public Delegate GetMatchedFunc(object[] parameters, out bool notfound)
        {
            notfound = false;
            for (int i = 0; i < _patterns.Count; i++)
            {
                if (IsTypeMatching(_patterns[i], parameters))
                    return _functions[i];
            }
            notfound = true;
            return null;
        }

        public object DynamicInvoke(params object[] parameters)
        {
            return GetMatchedFunc(parameters, out var _).DynamicInvoke(parameters);
        }

        private Type[] GetTypes(Delegate func)
        {
            return func.Method.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        private static bool IsTypeMatching(object[] pattern, object[] parameters)
        {
            if (pattern.Length != parameters.Length)
                return false;
            
            for (int i = 0; i < pattern.Length; i++)
            {
                var pat = pattern[i];
                var par = parameters[i];
                if (pat is string s)
                {
                    if (!IsMatchingOne(s, par))
                        return false;
                }
                else if (pat is int it)
                {
                    if (!IsMatchingOne(it, par))
                        return false;
                }
                else if (pat is float f)
                {
                    if (!IsMatchingOne(f, par))
                        return false;
                }
                else if (pat is Type t)
                {
                    if (!par.GetType().IsAssignableFrom(t))
                        return false;
                }
                else
                    throw new Exception($"Unexpected type {pat.GetType()}");
            }
            return true;
        }

        private static bool IsMatchingOne<T>(T expected, object actual)
        {
            if (!actual.GetType().IsAssignableFrom(typeof(T)))
                return false;
            if (!actual.Equals(expected))
                return false;
            return true;
        }
    }
}
