using System.Linq;
using shrew.Lexing;
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
            AssertLex("0.11", new Tok[] { TokFact.Literal("0.11", 0.11f) });
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
        public void TestWhitespacesLexing()
        {
            AssertLex("  1", new Tok[] { TokFact.Literal("1", 1) });
            AssertLex("1  ", new Tok[] { TokFact.Literal("1", 1) });
            AssertLex("  1  ", new Tok[] { TokFact.Literal("1", 1) });
            AssertLex("  1   +  1  ", new Tok[]
            {
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1)
            });
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

        [TestCategory("Lexer"), TestMethod]
        public void TestIdentifier()
        {
            AssertLex("a", new Tok[] { TokFact.Identifier("a") });
            AssertLex("main", new Tok[] { TokFact.Identifier("main") });
            AssertLex("foo12", new Tok[] { TokFact.Identifier("foo12") });
            AssertLex("pi_is_314_sure", new Tok[] { TokFact.Identifier("pi_is_314_sure") });
            AssertLex("a + 1", new Tok[]
            {
                TokFact.Identifier("a"),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1),
            });
            AssertLex("1 + a + b*3", new Tok[]
            {
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Identifier("a"),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Identifier("b"),
                TokFact.KeywordToken(Typ.AsteriskToken),
                TokFact.Literal("3", 3),
            });
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestError()
        {
            AssertLexError("1.16.1");
        }
        
        private void AssertLex(string code, Tok[] lexed)
        {
            var actual = Lexer.Lex(code);
            var l = lexed[0];
            var r = actual.First();
            l.Equals(r);
            CollectionAssert.AreEqual(lexed, actual.ToArray());
        }

        private void AssertLexError(string code)
        {
            Assert.ThrowsException<LexerException>(() => Lexer.Lex(code).ToArray());
        }
    }
}
