namespace RingEngine.Core.Storage;

using System;
using System.Collections.Generic;
using MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class StorageEngine
{
    /// <summary>
    /// 下一条执行的代码块index
    /// </summary>
    public int PC;

    /// <summary>
    /// 运行时是否暂停
    /// </summary>
    public bool Paused;

    /// <summary>
    /// 其它数据，为了使python可调用需要限制类型
    /// </summary>
    public Dictionary<string, string> Data = [];

    public string this[string key]
    {
        get => Data[key];
        set => Data[key] = value;
    }

    [IgnoreMember]
    public List<Snapshot> History = [];

    public Snapshot LoadHistory(int step)
    {
        if (step < 1 || step > History.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(step));
        }
        var ret = History[^step];
        History.RemoveRange(History.Count - step, step);
        return ret;
    }

    public string Serialize()
    {
        return MessagePackSerializer.SerializeToJson(this);
    }

    public static StorageEngine Deserialize(string json)
    {
        return MessagePackSerializer.Deserialize<StorageEngine>(
            MessagePackSerializer.ConvertFromJson(json)
        );
    }
}
