using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TokFact = shrew.Syntax.SyntaxFactory;
using Tok = shrew.Syntax.SyntaxToken;
using Typ = shrew.Syntax.SyntaxTokenType;

namespace shrew.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestCategory("Lexer"), TestMethod]
        public void TestNumericLexing()
        {
            AssertLex("0", new Tok[] { TokFact.Literal("0", 0) });
            AssertLex("012", new Tok[] { TokFact.Literal("012", 12) });
            AssertLex("0.11", new Tok[] { TokFact.Literal("0.11", 0.11f)});
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestKeywordLexing()
        {
            AssertLex("+", new Tok[] { TokFact.KeywordToken(Typ.PlusToken) });
            AssertLex("-", new Tok[] { TokFact.KeywordToken(Typ.MinusToken) });
            AssertLex("*", new Tok[] { TokFact.KeywordToken(Typ.AsteriskToken) });
            AssertLex("/", new Tok[] { TokFact.KeywordToken(Typ.SlashToken) });
            AssertLex("true", new Tok[] { TokFact.KeywordToken(Typ.TrueKeyword) });
            AssertLex("false", new Tok[] { TokFact.KeywordToken(Typ.FalseKeyword) });
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestNumericExpressionLexing()
        {
            AssertLex("1+1", new Tok[]
            {
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1)
            });
            AssertLex("314 *    42.05", new Tok[]
            {
                TokFact.Literal("314", 314),
                TokFact.KeywordToken(Typ.AsteriskToken),
                TokFact.Literal("42.05", 42.05f)
            });
            AssertLex("1+1*1/1-1", new Tok[]
            {
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.AsteriskToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.SlashToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.MinusToken),
                TokFact.Literal("1", 1)
            });
        }

        private void AssertLex(string code, Tok[] lexed)
        {
            var actual = Lexing.Lexer.Lex(code);
            CollectionAssert.AreEqual(lexed, actual.ToArray());
        }
    }
}
