using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "SoundInfo", menuName = "ScriptableObjects/SoundInfo", order = 1)]
public class SoundInfoScriptableObject : ScriptableObject
{
    public SoundInfo[] soundInfos;

    public bool TryGetSoundInfo(SoundName name, out SoundInfo soundInfo)
    {
        soundInfo = soundInfos.FirstOrDefault(s => s.name == name);
        return soundInfo != null;
    }
}

public enum SoundName
{
    ButtonTapped,
    InkDrop,
    GameFinished
}

[System.Serializable]
public class SoundInfo
{
    public SoundName name;
    public AudioClip clip;
}