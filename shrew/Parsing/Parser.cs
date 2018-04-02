using System;
using System.Linq;
using System.Collections.Generic;
using shrew.Lexing;
using shrew.Syntax;

namespace shrew.Parsing
{
    public class Parser
    {
        private readonly string _code;
        private readonly SymbolTypes _builtins;
        private readonly SymbolTypes _globals;
        private SyntaxToken[] _tokens;
        private int _index;

        private bool IsEOF
            => _index == _tokens.Length;

        private SyntaxToken Peek(int n = 0)
            => _tokens[_index + n];

        private SyntaxTokenType TopType
            => Peek().TokenType;

        private SyntaxToken Pop()
            => _tokens[_index++];

        private void Eat(SyntaxTokenType tok)
        {
            if (Pop().TokenType != tok)
                Error();
        }

        public Parser(string code, SymbolTypes builtins = null)
        {
            _code = code;
            _tokens = Lexer.Lex(code).ToArray();
            _builtins = builtins ?? new SymbolTypes();
            _globals = new SymbolTypes();
        }

        protected void Error()
        {
            throw new ParserException();
        }

        public static SyntaxNode Parse(string code, SymbolTypes builtins = null)
        {
            return new Parser(code, builtins).ParseStmts();
        }

        internal StmtsNode ParseStmts()
        {
            var nodes = new List<SyntaxNode>();

            while (!IsEOF)
                nodes.Add(Parse());

            return new StmtsNode(nodes.ToArray());
        }

        protected SyntaxNode Parse()
        {
            var idQueue = new Queue<IdentifierNode>();
            var startIndex = _index;
            bool isExpr = false;

            if (IsEOF) Error();
            do
            {
                if (Peek().TokenType != SyntaxTokenType.Identifier)
                {
                    // AssignToken => it's not expr
                    isExpr = Peek().TokenType != SyntaxTokenType.AssignToken;
                    break;
                }
                idQueue.Enqueue(new IdentifierNode(Pop()));
            }
            while (!IsEOF && !isExpr && Peek().TokenType != SyntaxTokenType.AssignToken);

            if (!isExpr)
            {
                if (IsEOF)
                    isExpr = true;
                else if (Peek().TokenType != SyntaxTokenType.AssignToken)
                    isExpr = true;
            }
            if (isExpr)
            {
                _index = startIndex;
                return ParseExpr(null);
            }

            Eat(SyntaxTokenType.AssignToken);
            var ids = idQueue.ToArray();
            var localScope = new SymbolTypes();
            foreach (var p in ids.Skip(1))
                localScope.Add(p.Token.Text);
            var rexpr = ParseExpr(localScope);
            // TODO: 타입 추론
            _globals.Add(ids[0].Token.Text, Enumerable.Repeat<Type>(null, ids.Length - 1).ToArray());
            return new AssignNode(ids, rexpr);
        }

        protected ExprNode ParseExpr(SymbolTypes local)
        {
            return ParseArith(local);
        }

        protected ExprNode ParseArith(SymbolTypes local)
        {
            var lexpr = ParseTerm(local);
            if (IsEOF) return lexpr;
            if (TopType == SyntaxTokenType.PlusToken || TopType == SyntaxTokenType.MinusToken)
            {
                var op = Pop();
                var rexpr = ParseArith(local);
                return new BinaryExprNode(lexpr, rexpr, op);
            }
            return lexpr;
        }

        protected ExprNode ParseTerm(SymbolTypes local)
        {
            var lexpr = ParseFactor(local, true);
            if (IsEOF) return lexpr;
            if (TopType == SyntaxTokenType.AsteriskToken || TopType == SyntaxTokenType.SlashToken)
            {
                var op = Pop();
                var rexpr = ParseTerm(local);
                return new BinaryExprNode(lexpr, rexpr, op);
            }
            return lexpr;
        }

        protected CallNode ParseCall(SymbolTypes local)
        {
            var id = new IdentifierNode(Pop());
            var name = id.Token.Text;
            Type[] pattern = null;
            var paramsQueue = new Queue<ExprNode>();

            if (local != null && local.ContainsKey(name))
                pattern = local[name];
            else if (_globals.ContainsKey(name))
                pattern = _globals[name];
            else if (_builtins.ContainsKey(name))
                pattern = _builtins[name];
            else
                Error();

            for (int i = 0; i < pattern.Length; i++)
            {
                var p = ParseFactor(local, false);
                // TODO: Type check
                paramsQueue.Enqueue(p);
            }

            return new CallNode(id, paramsQueue.ToArray());
        }
        
        protected ExprNode ParseFactor(SymbolTypes local, bool callAllowed)
        {
            if (TopType == SyntaxTokenType.LParenToken)
            {
                Eat(SyntaxTokenType.LParenToken);
                var expr = ParseExpr(local);
                Eat(SyntaxTokenType.RParenToken);

                return expr;
            }
            if (TopType == SyntaxTokenType.Identifier)
            {
                if (callAllowed)
                    return ParseCall(local);
                else
                    return new IdentifierNode(Pop());
            }
            if (TopType == SyntaxTokenType.IntegerLiteral || TopType == SyntaxTokenType.RealLiteral)
                return new LiteralNode(Pop());
            if (TopType == SyntaxTokenType.TrueKeyword || TopType == SyntaxTokenType.FalseKeyword)
                return new LiteralNode(Pop());

            Error();
            return null;
        }
    }
}
