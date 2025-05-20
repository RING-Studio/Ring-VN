namespace RingEngine.Core.General;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Godot;
using MessagePack;
using RingEngine.Core.Storage;

public static class UniformLoader
{
    public static T Load<T>(PathSTD path)
        where T : GodotObject
    {
        if (typeof(Resource).IsAssignableFrom(typeof(T)))
        {
            return GD.Load(path.GodotPath) as T;
        }
        else if (typeof(Node).IsAssignableFrom(typeof(T)))
        {
            return GD.Load<PackedScene>(path.GodotPath).Instantiate<T>();
        }
        else
        {
            Logger.Error($"Resource.Load<{typeof(T)}> is not supported.");
            return null;
        }
    }

    public static void Save(Resource resource, PathSTD path)
    {
        var error = DirAccess.MakeDirRecursiveAbsolute(path.GodotDirectory);
        if (error != Error.Ok)
        {
            Logger.Error($"Failed to create directory: {path.GodotDirectory}, ErrorCode: {error}");
            return;
        }
        ResourceSaver.Save(resource, path.GodotPath);
    }

    public static string LoadText(PathSTD path)
    {
        var file = FileAccess.Open(path.GodotPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            Logger.Error($"Failed to open file: {path}, ErrorCode: {FileAccess.GetOpenError()}");
            return null;
        }
        return file.GetAsText();
    }

    public static void SaveText(string content, PathSTD path)
    {
        var error = DirAccess.MakeDirRecursiveAbsolute(path.GodotDirectory);
        if (error != Error.Ok)
        {
            Logger.Error($"Failed to create directory: {path.GodotDirectory}, ErrorCode: {error}");
            return;
        }
        var file = FileAccess.Open(path.GodotPath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            Logger.Error($"Failed to open file: {path}, ErrorCode: {FileAccess.GetOpenError()}");
            return;
        }
        file.StoreString(content);
        file.Close();
    }

    public static BGConfig LoadBGConfig(PathSTD path)
    {
        var bg_name = path.FileNameWithoutExtension;
        var config = LoadConfig(path);
        if (config == null)
        {
            return null;
        }
        dynamic my_config = config[bg_name];
        return new BGConfig()
        {
            Size = new Vector2I(my_config["size"][0], my_config["size"][0]),
            Anchor = new Vector2I(my_config["anchor"][0], my_config["anchor"][1]),
        };
    }

    public static CharacterConfig LoadCharacterConfig(PathSTD path)
    {
        var character_name = path.FileNameWithoutExtension;
        var config = LoadConfig(path);
        if (config == null)
        {
            return null;
        }
        dynamic my_config = config[character_name];
        return new CharacterConfig()
        {
            Size = new Vector2I(my_config["size"][0], my_config["size"][1]),
            YBase = my_config["y_base"],
            AvatarSize = new Vector2I(my_config["avatar_size"][0], my_config["avatar_size"][1]),
        };
    }

    public static Dictionary<string, object> LoadConfig(PathSTD path)
    {
        // 从当前目录下加载配置文件
        var config_path = PathSTD.From(path.Directory) + "config.json";
        if (FileAccess.FileExists(config_path.GodotPath))
        {
            using var file = FileAccess.Open(config_path.GodotPath, FileAccess.ModeFlags.Read);
            if (file == null)
            {
                Logger.Error(
                    $"Failed to open config file: {config_path}, ErrorCode: {FileAccess.GetOpenError()}"
                );
                return null;
            }
            return MessagePackSerializer.Deserialize<Dictionary<string, object>>(
                MessagePackSerializer.ConvertFromJson(file.GetAsText())
            );
        }
        else
        {
            Logger.Error($"Config file not found: {config_path}");
            return null;
        }
    }
}
