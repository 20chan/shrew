using System;
using System.Linq;
using System.Collections.Generic;
using shrew.Syntax;

namespace shrew.Lexing
{
    public class Lexer
    {
        public string Code => _code;
        protected string _code;
        private int _index;

        public Lexer(string code)
        {
            _code = code;
        }

        private char Peek => _code[_index];
        private char Pop() => _code[_index++];
        private bool IsEOF => _code.Length == _index || IsWhitespacesOnlyLeft();

        public static IEnumerable<SyntaxToken> Lex(string code)
        {
            var lexer = new Lexer(code);
            while (!lexer.IsEOF)
                yield return lexer.Lex();
        }

        private bool IsWhitespacesOnlyLeft()
        {
            if (_code.Length == _index) return true;
            for (int idx=  _index; idx < _code.Length; idx++)
            {
                if (!char.IsWhiteSpace(_code, idx))
                    return false;
            }
            return true;
        }

        private void Error()
        {
            throw new LexerException();
        }

        public SyntaxToken Lex()
        {
            if (IsEOF)
                Error();
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
                case '=':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.AssignToken);
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    return Lex();
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
                    _index--;
                    if (char.IsLetterOrDigit(Peek) || Peek == '_')
                        return LexIdentifier();
                    else
                        Error();
                    return null;
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

            while (!IsEOF)
            {
                if (!char.IsNumber(Peek))
                    break;
                Pop();
            }
            int len = _index - startIdx;
            // TODO: Span<char> 으로 대체되어야 함.
            var text = _code.Substring(startIdx, len);
            return SyntaxFactory.Literal(text, Convert.ToSingle(text));
        }

        private SyntaxToken LexIdentifier()
        {
            int start = _index;
            while (!IsEOF)
            {
                if (Peek != '_' && !char.IsLetterOrDigit(Peek))
                    break;

                Pop();
            }
            int len = _index - start;
            var text = _code.Substring(start, len);
            if (text == "true")
                return SyntaxFactory.KeywordToken(SyntaxTokenType.TrueKeyword);
            else if (text == "false")
                return SyntaxFactory.KeywordToken(SyntaxTokenType.FalseKeyword);
            else
                return SyntaxFactory.Identifier(text);
        }
    }
}
