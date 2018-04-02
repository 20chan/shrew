using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using shrew.Parsing;
using shrew.Syntax;

using SynFact = shrew.Syntax.SyntaxFactory;

namespace shrew.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestCategory("Parser"), TestMethod]
        public void TestParseLiteralIdentifier()
        {
            AssertParse("1", new LiteralNode(SynFact.Literal("1", 1)));
            AssertParse("true", new LiteralNode(SynFact.KeywordToken(SyntaxTokenType.TrueKeyword)));
            AssertParse("false", new LiteralNode(SynFact.KeywordToken(SyntaxTokenType.FalseKeyword)));
            AssertParse("abcd", new IdentifierNode(SynFact.Identifier("abcd")));
        }

        [TestCategory("Parser"), TestMethod]
        public void TestParseExpression()
        {
            AssertParse("1+1",
                new BinaryExprNode(
                    new LiteralNode(SynFact.Literal("1", 1)),
                    new LiteralNode(SynFact.Literal("1", 1)),
                SynFact.KeywordToken(SyntaxTokenType.PlusToken)));

            AssertParse("1+2*3",
                new BinaryExprNode(
                    new LiteralNode(SynFact.Literal("1", 1)),
                    new BinaryExprNode(
                        new LiteralNode(SynFact.Literal("2", 2)),
                        new LiteralNode(SynFact.Literal("3", 3)),
                        SynFact.KeywordToken(SyntaxTokenType.AsteriskToken)),
                SynFact.KeywordToken(SyntaxTokenType.PlusToken)));

            AssertParse("(1+2)*3",
                new BinaryExprNode(
                    new BinaryExprNode(
                        new LiteralNode(SynFact.Literal("1", 1)),
                        new LiteralNode(SynFact.Literal("2", 2)),
                        SynFact.KeywordToken(SyntaxTokenType.PlusToken)),
                    new LiteralNode(SynFact.Literal("3", 3)),
                SynFact.KeywordToken(SyntaxTokenType.AsteriskToken)));
        }

        [TestCategory("Parser"), TestMethod]
        public void TestParseAssignment()
        {
            AssertParse("a = 1",
                new AssignNode(
                    new IdentifierNode(SynFact.Identifier("a")),
                    new LiteralNode(SynFact.Literal("1", 1))));

            AssertParse("abcd = b * 1.5",
                new AssignNode(
                    new IdentifierNode(SynFact.Identifier("abcd")),
                    new BinaryExprNode(
                        new IdentifierNode(SynFact.Identifier("b")),
                        new LiteralNode(SynFact.Literal("1.5", 1.5f)),
                    SynFact.KeywordToken(SyntaxTokenType.AsteriskToken))));
        }

        [TestCategory("Parser"), TestMethod]
        public void TestParseStatements()
        {
            AssertParse("a = 1 b = 2",
                new AssignNode(
                    new IdentifierNode(SynFact.Identifier("a")),
                    new LiteralNode(SynFact.Literal("1", 1))),
                new AssignNode(
                    new IdentifierNode(SynFact.Identifier("b")),
                    new LiteralNode(SynFact.Literal("2", 2))));

            AssertParse("a = 1 1 + 2",
                new AssignNode(
                    new IdentifierNode(SynFact.Identifier("a")),
                    new LiteralNode(SynFact.Literal("1", 1))),
                new BinaryExprNode(
                    new LiteralNode(SynFact.Literal("1", 1)),
                    new LiteralNode(SynFact.Literal("2", 2)),
                SynFact.KeywordToken(SyntaxTokenType.PlusToken)));
        }

        private void AssertParse(string code, params SyntaxNode[] nodes)
        {
            var actual = Parser.Parse(code);
            new StmtsNode(nodes).Equals(actual);
            Assert.AreEqual(actual, new StmtsNode(nodes));
        }
    }
}
