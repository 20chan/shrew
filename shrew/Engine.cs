using System;
using System.Collections.Generic;
using System.IO;
using shrew.Parsing;
using shrew.Syntax;

namespace shrew
{
    public class Engine
    {
        private SymbolTable _table;
        public StmtsNode RootNode;

        private Parser _parser;

        public Engine(string code)
        {
            _parser = new Parser(code);
            RootNode = _parser.ParseStmts();
            _table = new SymbolTable();
        }

        public Engine()
        {
            RootNode = null;
            _table = new SymbolTable();
        }

        /// <summary>
        /// Execute code
        /// </summary>
        /// <returns>Return value of main procedure</returns>
        public static int Execute(string code)
        {
            var interpreter = new Engine(code);
            interpreter.ExecuteAllStmts();
            return (int)interpreter.Get("main").DynamicInvoke();
        }

        /// <summary>
        /// Execute one line expresssion
        /// </summary>
        /// <returns>Value of expression</returns>
        public static object EvaluateExpr(string code)
        {
            var interpreter = new Engine(code);
            int i;
            for (i = 0; i < interpreter.RootNode.Nodes.Length - 1; i++)
            {
                interpreter.ExecuteStmt(interpreter.RootNode.Nodes[i]);
            }
            var last = interpreter.RootNode.Nodes[i] as ExprNode;
            if (last == null)
                return null;
            return interpreter.EvaluateExpr(last);
        }

        public static T EvaluateExpr<T>(string code)
            => (T)EvaluateExpr(code);

        public void ExecuteAllStmts()
        {
            var root = RootNode as StmtsNode;
            foreach (var node in root.Nodes)
                ExecuteStmt(node);
        }

        protected void ExecuteStmt(SyntaxNode node)
        {
            if (node is AssignNode)
            {
                var assign = node as AssignNode;

                Set(assign.Left[0].Token.Text, (Func<object>)(() => EvaluateExpr(assign.Right)));
            }
            else
            {
                EvaluateExpr(node as ExprNode);
            }
        }

        protected object EvaluateExpr(ExprNode node)
        {
            if (node is BinaryExprNode)
            {
                var bin = node as BinaryExprNode;
                dynamic left = EvaluateExpr(bin.Left);
                dynamic right = EvaluateExpr(bin.Right);
                switch(bin.Operator.TokenType)
                {
                    case SyntaxTokenType.PlusToken:
                        return left + right;
                    case SyntaxTokenType.MinusToken:
                        return left - right;
                    case SyntaxTokenType.AsteriskToken:
                        return left * right;
                    case SyntaxTokenType.SlashToken:
                        return left / right;
                    default:
                        throw new Exception();
                }
            }
            else if (node is LiteralNode)
            {
                var lit = node as LiteralNode;
                return lit.Token.Value;
            }
            else if (node is IdentifierNode)
            {
                var id = node as IdentifierNode;
                return Get(id.Token.Text).DynamicInvoke();
            }
            else
            {
                throw new Exception();
            }
        }

        public object ExecuteOrEvaluate(string code)
        {
            var node = Parser.Parse(code) as StmtsNode;
            int i = 0;
            for (; i < node.Nodes.Length - 1; i++)
                ExecuteStmt(node.Nodes[i]);
            var expr = node.Nodes[i] as ExprNode;
            if (expr == null)
            {
                ExecuteStmt(node.Nodes[i]);
                return null;
            }
            return EvaluateExpr(expr);
        }

        /// <summary>
        /// Get procedure by its name
        /// </summary>
        public Delegate this[string name]
        {
            get => Get(name);
        }

        protected Delegate Get(string name)
            => _table.Get(name);

        protected void Set(string name, Delegate value)
            => _table.Set(name, value);
    }
}
