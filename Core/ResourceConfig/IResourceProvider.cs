namespace RingEngine.Core.ResourceConfig;

using System.Numerics;
using RingEngine.Core.Stage;

/// <summary>
/// 提供 StageModel 所需的布局和资源信息，
/// VN Core 只依赖这个接口，不关心 JSON 与文件系统细节。
/// </summary>
public interface IStageResourceProvider
{
    // 点位坐标
    Vector2 GetPoint(StagePoint point);

    // 根据 StagePoint 返回推荐缩放系数（Near/Mid/Far）
    float GetDepthScale(StagePoint point);

    // 背景布局信息
    BackgroundLayout GetBackgroundLayout(string backgroundId);

    // 角色立绘布局信息
    CharacterLayout GetCharacterLayout(string characterId, string variantId, StagePoint point);

    // Overlay 资源信息
    OverlayLayout GetOverlayLayout(string overlayId);
}

public sealed record BackgroundLayout(
    string Id,
    string File,
    string FitMode,
    Vector2 Anchor,
    float Scale
);

public sealed record CharacterLayout(
    string CharacterId,
    string VariantId,
    string File,
    float BaseScale,
    float YOffset,
    Vector2 PointPosition,
    float DepthScale
);

public sealed record OverlayLayout(
    string Id,
    string File,
    string BlendMode
);

