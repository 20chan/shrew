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

        public void Initialize()
        {
            _index = 0;
        }

        public SyntaxToken Lex()
        {
            if (IsEOF)
                Error();
            switch (Peek)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                case '>':
                case '<':
                case '=':
                case '!':
                case '~':
                case '|':
                case '&':
                case '^':
                case '(':
                case ')':
                    return LexKeyword();
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    Pop();
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
                    return LexNumeric();
                case '"':
                    return LexString();
                default:
                    if (char.IsLetterOrDigit(Peek) || Peek == '_')
                        return LexIdentifier();
                    else
                        Error();
                    return null;
            }
        }

        private SyntaxToken LexKeyword()
        {
            var top = Pop();
            switch (top)
            {
                case '+':
                    if (!IsEOF && Peek == '+')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.ConcatToken);
                    }
                    else return SyntaxFactory.KeywordToken(SyntaxTokenType.PlusToken);
                case '>':
                    if (!IsEOF && Peek == '=')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.GreaterEqualToken);
                    }
                    else if (!IsEOF && Peek == '>')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.RShiftToken);
                    }
                    else return SyntaxFactory.KeywordToken(SyntaxTokenType.GreaterToken);
                case '<':
                    if (!IsEOF && Peek == '=')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.LessEqualToken);
                    }
                    else if (!IsEOF && Peek == '<')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.LShiftToken);
                    }
                    else return SyntaxFactory.KeywordToken(SyntaxTokenType.LessToken);
                case '|':
                    if (!IsEOF && Peek == '|')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.DoubleVBarToken);
                    }
                    else return SyntaxFactory.KeywordToken(SyntaxTokenType.VBarToken);
                case '&':
                    if (!IsEOF && Peek == '&')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.DoubleAmperToken);
                    }
                    else return SyntaxFactory.KeywordToken(SyntaxTokenType.AmperToken);
                case '=':
                    if (!IsEOF && Peek == '=')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.EqualToken);
                    }
                    else return SyntaxFactory.KeywordToken(SyntaxTokenType.AssignToken);
                case '!':
                    if (!IsEOF && Peek == '=')
                    {
                        Pop();
                        return SyntaxFactory.KeywordToken(SyntaxTokenType.NotEqualToken);
                    }
                    else return SyntaxFactory.KeywordToken(SyntaxTokenType.ExclamationToken);
                case '-':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.MinusToken);
                case '*':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.AsteriskToken);
                case '/':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.SlashToken);
                case '%':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.PercentToken);
                case '~':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.TildeToken);
                case '^':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.CaretToken);
                case '(':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.LParenToken);
                case ')':
                    return SyntaxFactory.KeywordToken(SyntaxTokenType.RParenToken);
                default:
                    throw new LexerException();
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

        private SyntaxToken LexString()
        {
            Pop();
            var result = new System.Text.StringBuilder();
            while (!IsEOF && Peek != '"')
            {
                var cur = Pop();
                if (cur == '\\')
                {
                    var unescaped = Pop();
                    if (unescaped == 'n')
                        result.Append('\n');
                    else if (unescaped == 'r')
                        result.Append('\r');
                    else if (unescaped == 't')
                        result.Append('\t');
                    else if (unescaped == '\\')
                        result.Append('\\');
                    else if (unescaped == '"')
                        result.Append('"');
                    else if (unescaped == '0')
                        result.Append('\0');
                    else
                        throw new LexerException();
                }
                else if (cur == '\r' || cur == '\n')
                    throw new LexerException();
                else
                    result.Append(cur);
            }
            Pop();
            return SyntaxFactory.Literal(result.ToString());
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
            else if (text == "_")
                return SyntaxFactory.KeywordToken(SyntaxTokenType.WildcardKeyword);
            else
                return SyntaxFactory.Identifier(text);
        }
    }
}
