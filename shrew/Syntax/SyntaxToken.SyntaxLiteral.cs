namespace shrew.Syntax
{
    public partial class SyntaxToken
    {
        public class SyntaxTokenWithValue<T> : SyntaxToken
        {
            protected readonly string _text;
            protected readonly T _value;
            internal SyntaxTokenWithValue(SyntaxTokenType type, string text, T value) : base(type, text.Length)
            {
                _text = text;
                _value = value;
            }

            public override string Text => _text;

            public override object Value => _value;
        }
    }
}
