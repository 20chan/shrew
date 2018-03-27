using System;
using shrew.Lexing;
using shrew.Syntax;

namespace shrew.Parsing
{
    public class Parser
    {
        private Lexer _lexer;
        public Parser(Lexer lexer)
        {
            _lexer = lexer;
        }

        public static SyntaxNode Parse(string code)
        {
            return new Parser(new Lexer(code)).Parse();
        }

        protected SyntaxNode Parse()
        {
            throw new NotImplementedException();
        }
    }
}
