namespace shrew.Syntax
{
    public partial class SyntaxToken
    {
        public SyntaxTokenType TokenType { get; }

        public int FullWidth { get; set; }

        protected SyntaxToken(SyntaxTokenType type)
        {
            TokenType = type;
            FullWidth = Text.Length;
        }

        protected SyntaxToken(SyntaxTokenType type, int width)
        {
            TokenType = type;
            FullWidth = width;
        }

        public virtual string Text
            => SyntaxFactory.GetText(TokenType);

        public virtual object Value
        {
            get
            {
                switch (TokenType)
                {
                    case SyntaxTokenType.TrueLiteral:
                        return true;
                    case SyntaxTokenType.FalseLiteral:
                        return false;
                    default:
                        return Text;
                }
            }
        }
    }
}
