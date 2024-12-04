namespace RingEngine.EAL.SceneTree;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using RingEngine.Core.Script;
using RingEngine.Core.UI.Export;

public static class UI
{
    public static void DisplayBranch(IEnumerable<Branch.BranchOption> options)
    {
        var branchRoot = SceneTreeProxy.BranchScene.Instantiate<Control>();
        branchRoot.Name = "Branch";
        branchRoot.Set("Callback", Callable.From<int, string>(UIModule.BranchCallBack));
        branchRoot.Set("Texts", new Array<string>(options.Select((option) => option.Text)));
        SceneTreeProxy.UIRoot.AddChild(branchRoot);
    }
}
