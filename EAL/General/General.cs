namespace RingEngine.EAL.General;

using Godot;
using RingEngine.Core.General;

public class GDOutput : IOutput
{
    public void Print(string message) => GD.Print(message);
}
