using System;
using System.Linq;
using System.Collections.Generic;
using shrew.Syntax;

namespace shrew.Lexing
{
    public class Lexer
    {
        protected string _code;
        private int _index;

        public Lexer(string code)
        {
            _code = code;
        }

        private char Peek => _code[_index];
        private char Pop() => _code[_index++];
        private bool IsEOF => _code.Length == _index;

        public static IEnumerable<SyntaxToken> Lex(string code)
        {
            var lexer = new Lexer(code);
            while (!lexer.IsEOF)
                yield return lexer.Lex();
        }

        private void Error()
        {
            throw new NotImplementedException();
        }

        public SyntaxToken Lex()
        {
            switch (Pop())
            {
                case '+':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.PlusToken);
                case '-':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.MinusToken);
                case '*':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.AsteriskToken);
                case '/':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.SlashToken);

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    _index--;
                    return LexNumeric();
                default:
                    throw new NotImplementedException();
            }
        }

        private SyntaxToken LexNumeric()
        {
            int start = _index;
            while (!IsEOF)
            {
                if (Peek == '.')
                    return LexRealFromDot(start);
                if (!char.IsNumber(Peek))
                    break;

                Pop();
            }
            int len = _index - start;
            // TODO: Span<char> 으로 대체되어야 함.
            var text = _code.Substring(start, len);
            return SyntaxFactory.Literal(text, Convert.ToInt32(text));
        }

        private SyntaxToken LexRealFromDot(int startIdx)
        {
            Pop(); // .
            if (IsEOF)
                Error();
            if (!char.IsNumber(Pop()))
                Error();

            while (!IsEOF && char.IsNumber(Pop())) ;
            int len = _index - startIdx;
            // TODO: Span<char> 으로 대체되어야 함.
            var text = _code.Substring(startIdx, len);
            return SyntaxFactory.Literal(text, Convert.ToSingle(text));
        }
    }
}
