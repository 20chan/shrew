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
        private SyntaxToken[] _tokens;
        private int _index;

        private bool IsEOF
            => _index == _tokens.Length || IsNewLineOnlyLeft();

        private SyntaxToken Peek(int n = 0)
            => _tokens[_index + n];

        private SyntaxTokenType TopType
            => Peek().TokenType;

        private SyntaxToken Pop()
            => _tokens[_index++];

        private bool IsNewLineOnlyLeft()
        {
            for (int i = _index; i < _tokens.Length; i++)
            {
                if (_tokens[i].TokenType != NewLine)
                    return false;
            }
            return true;
        }

        private void Eat(SyntaxTokenType tok)
        {
            if (Pop().TokenType != tok)
                Error();
        }

        public Parser(string code)
        {
            _code = code;
            _tokens = Lexer.Lex(code).ToArray();
        }

        protected void Error()
        {
            throw new ParserException();
        }

        public static StmtsNode Parse(string code)
            => new Parser(code).ParseStmts();
        
        protected StmtsNode ParseStmts()
        {
            var nodes = new List<SyntaxNode>();

            while (!IsEOF)
                nodes.Add(Parse());

            return new StmtsNode(nodes.ToArray());
        }

        protected SyntaxNode Parse()
        {
            if (Peek().TokenType == NewLine)
            {
                Pop();
                return Parse();
            }
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
                return ParseExpr();
            }

            Eat(AssignToken);
            var ids = tokQueue.ToArray();
            var rexpr = ParseExpr();
            return new AssignNode(ids, rexpr);
        }

        protected ExprNode ParseExpr()
        {
            return ParseDoubleOr();
        }

        protected ExprNode ParseDoubleOr()
            => ParseBinary(ParseDoubleAnd, DoubleVBarToken);

        protected ExprNode ParseDoubleAnd()
            => ParseBinary(ParseOr, DoubleAmperToken);

        protected ExprNode ParseOr()
            => ParseBinary(ParseXor, VBarToken);

        protected ExprNode ParseXor()
            => ParseBinary(ParseAnd, CaretToken);

        protected ExprNode ParseAnd()
            => ParseBinary(ParseCompare, AmperToken);

        protected ExprNode ParseCompare()
            => ParseBinary(ParseShift,
                GreaterToken, LessToken, GreaterEqualToken, LessEqualToken, EqualToken, NotEqualToken);

        protected ExprNode ParseShift()
            => ParseBinary(ParseArith, LShiftToken, RShiftToken);

        protected ExprNode ParseArith()
            => ParseBinary(ParseTerm, PlusToken, MinusToken, ConcatToken);

        protected ExprNode ParseTerm()
            => ParseBinary(ParseFactor, AsteriskToken, SlashToken, PercentToken);

        protected ExprNode ParseFactor()
        {
            if (TopType == MinusToken
                || TopType == ExclamationToken
                || TopType == TildeToken)
            {
                var op = Pop();
                var expr = ParseFactor();
                return new UnaryExprNode(expr, op);
            }
            return ParseAtom(true);
        }

        protected ExprNode ParseAtom(bool callAllowed)
        {
            if (TopType == LParenToken)
            {
                Eat(LParenToken);
                var expr = ParseExpr();
                Eat(RParenToken);

                return expr;
            }
            if (TopType == Identifier)
            {
                if (callAllowed)
                    return ParseCall();
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

        protected CallNode ParseCall()
        {
            var id = new IdentifierNode(Pop());
            var name = id.Token.Text;
            var paramsQueue = new Queue<ExprNode>();
            
            while (!IsEOF && Peek().TokenType.IsAtomBeginning())
            {
                ExprNode p = ParseAtom(false);
                if (p is IdentifierNode idnode)
                {
                    p = new CallNode(idnode.Token);
                }
                // TODO: Type check
                paramsQueue.Enqueue(p);
            }

            return new CallNode(id, paramsQueue.ToArray());
        }

        private ExprNode ParseBinary(Func<ExprNode> lexpr_func,
            params SyntaxTokenType[] types)
        {
            var lexpr = lexpr_func();
            if (IsEOF) return lexpr;
            if (types.Contains(TopType))
            {
                var op = Pop();
                var rexpr = ParseBinary(lexpr_func, types);
                return new BinaryExprNode(lexpr, rexpr, op);
            }
            return lexpr;
        }
    }
}
