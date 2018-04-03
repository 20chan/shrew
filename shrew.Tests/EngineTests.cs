using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static shrew.Engine;

namespace shrew.Tests
{
    [TestClass]
    public class EngineTests
    {
        [TestCategory("Engine"), TestMethod]
        public void TestExecution()
        {
            Assert.AreEqual(0, Execute("main = 0"));
            Assert.AreEqual(2, Execute("main = 1 + 1"));
            Assert.AreEqual(30, Execute("main = 3 * 10"));
        }

        [TestCategory("Engine"), TestMethod]
        public void TestVariable()
        {
            var engine = new Engine();
            engine.ExecuteCode("a = 1");
            Assert.AreEqual(1, engine["a"].DynamicInvoke());

            engine = new Engine();
            engine.ExecuteCode("a = 1 + 1");
            Assert.AreEqual(2, engine["a"].DynamicInvoke());

            engine = new Engine();
            engine.ExecuteCode("a = 1\nb = a + 2");
            Assert.AreEqual(3, engine["b"].DynamicInvoke());

            engine = new Engine();
            engine.ExecuteCode("a=1");

            Assert.AreEqual(3, engine.EvaluateCode("a+2"));
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
            var builtins = new SymbolTable(args: new Dictionary<string, Delegate>()
            {
                { "add", (Func<int, int, int>)((a, b) => a + b) },
            });

            var engine = new Engine(builtins);
            Assert.AreEqual(3, engine.EvaluateCode("add 1 2"));

            engine.ExecuteOrEvaluate("add3 a b c = add (add a b) c");
            Assert.AreEqual(6, engine.EvaluateCode("add3 1 2 3"));
        }
    }
}
