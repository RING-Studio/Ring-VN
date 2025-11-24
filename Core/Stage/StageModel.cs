namespace RingEngine.Core.Stage;

using System.Collections.Generic;
using System.Numerics;

public class StageModel
{
    public string BackgroundId { get; set; }

    // key: characterId
    public Dictionary<string, CharacterState> Characters { get; } = [];

    public List<OverlayState> Overlays { get; } = [];

    public CameraState Camera { get; set; } = new();
}

public class CharacterState
{
    public string CharacterId { get; init; }
    public string VariantId { get; set; }
    public StagePoint Point { get; set; }
    public bool Visible { get; set; }
    public int ZOrder { get; set; }
}

public enum StagePoint
{
    NearLeft, NearCenter, NearRight,
    MidLeft, MidCenter, MidRight,
    FarLeft, FarCenter, FarRight
}

public class CameraState
{
    // 1.0 = 正常；>1 拉近；<1 拉远
    public float Zoom { get; set; }

    public Vector2 Offset { get; set; }
}

public class OverlayState
{
    public string OverlayId { get; init; }
    public float Opacity { get; set; }  // 0~1
    public int ZOrder { get; set; }
}
