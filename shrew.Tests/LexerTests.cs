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
            AssertLex("0", TokFact.Literal("0", 0));
            AssertLex("012", TokFact.Literal("012", 12));
            AssertLex("0.11", TokFact.Literal("0.11", 0.11f));
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestStringLexing()
        {
            AssertLex("\"abc\"", TokFact.Literal("abc"));
            AssertLex("\"a  bc  \" ", TokFact.Literal("a  bc  "));
            AssertLex("\"\\\\\"", TokFact.Literal("\\"));
            AssertLex("\"slash\\\"\"", TokFact.Literal("slash\""));
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestWildcardLexing()
        {
            AssertLex("_", TokFact.KeywordToken(Typ.WildcardKeyword));
            AssertLex("__", TokFact.Identifier("__"));
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestKeywordLexing()
        {
            AssertLex("+", TokFact.KeywordToken(Typ.PlusToken));
            AssertLex("-", TokFact.KeywordToken(Typ.MinusToken));
            AssertLex("*", TokFact.KeywordToken(Typ.AsteriskToken));
            AssertLex("/", TokFact.KeywordToken(Typ.SlashToken));
            AssertLex("%", TokFact.KeywordToken(Typ.PercentToken));
            AssertLex("++", TokFact.KeywordToken(Typ.ConcatToken));
            AssertLex(">", TokFact.KeywordToken(Typ.GreaterToken));
            AssertLex("<", TokFact.KeywordToken(Typ.LessToken));
            AssertLex(">=", TokFact.KeywordToken(Typ.GreaterEqualToken));
            AssertLex("<=", TokFact.KeywordToken(Typ.LessEqualToken));
            AssertLex("==", TokFact.KeywordToken(Typ.EqualToken));
            AssertLex("!=", TokFact.KeywordToken(Typ.NotEqualToken));
            AssertLex("<<", TokFact.KeywordToken(Typ.LShiftToken));
            AssertLex(">>", TokFact.KeywordToken(Typ.RShiftToken));
            AssertLex("!", TokFact.KeywordToken(Typ.ExclamationToken));
            AssertLex("~", TokFact.KeywordToken(Typ.TildeToken));
            AssertLex("|", TokFact.KeywordToken(Typ.VBarToken));
            AssertLex("&", TokFact.KeywordToken(Typ.AmperToken));
            AssertLex("||", TokFact.KeywordToken(Typ.DoubleVBarToken));
            AssertLex("&&", TokFact.KeywordToken(Typ.DoubleAmperToken));
            AssertLex("^", TokFact.KeywordToken(Typ.CaretToken));
            AssertLex("=", TokFact.KeywordToken(Typ.AssignToken));
            AssertLex("true", TokFact.KeywordToken(Typ.TrueKeyword));
            AssertLex("false", TokFact.KeywordToken(Typ.FalseKeyword));
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestWhitespacesLexing()
        {
            AssertLex("  1", TokFact.Literal("1", 1));
            AssertLex("1  ", TokFact.Literal("1", 1));
            AssertLex("  1  ", TokFact.Literal("1", 1));
            AssertLex("  1   +  1  ",
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1)
            );
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestNumericExpressionLexing()
        {
            AssertLex("1+1",
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1)
            );
            AssertLex("314 *    42.05",
                TokFact.Literal("314", 314),
                TokFact.KeywordToken(Typ.AsteriskToken),
                TokFact.Literal("42.05", 42.05f)
            );
            AssertLex("1+1*1/1-1",
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.AsteriskToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.SlashToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.MinusToken),
                TokFact.Literal("1", 1)
            );
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestIdentifier()
        {
            AssertLex("a", TokFact.Identifier("a"));
            AssertLex("main", TokFact.Identifier("main"));
            AssertLex("foo12", TokFact.Identifier("foo12"));
            AssertLex("pi_is_314_sure", TokFact.Identifier("pi_is_314_sure"));
            AssertLex("a + 1",
                TokFact.Identifier("a"),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1)
            );
            AssertLex("1 + a + b*3",
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Identifier("a"),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Identifier("b"),
                TokFact.KeywordToken(Typ.AsteriskToken),
                TokFact.Literal("3", 3)
            );
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestError()
        {
            AssertLexError("1.16.1");
        }

        [TestCategory("Lexer"), TestMethod]
        public void TestAssign()
        {
            AssertLex("a = 1 + 1",
                TokFact.Identifier("a"),
                TokFact.KeywordToken(Typ.AssignToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1)
            );
            AssertLex(@"a = 1 + 1
b = a + 1",
                TokFact.Identifier("a"),
                TokFact.KeywordToken(Typ.AssignToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1),
                TokFact.KeywordToken(Typ.NewLine),
                TokFact.Identifier("b"),
                TokFact.KeywordToken(Typ.AssignToken),
                TokFact.Identifier("a"),
                TokFact.KeywordToken(Typ.PlusToken),
                TokFact.Literal("1", 1)
            );
        }
        
        private void AssertLex(string code, params Tok[] lexed)
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
