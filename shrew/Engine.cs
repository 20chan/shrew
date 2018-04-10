using System;
using System.Collections.Generic;
using System.Linq;
using shrew.Parsing;
using shrew.Syntax;

namespace shrew
{
    public class Engine
    {
        private SymbolTable _globals;

        public Engine(SymbolTable builtin = null)
        {
            _globals = new SymbolTable(builtin);
        }

        /// <summary>
        /// Execute code
        /// </summary>
        /// <returns>Return value of main procedure</returns>
        public static int Execute(string code)
        {
            var interpreter = new Engine();
            interpreter.ExecuteCode(code);
            return (int)interpreter._globals.Get("main").DynamicInvoke();
        }

        /// <summary>
        /// Execute one line expresssion
        /// </summary>
        /// <returns>Value of expression</returns>
        public static object EvaluateExpr(string code)
        {
            return new Engine().EvaluateCode(code);
        }

        public static T EvaluateExpr<T>(string code)
            => (T)EvaluateExpr(code);

        public void ExecuteCode(string code)
        {
            ExecuteAllStmts(Parser.Parse(code, _globals._symbols));
        }

        public object EvaluateCode(string code)
        {
            return EvaluateExpr(Parser.Parse(code, _globals._symbols).Nodes[0] as ExprNode, _globals);
        }

        protected void ExecuteAllStmts(StmtsNode root)
        {
            foreach (var node in root.Nodes)
                ExecuteStmt(node, _globals);
        }

        protected void ExecuteStmt(SyntaxNode node, SymbolTable env)
        {
            if (node is AssignNode)
            {
                ExecuteAssign(node as AssignNode, env);
            }
            else if (node as ExprNode != null)
            {
                EvaluateExpr(node as ExprNode, env);
            }
            else throw new Exception($"Unexpected node type {node.GetType()}");
        }

        protected void ExecuteAssign(AssignNode node, SymbolTable env)
        {
            var id = node.Left[0].Token.Text;
            Delegate function = null;
            switch (node.Left.Length)
            {
                case 1:
                    function = (Func<object>)(() => EvaluateExpr(node.Right, new SymbolTable(env)));
                    break;
                case 2:
                    {
                        object func(object arg1)
                        {
                            var scoped = new SymbolTable(env, new Dictionary<string, Delegate>
                            {
                                { node.Left[1].Token.Text, (Func<object>)(() => arg1) },
                            });
                            return EvaluateExpr(node.Right, scoped);
                        }
                        function = new Func<object, object>(func);
                        break;
                    }
                case 3:
                    {
                        object func(object arg1, object arg2)
                        {
                            var scoped = new SymbolTable(env, new Dictionary<string, Delegate>
                            {
                                { node.Left[1].Token.Text, (Func<object>)(() => arg1) },
                                { node.Left[2].Token.Text, (Func<object>)(() => arg2) },
                            });
                            return EvaluateExpr(node.Right, scoped);
                        }
                        function = new Func<object, object, object>(func);
                        break;
                    }
                case 4:
                    {
                        object func(object arg1, object arg2, object arg3)
                        {
                            var scoped = new SymbolTable(env, new Dictionary<string, Delegate>
                            {
                                { node.Left[1].Token.Text, (Func<object>)(() => arg1) },
                                { node.Left[2].Token.Text, (Func<object>)(() => arg2) },
                                { node.Left[3].Token.Text, (Func<object>)(() => arg3) },
                            });
                            return EvaluateExpr(node.Right, scoped);
                        }
                        function = new Func<object, object, object, object>(func);
                        break;
                    }
            }
            _globals.Set(id, function);
        }

        protected object EvaluateExpr(ExprNode node, SymbolTable env)
        {
            if (node is UnaryExprNode)
            {
                var un = node as UnaryExprNode;
                dynamic right = EvaluateExpr(un.Right, env);
                switch (un.Operator.TokenType)
                {
                    case SyntaxTokenType.ExclamationToken:
                        return !right;
                    case SyntaxTokenType.TildeToken:
                        return ~right;
                    case SyntaxTokenType.MinusToken:
                        return -right;
                }
            }
            if (node is BinaryExprNode)
            {
                var bin = node as BinaryExprNode;
                dynamic left = EvaluateExpr(bin.Left, env);
                dynamic right = EvaluateExpr(bin.Right, env);
                switch (bin.Operator.TokenType)
                {
                    case SyntaxTokenType.PlusToken:
                        return left + right;
                    case SyntaxTokenType.MinusToken:
                        return left - right;
                    case SyntaxTokenType.AsteriskToken:
                        return left * right;
                    case SyntaxTokenType.SlashToken:
                        return left / right;
                    case SyntaxTokenType.PercentToken:
                        return left % right;
                    case SyntaxTokenType.ConcatToken:
                        return string.Concat(left, right);
                    case SyntaxTokenType.GreaterToken:
                        return left > right;
                    case SyntaxTokenType.GreaterEqualToken:
                        return left >= right;
                    case SyntaxTokenType.LessToken:
                        return left < right;
                    case SyntaxTokenType.LessEqualToken:
                        return left <= right;
                    case SyntaxTokenType.EqualToken:
                        return left.Equals(right);
                    case SyntaxTokenType.NotEqualToken:
                        return !left.Equals(right);
                    case SyntaxTokenType.LShiftToken:
                        return left << right;
                    case SyntaxTokenType.RShiftToken:
                        return left >> right;
                    case SyntaxTokenType.AmperToken:
                        return left & right;
                    case SyntaxTokenType.CaretToken:
                        return left ^ right;
                    case SyntaxTokenType.VBarToken:
                        return left | right;
                    case SyntaxTokenType.DoubleAmperToken:
                        return left && right;
                    case SyntaxTokenType.DoubleVBarToken:
                        return left || right;
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
                return env.Get(id.Token.Text).DynamicInvoke();
            }
            else if (node is CallNode)
            {
                var call = node as CallNode;
                return env.Get(call.Function.Token.Text).DynamicInvoke(
                    call.Parameters
                        .Select(n => EvaluateExpr(n, env))
                        .ToArray());
            }
            else
            {
                throw new Exception();
            }
        }

        public object ExecuteOrEvaluate(string code)
        {
            var node = Parser.Parse(code, _globals._symbols) as StmtsNode;
            int i = 0;
            for (; i < node.Nodes.Length - 1; i++)
                ExecuteStmt(node.Nodes[i], _globals);
            var expr = node.Nodes[i] as ExprNode;
            if (expr == null)
            {
                ExecuteStmt(node.Nodes[i], _globals);
                return null;
            }
            return EvaluateExpr(expr, _globals);
        }

        /// <summary>
        /// Get procedure by its name
        /// </summary>
        public Delegate this[string name]
        {
            get => _globals.Get(name);
        }
    }
}
