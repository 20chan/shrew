namespace shrew.Syntax
{
    public class SyntaxNode
    {

    }

    public class StmtsNode : SyntaxNode
    {
        public readonly SyntaxNode[] Nodes;

        public StmtsNode(params SyntaxNode[] nodes)
        {
            Nodes = nodes;
        }

        public override bool Equals(object obj)
        {
            var node = obj as StmtsNode;
            if (node == null)
                return false;
            if (Nodes.Length != node.Nodes.Length)
                return false;
            for (int i = 0; i < Nodes.Length; i++)
            {
                if (!(Nodes[i].Equals(node.Nodes[i])))
                    return false;
            }

            return true;
        }
    }

    public class AssignNode : SyntaxNode
    {
        public readonly TokenNode[] Left;
        public ExprNode Right;

        public AssignNode(TokenNode[] left, ExprNode right)
        {
            Left = left;
            Right = right;
        }

        public override bool Equals(object obj)
        {
            var node = obj as AssignNode;
            if (node == null)
                return false;
            if (Left.Length != node.Left.Length)
                return false;
            for(int i = 0; i < Left.Length; i++)
            {
                if (!Left[i].Equals(node.Left[i]))
                    return false;
            }
            if (!(Right.Equals(node.Right)))
                return false;
            return true;
        }
    }

    public class ExprNode : SyntaxNode
    {

    }

    public class CallNode : ExprNode
    {
        public IdentifierNode Function;
        public readonly ExprNode[] Parameters;

        public CallNode(IdentifierNode function, params ExprNode[] parameters)
        {
            Function = function;
            Parameters = parameters;
        }

        public CallNode(SyntaxToken identifier, params ExprNode[] parameters)
        {
            Function = new IdentifierNode(identifier);
            Parameters = parameters;
        }

        public override bool Equals(object obj)
        {
            var node = obj as CallNode;
            if (node == null)
                return false;
            if (!Function.Equals(node.Function))
                return false;
            if (Parameters.Length != node.Parameters.Length)
                return false;
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (!Parameters[i].Equals(node.Parameters[i]))
                    return false;
            }
            return true;
        }
    }

    public class UnaryExprNode : ExprNode
    {
        public ExprNode Right;
        public SyntaxToken Operator;

        public UnaryExprNode(ExprNode right, SyntaxToken op)
        {
            Right = right;
            Operator = op;
        }

        public override bool Equals(object obj)
        {
            var node = obj as UnaryExprNode;
            if (node == null)
                return false;
            if (!(Right.Equals(node.Right)))
                return false;
            if (!(Operator.Equals(node.Operator)))
                return false;
            return true;
        }
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

        public override bool Equals(object obj)
        {
            var node = obj as BinaryExprNode;
            if (node == null)
                return false;
            if (!(Left.Equals(node.Left)))
                return false;
            if (!(Right.Equals(node.Right)))
                return false;
            if (!(Operator.Equals(node.Operator)))
                return false;
            return true;
        }
    }

    public class TokenNode : ExprNode
    {
        public SyntaxToken Token;

        public TokenNode(SyntaxToken identifier)
        {
            Token = identifier;
        }

        public override bool Equals(object obj)
        {
            var node = obj as TokenNode;
            if (node == null)
                return false;
            if (GetType() != node.GetType())
                return false;
            if (node == null)
                return false;
            if (!(Token.Equals(node.Token)))
                return false;
            return true;
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
