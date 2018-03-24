using System;

namespace shrew.Syntax
{
    public static class SyntaxFactory
    {
        private static SyntaxTokenType FirstToken = SyntaxTokenType.KnownKeywordStart;
        private static SyntaxTokenType LastToken = SyntaxTokenType.KnownKeywordEnd;
        private static SyntaxToken[] _tokens;

        static SyntaxFactory()
        {
            _tokens = new SyntaxToken[LastToken - FirstToken + 1];

            for (var typ = FirstToken; typ <= LastToken; typ++)
            {
                _tokens[(int)typ] = new SyntaxToken(typ);
            }
        }

        public static string GetText(SyntaxTokenType type)
        {
            switch(type)
            {
                case SyntaxTokenType.PlusToken:
                    return "+";
                case SyntaxTokenType.MinusToken:
                    return "-";
                case SyntaxTokenType.AsteriskToken:
                    return "*";
                case SyntaxTokenType.SlashToken:
                    return "/";
                case SyntaxTokenType.TrueKeyword:
                    return "true";
                case SyntaxTokenType.FalseKeyword:
                    return "false";
                default:
                    return string.Empty;
            }
        }

        public static SyntaxToken KeywordToken(SyntaxTokenType type)
            => _tokens[type - FirstToken];

        public static SyntaxToken Literal(string text, int value)
            => SyntaxToken.WithValue(SyntaxTokenType.IntegerLiteral, text, value);

        public static SyntaxToken Literal(string text, float value)
            => SyntaxToken.WithValue(SyntaxTokenType.RealLiteral, text, value);
    }
}
