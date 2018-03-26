namespace shrew.Syntax
{
    public class SyntaxNode
    {

    }

    public class AssignNode : SyntaxNode
    {
        public IdentifierNode Left;
        public ExprNode Right;

        public AssignNode(IdentifierNode left, ExprNode right)
        {
            Left = left;
            Right = right;
        }
    }

    public class ExprNode : SyntaxNode
    {

    }

    public class BinaryExprNode : ExprNode
    {
        public ExprNode Left, Right;
        public SyntaxToken Operator;

        public BinaryExprNode(ExprNode left, ExprNode right, SyntaxToken op)
        {
            Left = left;
            Right = right;
            Operator = op;
        }
    }

    public class TokenNode : ExprNode
    {
        public SyntaxToken Token;

        public TokenNode(SyntaxToken identifier)
        {
            Token = identifier;
        }
    }

    public class LiteralNode : TokenNode
    {
        public LiteralNode(SyntaxToken token) : base(token)
        {

        }
    }

    public class IdentifierNode : TokenNode
    {
        public IdentifierNode(SyntaxToken token) : base(token)
        {

        }
    }
}
