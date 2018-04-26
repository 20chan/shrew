namespace shrew.Syntax
{
    public static class SyntaxTokenTypeExtension
    {
        public static bool IsLiteral(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.IntegerLiteral:
                case SyntaxTokenType.RealLiteral:
                case SyntaxTokenType.StringLiteral:
                case SyntaxTokenType.TrueKeyword:
                case SyntaxTokenType.FalseKeyword:
                case SyntaxTokenType.WildcardKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAtomBeginning(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.LParenToken:
                case SyntaxTokenType.Identifier:
                case SyntaxTokenType.IntegerLiteral:
                case SyntaxTokenType.RealLiteral:
                case SyntaxTokenType.StringLiteral:
                case SyntaxTokenType.TrueKeyword:
                case SyntaxTokenType.FalseKeyword:
                    return true;
                default:
                    return false;
            }
        }
    }
}
