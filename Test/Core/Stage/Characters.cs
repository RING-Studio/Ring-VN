namespace Test.Core.Stage;

using RingEngine.Core.Stage.Export;
using Test.Mock.Resource;

[TestClass]
public class TestCharacters
{
    [TestMethod]
    public void TestAdd()
    {
        var characters = new Characters();
        var sprite = new MockSprite();
        characters.Add("test", sprite);
        Assert.IsTrue(characters.Has("test"));
        Assert.AreEqual(characters["test"], sprite);
    }
    [TestMethod]
    public void TestRemove()
    {
        var characters = new Characters();
        var sprite = new MockSprite();
        characters.Add("test", sprite);
        var removed = characters.Remove("test");
        Assert.IsFalse(characters.Has("test"));
        Assert.AreEqual(removed, sprite);
    }
    [TestMethod]
    public void TestRename()
    {
        var characters = new Characters();
        var sprite = new MockSprite()
        {
            Name = "test"
        };
        characters.Add("test", sprite);
        characters.Rename("test", "test2");
        Assert.IsFalse(characters.Has("test"));
        Assert.IsTrue(characters.Has("test2"));
        Assert.AreEqual(characters["test2"], sprite);
    }
}
