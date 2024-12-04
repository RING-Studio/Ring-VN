namespace Test.Core.General;

using RingEngine.Core.General;
using Test.Mock.General;

[TestClass]
public class TestAssertWrapper
{
    [TestInitialize]
    public void TestInitialize()
    {
        Logger.Output = new MockOutput();
    }

    [TestMethod]
    public void TestAssert()
    {
        AssertWrapper.Assert(true);
        Assert.ThrowsException<AssertException>(() => AssertWrapper.Assert(false));
    }

    [TestMethod]
    public void TestUnreachable()
    {
        Assert.ThrowsException<AssertException>(() => AssertWrapper.Unreachable());
    }
}
