using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        }

        [TestCategory("Engine"), TestMethod]
        public void TestIntegerExpr()
        {
            Assert.AreEqual(0, Engine.EvaluateExpr("0"));
            Assert.AreEqual(1, Engine.EvaluateExpr("1"));
            Assert.AreEqual(12345, Engine.EvaluateExpr("12345"));
            Assert.AreEqual(0, Engine.EvaluateExpr("1 - 1"));
            Assert.AreEqual(10, Engine.EvaluateExpr("1 + 2 + 3 + 4"));
        }

        [TestCategory("Engine"), TestMethod]
        public void TestRealExpr()
        {
            Assert.AreEqual(0f, Engine.EvaluateExpr("0.0"));
            Assert.AreEqual(0.001f, Engine.EvaluateExpr("0.001"));
            Assert.AreEqual(3.14f, Engine.EvaluateExpr("3.14"));
            Assert.AreEqual(10f, Engine.EvaluateExpr("10.0000"));
            Assert.AreEqual(6f, Engine.EvaluateExpr("1 + 2 + 3.0"));
            Assert.AreEqual(0f, Engine.EvaluateExpr<float>("0.1 + 0.2 - 0.3"), 0.0001f);
        }

        [TestCategory("Engine"), TestMethod]
        public void TestParenExpr()
        {
            Assert.AreEqual(0, Engine.EvaluateExpr("(0)"));
            Assert.AreEqual(1, Engine.EvaluateExpr("(((1)))"));
            Assert.AreEqual(3, Engine.EvaluateExpr("(1 + 2)"));
            Assert.AreEqual(3, Engine.EvaluateExpr("((1) + 2)"));
            Assert.AreEqual(9, Engine.EvaluateExpr("(1 + 2) * 3"));
            Assert.AreEqual(0, Engine.EvaluateExpr("3 - (2 + 1)"));
            Assert.AreEqual(40, Engine.EvaluateExpr("(5 - 1) * (30 / (1 + 2))"));
        }
    }
}
