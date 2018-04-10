using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using shrew.Parsing;
using shrew.Syntax;

using static shrew.Syntax.SyntaxTokenType;
using SynFact = shrew.Syntax.SyntaxFactory;

namespace shrew.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestCategory("Parser"), TestMethod]
        public void TestParseLiteralIdentifier()
        {
            AssertParse("1", LIT("1", 1));
            AssertParse("true", KEY(TrueKeyword));
            AssertParse("false", KEY(FalseKeyword));
            AssertParse("\"hello\"", LIT("hello"));
            AssertParse("abcd", new SymbolTypes { "abcd" }, CALL("abcd"));
        }

        [TestCategory("Parser"), TestMethod]
        public void TestParseExpression()
        {
            AssertParse("-1",
                UN(
                    LIT("1", 1),
                    MinusToken));

            AssertParse("!~!-a", new SymbolTypes { "a" },
                UN(
                    UN(
                        UN(
                            UN(
                                CALL("a"),
                                MinusToken),
                            ExclamationToken),
                        TildeToken),
                    ExclamationToken));

            AssertParse("1+1",
                BIN(
                    LIT("1", 1),
                    LIT("1", 1),
                PlusToken));

            AssertParse("1--1",
                BIN(
                    LIT("1", 1),
                    UN(
                        LIT("1", 1),
                        MinusToken),
                    MinusToken));

            AssertParse("1+2*3",
                BIN(
                    LIT("1", 1),
                    BIN(
                        LIT("2", 2),
                        LIT("3", 3),
                        AsteriskToken),
                PlusToken));

            AssertParse("(1+2)*3",
                BIN(
                    BIN(
                        LIT("1", 1),
                        LIT("2", 2),
                        PlusToken),
                    LIT("3", 3),
                AsteriskToken));

            AssertParse("1 | 2 ^ 3 & 4 >> 5 + 6 * -1",
                BIN(
                    LIT("1", 1),
                    BIN(
                        LIT("2", 2),
                        BIN(
                            LIT("3", 3),
                            BIN(
                                LIT("4", 4),
                                BIN(
                                    LIT("5", 5),
                                    BIN(
                                        LIT("6", 6),
                                        UN(
                                            LIT("1", 1),
                                            MinusToken),
                                        AsteriskToken),
                                    PlusToken),
                                RShiftToken),
                            AmperToken),
                        CaretToken),
                    VBarToken));

            AssertParse("1 == (2 + 1) >= 3",
                BIN(
                    LIT("1", 1),
                    BIN(
                        BIN(
                            LIT("2", 2),
                            LIT("1", 1),
                            PlusToken),
                        LIT("3", 3),
                        GreaterEqualToken),
                    EqualToken));
        }

        [TestCategory("Parser"), TestMethod]
        public void TestParseAssignment()
        {
            AssertParse("a = 1",
                ASGN(
                    "a",
                    LIT("1", 1)));

            AssertParse("abcd = b * 1.5", new SymbolTypes { "b" },
                ASGN(
                    "abcd",
                    BIN(
                        CALL("b"),
                        LIT("1.5", 1.5f),
                    AsteriskToken)));
        }

        [TestCategory("Parser"), TestMethod]
        public void TestParseStatements()
        {
            AssertParse("a = 1 b = 2",
                ASGN(
                    "a",
                    LIT("1", 1)),
                ASGN(
                    "b",
                    LIT("2", 2)));

            AssertParse("a = 1 1 + 2",
                ASGN(
                    "a",
                    LIT("1", 1)),
                BIN(
                    LIT("1", 1),
                    LIT("2", 2),
                PlusToken));
        }

        [TestCategory("Parser"), TestMethod]
        public void TestCallParams()
        {
            var builtins = new SymbolTypes
            {
                { "add",  typeof(int), typeof(int) },
                { "a" },
                { "b" },
            };

            AssertParse("add 1 2", builtins,
                CALL(
                    "add",
                    LIT("1", 1),
                    LIT("2", 2)));

            AssertParse("add (a + b) 3", builtins,
                CALL(
                    "add",
                    BIN(
                        CALL("a"),
                        CALL("b"),
                        PlusToken),
                    LIT("3", 3)));

            AssertParse("add3 a b c = a + b + c\nres = add3 b 10 a", builtins,
                ASGN(
                    new[] {
                        ID("add3"),
                        ID("a"),
                        ID("b"),
                        ID("c")
                    },
                    BIN(
                        CALL("a"),
                        BIN(
                            CALL("b"),
                            CALL("c"),
                            PlusToken),
                        PlusToken)),
                ASGN(
                    "res",
                    CALL(
                        "add3",
                        ID("b"),
                        LIT("10", 10),
                        ID("a"))));
        }

        private void AssertParse(string code, params SyntaxNode[] nodes)
            => AssertParse(code, null, nodes);

        private void AssertParse(string code, SymbolTypes builtins, params SyntaxNode[] nodes)
        {
            var actual = Parser.Parse(code, builtins);
            new StmtsNode(nodes).Equals(actual);
            Assert.AreEqual(actual, new StmtsNode(nodes));
        }

        private static IdentifierNode ID(string name)
            => new IdentifierNode(SynFact.Identifier(name));

        private static LiteralNode LIT(string text, int value)
            => new LiteralNode(SynFact.Literal(text, value));

        private static LiteralNode LIT(string text, float value)
            => new LiteralNode(SynFact.Literal(text, value));

        private static LiteralNode LIT(string value)
            => new LiteralNode(SynFact.Literal(value));

        private static LiteralNode KEY(SyntaxTokenType type)
            => new LiteralNode(SynFact.KeywordToken(type));

        private static UnaryExprNode UN(ExprNode right, SyntaxTokenType type)
            => new UnaryExprNode(right, SynFact.KeywordToken(type));

        private static BinaryExprNode BIN(ExprNode left, ExprNode right, SyntaxTokenType type)
            => new BinaryExprNode(left, right, SynFact.KeywordToken(type));

        private static AssignNode ASGN(string leftOne, ExprNode right)
            => ASGN(new[] { ID(leftOne) }, right);

        private static AssignNode ASGN(IdentifierNode[] left, ExprNode right)
            => new AssignNode(left, right);

        private static CallNode CALL(string id, params ExprNode[] parameters)
            => new CallNode(SynFact.Identifier(id), parameters);
    }
}
