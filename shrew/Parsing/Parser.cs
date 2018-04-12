using System;
using System.Linq;
using System.Collections.Generic;
using shrew.Lexing;
using shrew.Syntax;

using static shrew.Syntax.SyntaxTokenType;

namespace shrew.Parsing
{
    public class Parser
    {
        private readonly string _code;
        private readonly SymbolTable _globals;
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

        public Parser(string code, SymbolTable builtins)
        {
            _code = code;
            _tokens = Lexer.Lex(code).ToArray();
            _globals = new SymbolTable(builtins);
        }

        protected void Error()
        {
            throw new ParserException();
        }

        public static StmtsNode Parse(string code, SymbolTable builtins)
            => new Parser(code, builtins).ParseStmts();

        public static StmtsNode Parse(string code, SymbolTable builtins, out SymbolTable globals)
        {
            var parser = new Parser(code, builtins);
            var res = parser.ParseStmts();
            globals =  parser._globals;
            return res;
        }

        protected StmtsNode ParseStmts()
        {
            var nodes = new List<SyntaxNode>();

            while (!IsEOF)
                nodes.Add(Parse());

            return new StmtsNode(nodes.ToArray());
        }

        protected SyntaxNode Parse()
        {
            var tokQueue = new Queue<TokenNode>();
            var startIndex = _index;
            bool isExpr = false;

            if (IsEOF) Error();
            do
            {
                if (Peek().TokenType != Identifier
                    && !Peek().TokenType.IsLiteral())
                {
                    // AssignToken => it's not expr
                    isExpr = Peek().TokenType != AssignToken;
                    break;
                }
                if (Peek().TokenType == Identifier)
                    tokQueue.Enqueue(new IdentifierNode(Pop()));
                else
                    tokQueue.Enqueue(new LiteralNode(Pop()));
            }
            while (!IsEOF && !isExpr && Peek().TokenType != AssignToken);

            if (!isExpr)
            {
                if (IsEOF)
                    isExpr = true;
                else if (Peek().TokenType != AssignToken)
                    isExpr = true;
            }
            if (isExpr)
            {
                _index = startIndex;
                return ParseExpr(_globals);
            }

            Eat(AssignToken);
            var ids = tokQueue.ToArray();
            var localScope = new SymbolTable(_globals);
            foreach (var p in ids.Skip(1).Where(t => t.Token.TokenType == Identifier))
                localScope.Add(p.Token.Text, new object[] { });
            var rexpr = ParseExpr(localScope);
            // TODO: 타입 추론
            _globals.Add(ids[0].Token.Text, Enumerable.Repeat<Type>(null, ids.Length - 1).ToArray());
            return new AssignNode(ids, rexpr);
        }

        protected ExprNode ParseExpr(SymbolTable local)
        {
            return ParseDoubleOr(local);
        }

        protected ExprNode ParseDoubleOr(SymbolTable local)
            => ParseBinary(local, ParseDoubleAnd, DoubleVBarToken);

        protected ExprNode ParseDoubleAnd(SymbolTable local)
            => ParseBinary(local, ParseOr, DoubleAmperToken);

        protected ExprNode ParseOr(SymbolTable local)
            => ParseBinary(local, ParseXor, VBarToken);

        protected ExprNode ParseXor(SymbolTable local)
            => ParseBinary(local, ParseAnd, CaretToken);

        protected ExprNode ParseAnd(SymbolTable local)
            => ParseBinary(local, ParseCompare, AmperToken);

        protected ExprNode ParseCompare(SymbolTable local)
            => ParseBinary(local, ParseShift,
                GreaterToken, LessToken, GreaterEqualToken, LessEqualToken, EqualToken, NotEqualToken);

        protected ExprNode ParseShift(SymbolTable local)
            => ParseBinary(local, ParseArith, LShiftToken, RShiftToken);

        protected ExprNode ParseArith(SymbolTable local)
            => ParseBinary(local, ParseTerm, PlusToken, MinusToken, ConcatToken);

        protected ExprNode ParseTerm(SymbolTable local)
            => ParseBinary(local, ParseFactor, AsteriskToken, SlashToken, PercentToken);

        protected ExprNode ParseFactor(SymbolTable local)
        {
            if (TopType == MinusToken
                || TopType == ExclamationToken
                || TopType == TildeToken)
            {
                var op = Pop();
                var expr = ParseFactor(local);
                return new UnaryExprNode(expr, op);
            }
            return ParseAtom(local, true);
        }

        protected ExprNode ParseAtom(SymbolTable local, bool callAllowed)
        {
            if (TopType == LParenToken)
            {
                Eat(LParenToken);
                var expr = ParseExpr(local);
                Eat(RParenToken);

                return expr;
            }
            if (TopType == Identifier)
            {
                if (callAllowed)
                    return ParseCall(local);
                else
                    return new IdentifierNode(Pop());
            }
            if (TopType == IntegerLiteral || TopType == RealLiteral || TopType == StringLiteral)
                return new LiteralNode(Pop());
            if (TopType == TrueKeyword || TopType == FalseKeyword)
                return new LiteralNode(Pop());

            Error();
            return null;
        }

        protected CallNode ParseCall(SymbolTable local)
        {
            var id = new IdentifierNode(Pop());
            var name = id.Token.Text;
            int patterncount = -1;
            var paramsQueue = new Queue<ExprNode>();

            if (local != null && local.ContainsKey(name))
                patterncount = local.Get(name).ParameterCount;
            else
                Error();

            for (int i = 0; i < patterncount; i++)
            {
                var p = ParseAtom(local, false);
                // TODO: Type check
                paramsQueue.Enqueue(p);
            }

            return new CallNode(id, paramsQueue.ToArray());
        }

        private ExprNode ParseBinary(SymbolTable local,
            Func<SymbolTable, ExprNode> lexpr_func,
            params SyntaxTokenType[] types)
        {
            var lexpr = lexpr_func(local);
            if (IsEOF) return lexpr;
            if (types.Contains(TopType))
            {
                var op = Pop();
                var rexpr = ParseBinary(local, lexpr_func, types);
                return new BinaryExprNode(lexpr, rexpr, op);
            }
            return lexpr;
        }
    }
}
