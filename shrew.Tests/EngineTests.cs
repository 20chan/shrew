using shrew;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace shrew.Tests
{
    [TestClass]
    public class EngineTests
    {
        [TestCategory("Engine"), TestCategory("Execute"), TestMethod]
        public void TestExecution()
        {
            Assert.AreEqual(0, Engine.Execute("main = 0"));
            Assert.AreEqual(2, Engine.Execute("main = 1 + 1"));
            Assert.AreEqual(30, Engine.Execute("main = 3 * 10"));
            Assert.AreEqual(30, Engine.Execute("main = 3 * 10"));
        }

        [TestCategory("Engine"), TestCategory("Variable"), TestMethod]
        public void TestVariable()
        {
            Assert.AreEqual(1, new Engine("a = 1")["a"].DynamicInvoke());
            Assert.AreEqual(2, new Engine("a = 1 + 1")["a"].DynamicInvoke());
            Assert.AreEqual(3, new Engine("a = 1\nb = a + 2")["b"].DynamicInvoke());
        }

        [TestCategory("Engine"), TestCategory("Expression"), TestMethod]
        public void TestIntegerExpr()
        {
            Assert.AreEqual(0, Engine.EvaluateExpr("0"));
            Assert.AreEqual(1, Engine.EvaluateExpr("1"));
            Assert.AreEqual(12345, Engine.EvaluateExpr("12345"));
            Assert.AreEqual(0, Engine.EvaluateExpr("1 - 1"));
            Assert.AreEqual(10, Engine.EvaluateExpr("1 + 2 + 3 + 4"));
        }

        [TestCategory("Engine"), TestCategory("Expression"), TestMethod]
        public void TestRealExpr()
        {
            Assert.AreEqual(0f, Engine.EvaluateExpr("0.0"));
            Assert.AreEqual(0.001f, Engine.EvaluateExpr("0.001"));
            Assert.AreEqual(3.14f, Engine.EvaluateExpr("3.14"));
            Assert.AreEqual(10f, Engine.EvaluateExpr("10.0000"));
            Assert.AreEqual(6f, Engine.EvaluateExpr("1 + 2 + 3.0"));
            Assert.AreEqual(6f, Engine.EvaluateExpr<float>("0.1 + 0.2 - 0.3"), 0.0001f);
        }

        [TestCategory("Engine"), TestCategory("Expression"), TestMethod]
        public void TestParenExpr()
        {
            Assert.AreEqual(0, Engine.EvaluateExpr("(0)"));
            Assert.AreEqual(1, Engine.EvaluateExpr("(((1)))"));
            Assert.AreEqual(3, Engine.EvaluateExpr("(1 + 2)"));
            Assert.AreEqual(3, Engine.EvaluateExpr("((1) + 2)"));
            Assert.AreEqual(9, Engine.EvaluateExpr("(1 + 2) * 3"));
            Assert.AreEqual(0, Engine.EvaluateExpr("3 - (2 + 1)"));
        }
    }
}
