namespace RingEngine.Core.ResourceConfig;

using System;
using System.Collections.Generic;
using System.Numerics;
using RingEngine.Core.Stage;

public sealed class JsonStageResourceProvider : IStageResourceProvider
{
    private readonly StageResourceConfig _config;
    private readonly Dictionary<StagePoint, Vector2> _pointMap;
    private readonly Dictionary<string, BackgroundItemConfig> _backgroundMap;
    private readonly Dictionary<string, CharacterItemConfig> _characterMap;
    private readonly Dictionary<string, OverlayItemConfig> _overlayMap;

    public JsonStageResourceProvider(StageResourceConfig config)
    {
        _config = config;

        _pointMap = BuildPointMap(config.Stage);
        _backgroundMap = BuildBackgroundMap(config.Backgrounds);
        _characterMap = BuildCharacterMap(config.Characters);
        _overlayMap = BuildOverlayMap(config.Overlays);
    }

    public Vector2 GetPoint(StagePoint point)
    {
        if (_pointMap.TryGetValue(point, out var v))
        {
            return v;
        }

        throw new KeyNotFoundException($"Stage point '{point}' not configured.");
    }

    public float GetDepthScale(StagePoint point)
    {
        var depth = GetDepthBucket(point);
        return depth switch
        {
            DepthBucket.Near => _config.Stage.DepthScales.Near,
            DepthBucket.Mid => _config.Stage.DepthScales.Mid,
            DepthBucket.Far => _config.Stage.DepthScales.Far,
            _ => 1.0f
        };
    }

    public BackgroundLayout GetBackgroundLayout(string backgroundId)
    {
        if (!_backgroundMap.TryGetValue(backgroundId, out var item))
        {
            throw new KeyNotFoundException($"Background '{backgroundId}' not found.");
        }

        var defaults = _config.Backgrounds.Defaults;

        var fitMode = item.FitMode ?? defaults.FitMode;
        var anchor = (item.Anchor ?? defaults.Anchor).ToVector2();
        var scale = item.Scale ?? 1.0f;

        return new BackgroundLayout(
            Id: item.Id,
            File: item.File,
            FitMode: fitMode,
            Anchor: anchor,
            Scale: scale
        );
    }

    public CharacterLayout GetCharacterLayout(string characterId, string variantId, StagePoint point)
    {
        if (!_characterMap.TryGetValue(characterId, out var item))
        {
            throw new KeyNotFoundException($"Character '{characterId}' not found.");
        }

        var defaults = _config.Characters.Defaults;

        // 1) 基础参数：角色默认或全局默认
        var baseScale = item.BaseScale ?? defaults.BaseScale;
        var yOffset = item.YOffset ?? defaults.YOffset;

        // 2) variant 覆写
        var filePath = ResolveCharacterVariantFile(item, variantId);

        if (item.Variants.TryGetValue(variantId, out var ov))
        {
            if (ov.BaseScale is not null)
            {
                baseScale = ov.BaseScale.Value;
            }

            if (ov.YOffset is not null)
            {
                yOffset = ov.YOffset.Value;
            }
        }

        // 3) 点位 + depth scale
        var pointPos = GetPoint(point);
        var depthScale = GetDepthScale(point);

        return new CharacterLayout(
            CharacterId: characterId,
            VariantId: variantId,
            File: filePath,
            BaseScale: baseScale,
            YOffset: yOffset,
            PointPosition: pointPos,
            DepthScale: depthScale
        );
    }

    public OverlayLayout GetOverlayLayout(string overlayId)
    {
        if (!_overlayMap.TryGetValue(overlayId, out var item))
        {
            throw new KeyNotFoundException($"Overlay '{overlayId}' not found.");
        }

        return new OverlayLayout(
            Id: item.Id,
            File: item.File,
            BlendMode: item.BlendMode
        );
    }

    // --------- helpers ---------

    private static Dictionary<StagePoint, Vector2> BuildPointMap(StageLayoutConfig cfg)
    {
        var dict = new Dictionary<StagePoint, Vector2>();

        foreach (StagePoint sp in Enum.GetValues(typeof(StagePoint)))
        {
            var key = sp.ToString(); // "NearLeft" 等
            if (cfg.Points.TryGetValue(key, out var p))
            {
                dict[sp] = p.ToVector2();
            }
        }

        return dict;
    }

    private static Dictionary<string, BackgroundItemConfig> BuildBackgroundMap(BackgroundsConfig cfg)
    {
        var dict = new Dictionary<string, BackgroundItemConfig>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in cfg.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                continue;
            }

            dict[item.Id] = item;
        }
        return dict;
    }

    private static Dictionary<string, CharacterItemConfig> BuildCharacterMap(CharactersConfig cfg)
    {
        var dict = new Dictionary<string, CharacterItemConfig>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in cfg.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                continue;
            }

            dict[item.Id] = item;
        }
        return dict;
    }

    private static Dictionary<string, OverlayItemConfig> BuildOverlayMap(OverlaysConfig cfg)
    {
        var dict = new Dictionary<string, OverlayItemConfig>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in cfg.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                continue;
            }

            dict[item.Id] = item;
        }
        return dict;
    }

    private static DepthBucket GetDepthBucket(StagePoint point) =>
        point switch
        {
            StagePoint.NearLeft or StagePoint.NearCenter or StagePoint.NearRight => DepthBucket.Near,
            StagePoint.MidLeft or StagePoint.MidCenter or StagePoint.MidRight => DepthBucket.Mid,
            StagePoint.FarLeft or StagePoint.FarCenter or StagePoint.FarRight => DepthBucket.Far,
            _ => DepthBucket.Mid
        };

    private static string ResolveCharacterVariantFile(CharacterItemConfig item, string variantId)
    {
        // 1) 如果 variant override 提供了 file，就用它
        if (item.Variants.TryGetValue(variantId, out var ov) && !string.IsNullOrWhiteSpace(ov.File))
        {
            return System.IO.Path.Combine(item.Folder, ov.File);
        }

        // 2) 否则用 auto_variants 规则
        if (item.AutoVariants is { Enabled: true } av)
        {
            // 简单替换 {variant}
            var fileName = av.Pattern.Replace("{variant}", variantId);
            return System.IO.Path.Combine(item.Folder, fileName);
        }

        // 3) fallback：直接用 variantId 当文件名
        return System.IO.Path.Combine(item.Folder, variantId + ".png");
    }

    private enum DepthBucket
    {
        Near,
        Mid,
        Far
    }
}

