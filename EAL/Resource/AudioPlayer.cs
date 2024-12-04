namespace RingEngine.EAL.Resource;

using Godot;

public class AudioPlayer
{
    public AudioStreamPlayer Player;
    public AudioStream Stream
    {
        get => Player.Stream;
        set => Player.Stream = value;
    }

    /// <summary>
    /// 线性音量值（0-1）
    /// </summary>
    public double Volume
    {
        get => Mathf.DbToLinear(Player.VolumeDb);
        set => Player.VolumeDb = (float)Mathf.LinearToDb(value);
    }

    public void Play(AudioStream stream)
    {
        Player.Stream = stream;
        Player.StreamPaused = false;
        Player.Play();
    }

    public void Play()
    {
        if (Player.StreamPaused)
        {
            Player.StreamPaused = false;
        }
        else
        {
            Player.Play();
        }
    }

    public void Pause() => Player.StreamPaused = true;

    public void Stop() => Player.Stop();

    public AudioPlayer(AudioStreamPlayer player)
    {
        Player = player;
    }

    public static implicit operator AudioStreamPlayer(AudioPlayer player) => player.Player;
}
