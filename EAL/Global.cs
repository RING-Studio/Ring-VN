using System.Collections.Generic;
using Godot;

public delegate void EachFrameCallback();

public partial class Global : Node
{
    public List<EachFrameCallback> CallEachFrame = [];

    public override void _Process(double delta)
    {
        foreach (var callback in CallEachFrame)
        {
            callback();
        }
    }
}
