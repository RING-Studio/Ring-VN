namespace Test.Core.General;

using RingEngine.Core.General;

[TestClass]
public class TestPathSTD
{
    [TestMethod]
    [DataRow("res://folder/test.tscn")]
    [DataRow("./folder/test.tscn")]
    [DataRow("folder/test.tscn")]
    public void TestFrom(string rawpath)
    {
        var path = PathSTD.From(rawpath);
        Assert.AreEqual("folder/test.tscn", path.RelativePath);
        Assert.AreEqual("folder", path.Directory);
        Assert.AreEqual("test.tscn", path.FileName);
        Assert.AreEqual("test", path.FileNameWithoutExtension);
        Assert.AreEqual(".tscn", path.Extension);
        Assert.AreEqual("res://folder/test.tscn", path.GodotPath);
        Assert.AreEqual("res://folder", path.GodotDirectory);
    }

    [TestMethod]
    [DataRow("res://folder/", "test.tscn")]
    [DataRow("res://folder/", "./test.tscn")]
    [DataRow("folder/", "test.tscn")]
    [DataRow("folder/", "./test.tscn")]
    [DataRow("./folder", "test.tscn")]
    [DataRow("./folder", "./test.tscn")]
    public void TestAdd(string rawpath, string addpath)
    {
        var path = PathSTD.From(rawpath) + addpath;
        Assert.AreEqual("folder/test.tscn", path.RelativePath);
        Assert.AreEqual("folder", path.Directory);
        Assert.AreEqual("test.tscn", path.FileName);
        Assert.AreEqual("test", path.FileNameWithoutExtension);
        Assert.AreEqual(".tscn", path.Extension);
        Assert.AreEqual("res://folder/test.tscn", path.GodotPath);
        Assert.AreEqual("res://folder", path.GodotDirectory);
    }
}
