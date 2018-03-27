using System.Linq;
using System.Collections.Generic;
using shrew.Lexing;
using shrew.Syntax;

namespace shrew.Parsing
{
    public class Parser
    {
        private readonly string _code;
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

        public Parser(string code)
        {
            _code = code;
            _tokens = Lexer.Lex(code).ToArray();
        }

        protected void Error()
        {
            throw new ParserException();
        }

        public static SyntaxNode Parse(string code)
        {
            return new Parser(code).ParseStmts();
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
            var lexpr = ParseExpr();
            if (IsEOF) return lexpr;

            if (Peek().TokenType == SyntaxTokenType.AssignToken)
            {
                Eat(SyntaxTokenType.AssignToken);
                if (!(lexpr is IdentifierNode))
                    Error();
                var rexpr = ParseExpr();
                return new AssignNode(lexpr as IdentifierNode, rexpr);
            }

            return lexpr;
        }

        protected ExprNode ParseExpr()
        {
            var lexpr = ParseTerm();
            if (IsEOF) return lexpr;
            if (TopType == SyntaxTokenType.PlusToken || TopType == SyntaxTokenType.MinusToken)
            {
                var op = Pop();
                var rexpr = ParseExpr();
                return new BinaryExprNode(lexpr, rexpr, op);
            }
            return lexpr;
        }

        protected ExprNode ParseTerm()
        {
            var lexpr = ParseFactor();
            if (IsEOF) return lexpr;
            if (TopType == SyntaxTokenType.AsteriskToken || TopType == SyntaxTokenType.SlashToken)
            {
                var op = Pop();
                var rexpr = ParseTerm();
                return new BinaryExprNode(lexpr, rexpr, op);
            }
            return lexpr;
        }

        protected ExprNode ParseFactor()
        {
            if (TopType == SyntaxTokenType.LParenToken)
            {
                Eat(SyntaxTokenType.LParenToken);
                var expr = ParseExpr();
                Eat(SyntaxTokenType.RParenToken);

                return expr;
            }
            if (TopType == SyntaxTokenType.Identifier)
                return new IdentifierNode(Pop());
            if (TopType == SyntaxTokenType.IntegerLiteral || TopType == SyntaxTokenType.RealLiteral)
                return new LiteralNode(Pop());
            if (TopType == SyntaxTokenType.TrueKeyword || TopType == SyntaxTokenType.FalseKeyword)
                return new LiteralNode(Pop());

            Error();
            return null;
        }
    }
}
