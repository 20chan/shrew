namespace shrew.Syntax
{
    public partial class SyntaxToken
    {
        public SyntaxTokenType TokenType { get; }

        public int FullWidth { get; set; }

        internal SyntaxToken(SyntaxTokenType type)
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
                    case SyntaxTokenType.TrueKeyword:
                        return true;
                    case SyntaxTokenType.FalseKeyword:
                        return false;
                    default:
                        return Text;
                }
            }
        }

        internal static SyntaxToken WithValue<T>(SyntaxTokenType type, string text, T value)
            => new SyntaxTokenWithValue<T>(type, text, value);

        public override bool Equals(object obj)
        {
            var tok = obj as SyntaxToken;
            if (tok == null)
                return false;
            if (tok.TokenType != this.TokenType)
                return false;
            if (tok.Text != this.Text)
                return false;
            if (tok.Value.GetType() != Value.GetType())
                return false;
            if (tok.Value.GetType().IsValueType
                ? !tok.Value.Equals(Value)
                : tok.Value != Value)
                return false;
            return true;
        }
    }
}
