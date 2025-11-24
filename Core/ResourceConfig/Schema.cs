namespace RingEngine.Core.ResourceConfig;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 顶层配置对象，对应整个 config.json。
/// </summary>
public sealed class StageResourceConfig
{
    [JsonPropertyName("stage")]
    public StageLayoutConfig Stage { get; init; } = new();

    [JsonPropertyName("backgrounds")]
    public BackgroundsConfig Backgrounds { get; init; } = new();

    [JsonPropertyName("characters")]
    public CharactersConfig Characters { get; init; } = new();

    [JsonPropertyName("overlays")]
    public OverlaysConfig Overlays { get; init; } = new();

    public static StageResourceConfig FromJson(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var config = JsonSerializer.Deserialize<StageResourceConfig>(json, options);
        if (config is null)
        {
            throw new InvalidOperationException("Failed to deserialize StageResourceConfig.");
        }
        return config;
    }
}

// --------- stage 部分 ---------

public sealed class StageLayoutConfig
{
    [JsonPropertyName("points")]
    public Dictionary<string, PointConfig> Points { get; init; } = new();

    [JsonPropertyName("depth_scales")]
    public DepthScaleConfig DepthScales { get; init; } = new();
}

public sealed class PointConfig
{
    [JsonPropertyName("x")]
    public float X { get; init; }

    [JsonPropertyName("y")]
    public float Y { get; init; }

    public Vector2 ToVector2() => new(X, Y);
}

public sealed class DepthScaleConfig
{
    [JsonPropertyName("Near")]
    public float Near { get; init; } = 1.15f;

    [JsonPropertyName("Mid")]
    public float Mid { get; init; } = 1.0f;

    [JsonPropertyName("Far")]
    public float Far { get; init; } = 0.85f;
}

// --------- backgrounds 部分 ---------

public sealed class BackgroundsConfig
{
    [JsonPropertyName("defaults")]
    public BackgroundDefaultsConfig Defaults { get; init; } = new();

    [JsonPropertyName("items")]
    public List<BackgroundItemConfig> Items { get; init; } = new();
}

public sealed class BackgroundDefaultsConfig
{
    [JsonPropertyName("fit_mode")]
    public string FitMode { get; init; } = "cover"; // cover / contain / none

    [JsonPropertyName("anchor")]
    public PointConfig Anchor { get; init; } = new() { X = 0.5f, Y = 0.5f };
}

public sealed class BackgroundItemConfig
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("file")]
    public string File { get; init; } = "";

    [JsonPropertyName("fit_mode")]
    public string? FitMode { get; init; }

    [JsonPropertyName("anchor")]
    public PointConfig? Anchor { get; init; }

    [JsonPropertyName("scale")]
    public float? Scale { get; init; }
}

// --------- characters 部分 ---------

public sealed class CharactersConfig
{
    [JsonPropertyName("defaults")]
    public CharacterDefaultsConfig Defaults { get; init; } = new();

    [JsonPropertyName("items")]
    public List<CharacterItemConfig> Items { get; init; } = new();
}

public sealed class CharacterDefaultsConfig
{
    [JsonPropertyName("base_scale")]
    public float BaseScale { get; init; } = 1.0f;

    [JsonPropertyName("y_offset")]
    public float YOffset { get; init; } = 0.0f;
}

public sealed class CharacterItemConfig
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("folder")]
    public string Folder { get; init; } = "";

    [JsonPropertyName("base_scale")]
    public float? BaseScale { get; init; }

    // 调整角色垂直偏移，水平默认居中，如果有问题直接修改素材
    [JsonPropertyName("y_offset")]
    public float? YOffset { get; init; }

    [JsonPropertyName("auto_variants")]
    public AutoVariantConfig? AutoVariants { get; init; }

    // key: variantId
    [JsonPropertyName("variants")]
    public Dictionary<string, CharacterVariantOverride> Variants { get; init; } = new();
}

public sealed class AutoVariantConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }

    // pattern like "{variant}.png", "body_{variant}.png"
    [JsonPropertyName("pattern")]
    public string Pattern { get; init; } = "{variant}.png";

    [JsonPropertyName("exclude")]
    public List<string> Exclude { get; init; } = new();
}

public sealed class CharacterVariantOverride
{
    [JsonPropertyName("file")]
    public string? File { get; init; }

    [JsonPropertyName("base_scale")]
    public float? BaseScale { get; init; }

    [JsonPropertyName("y_offset")]
    public float? YOffset { get; init; }
}

// --------- overlays 部分 ---------

public sealed class OverlaysConfig
{
    [JsonPropertyName("items")]
    public List<OverlayItemConfig> Items { get; init; } = new();
}

public sealed class OverlayItemConfig
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("file")]
    public string File { get; init; } = "";

    [JsonPropertyName("blend_mode")]
    public string BlendMode { get; init; } = "normal";
}

