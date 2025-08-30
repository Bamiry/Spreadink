using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    #region 参照
    [SerializeField] private SoundInfoScriptableObject soundInfoSO;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource bgmAudioSource;
    #endregion

    #region プロパティ
    public float MasterVolumeScaler { get; private set; } = 1f;
    #endregion

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnVolumeChanged(float volumeScaler)
    {
        MasterVolumeScaler = volumeScaler;
        bgmAudioSource.volume = MasterVolumeScaler;
    }

    public void PlayOneShot(SoundName name)
    {
        if (soundInfoSO.TryGetSoundInfo(name, out SoundInfo soundInfo))
        {
            audioSource.PlayOneShot(soundInfo.clip, MasterVolumeScaler);
        }
        else
        {
            Debug.LogError($"SoundInfo with name '{name}' not found.");
        }
    }
}