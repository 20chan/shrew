namespace shrew.Syntax
{
    public enum SyntaxTokenType
    {
        None = 0,
        IntegerLiteral,
        RealLiteral,

        KnownKeywordStart,

        PlusToken = KnownKeywordStart,
        MinusToken,
        AsteriskToken,
        SlashToken,
        TrueKeyword,
        FalseKeyword,

        KnownKeywordEnd = FalseKeyword,
        Identifier,
        EOF,
    }
}
