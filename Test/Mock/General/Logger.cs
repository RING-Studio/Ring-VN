namespace Test.Mock.General;

using RingEngine.Core.General;

public class MockOutput : IOutput
{
    public string Output = "";

    public void Print(string message) => Output += message + "\n";
}
