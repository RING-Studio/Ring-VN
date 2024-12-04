namespace Test.Core.General;

using Mock.General;
using RingEngine.Core.General;

[TestClass]
public class TestLogger
{
    [TestInitialize]
    public void TestInitialize()
    {
        Logger.Output = new MockOutput();
    }

    [TestMethod]
    public void TestLog()
    {
        Logger.Log("Test");
        Assert.AreEqual("[Log] Test\n", ((MockOutput)Logger.Output).Output);
    }

    [TestMethod]
    public void TestWarning()
    {
        Logger.Warning("Test");
        Assert.AreEqual("[Warning] Test\n", ((MockOutput)Logger.Output).Output);
    }

    [TestMethod]
    public void TestError()
    {
        Logger.Error("Test");
        Assert.AreEqual("[Error] Test\n", ((MockOutput)Logger.Output).Output);
    }
}
