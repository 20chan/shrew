namespace shrew.Syntax
{
    public partial class SyntaxToken
    {
        public class SyntaxIdentifier : SyntaxToken
        {
            protected readonly string _name;
            internal SyntaxIdentifier(SyntaxTokenType type, string name)
                // Text가 아직 설정되지 않은 상태에서 base 생성자에서는 Text.Length 를 사용해서
                : base(type, name.Length)
            {
                _name = name;
            }

            public override string Text => _name;

            public override object Value => _name;
        }
    }
}
