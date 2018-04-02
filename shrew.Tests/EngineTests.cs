﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using static shrew.Engine;

namespace shrew.Tests
{
    [TestClass]
    public class EngineTests
    {
        [TestCategory("Engine"), TestMethod]
        public void TestExecution()
        {
            Assert.AreEqual(0, Engine.Execute("main = 0"));
            Assert.AreEqual(2, Engine.Execute("main = 1 + 1"));
            Assert.AreEqual(30, Engine.Execute("main = 3 * 10"));
        }

        [TestCategory("Engine"), TestMethod]
        public void TestVariable()
        {
            var engine = new Engine("a = 1");
            engine.ExecuteAllStmts();
            Assert.AreEqual(1, engine["a"].DynamicInvoke());

            engine = new Engine("a = 1 + 1");
            engine.ExecuteAllStmts();
            Assert.AreEqual(2, engine["a"].DynamicInvoke());

            engine = new Engine("a = 1\nb = a + 2");
            engine.ExecuteAllStmts();
            Assert.AreEqual(3, engine["b"].DynamicInvoke());

            Assert.AreEqual(3, EvaluateExpr("a = 1\na+2"));
        }

        [TestCategory("Engine"), TestMethod]
        public void TestIntegerExpr()
        {
            Assert.AreEqual(0, EvaluateExpr("0"));
            Assert.AreEqual(1, EvaluateExpr("1"));
            Assert.AreEqual(12345, EvaluateExpr("12345"));
            Assert.AreEqual(0, EvaluateExpr("1 - 1"));
            Assert.AreEqual(10, EvaluateExpr("1 + 2 + 3 + 4"));
        }

        [TestCategory("Engine"), TestMethod]
        public void TestRealExpr()
        {
            Assert.AreEqual(0f, EvaluateExpr("0.0"));
            Assert.AreEqual(0.001f, EvaluateExpr("0.001"));
            Assert.AreEqual(3.14f, EvaluateExpr("3.14"));
            Assert.AreEqual(10f, EvaluateExpr("10.0000"));
            Assert.AreEqual(6f, EvaluateExpr("1 + 2 + 3.0"));
            Assert.AreEqual(0f, EvaluateExpr<float>("0.1 + 0.2 - 0.3"), 0.0001f);
        }

        [TestCategory("Engine"), TestMethod]
        public void TestParenExpr()
        {
            Assert.AreEqual(0, EvaluateExpr("(0)"));
            Assert.AreEqual(1, EvaluateExpr("(((1)))"));
            Assert.AreEqual(3, EvaluateExpr("(1 + 2)"));
            Assert.AreEqual(3, EvaluateExpr("((1) + 2)"));
            Assert.AreEqual(9, EvaluateExpr("(1 + 2) * 3"));
            Assert.AreEqual(0, EvaluateExpr("3 - (2 + 1)"));
            Assert.AreEqual(40, EvaluateExpr("(5 - 1) * (30 / (1 + 2))"));
        }

        [TestCategory("Engine"), TestMethod]
        public void TestParameter()
        {

        }
    }
}
