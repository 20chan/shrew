using System;
using System.Collections.Generic;
using System.IO;
using shrew.Parsing;
using shrew.Syntax;

namespace shrew
{
    public class Engine
    {
        public Dictionary<string, Func<object>> VariableTable;
        public StmtsNode RootNode;
        public Engine(string code)
        {
            RootNode = Parser.Parse(code);
            VariableTable = new Dictionary<string, Func<object>>();
        }

        /// <summary>
        /// Execute code
        /// </summary>
        /// <returns>Return value of main procedure</returns>
        public static int Execute(string code)
        {
            var interpreter = new Engine(code);
            interpreter.ExecuteAllStmts();
            return interpreter.Get<int>("main");
        }

        /// <summary>
        /// Execute one line expresssion
        /// </summary>
        /// <returns>Value of expression</returns>
        public static object EvaluateExpr(string code)
        {
            var interpreter = new Engine(code);
            var last = interpreter.RootNode.Nodes[interpreter.RootNode.Nodes.Length - 1] as ExprNode;
            if (last == null)
                return null;
            return interpreter.EvaluateExpr(last);
        }

        public static T EvaluateExpr<T>(string code)
            => (T)EvaluateExpr(code);

        protected void ExecuteAllStmts()
        {
            var root = RootNode as StmtsNode;
            foreach (var node in root.Nodes)
                ExecuteStmt(node);
        }

        protected object ExecuteExceptLast

        protected void ExecuteStmt(SyntaxNode node)
        {

        }

        protected object EvaluateExpr(ExprNode node)
        {

        }

        /// <summary>
        /// Get procedure by its name
        /// </summary>
        public Delegate this[string name]
        {
            get => VariableTable[name];
        }

        protected T Get<T>(string name)
        {
            return (T)VariableTable[name]();
        }

        protected void Set<T>(string name, T value)
        {
            if (VariableTable.ContainsKey(name))
                VariableTable[name] = () => value;
            else
                VariableTable.Add(name, () => value);
        }
    }
}
