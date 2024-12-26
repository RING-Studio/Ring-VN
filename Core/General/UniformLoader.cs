namespace RingEngine.Core.General;

using Godot;

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

    public static PackedScene LoadScene(PathSTD path) => Load<PackedScene>(path);

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
}
