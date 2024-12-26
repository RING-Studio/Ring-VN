namespace RingEngine.Core.Audio;

using RingEngine.EAL.Resource;
using RingEngine.EAL.SceneTree;

public class AudioModule
{
    public AudioPlayer BGM;
    public AudioPlayer SE;
    public AudioPlayer Voice;

    public AudioModule()
    {
        BGM = SceneTreeProxy.BGM;
        SE = SceneTreeProxy.SE;
        Voice = SceneTreeProxy.Voice;
    }
}
