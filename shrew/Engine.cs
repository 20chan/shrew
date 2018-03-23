using System;
using System.IO;

namespace shrew
{
    public class Engine
    {
        public Engine(string code)
        {

        }

        /// <summary>
        /// Execute code
        /// </summary>
        /// <returns>Return value of main procedure</returns>
        public static int Execute(string code)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute one line expresssion
        /// </summary>
        /// <returns>Value of expression</returns>
        public static object EvaluateExpr(string code)
        {
            throw new NotImplementedException();
        }

        public static T EvaluateExpr<T>(string code)
            => (T)EvaluateExpr(code);

        /// <summary>
        /// Get procedure by its name
        /// </summary>
        public Delegate this[string name]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
