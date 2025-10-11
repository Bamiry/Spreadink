using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class VolumeChange : MonoBehaviour
{
    private Slider slider;

    async void Awake()
    {
        await UniTask.WaitUntil(() => ManagerSceneAutoLoader.IsManagerSceneLoaded);
        slider = GetComponent<Slider>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            // SoundManagerの現在の音量で初期値を設定
            if (SoundManager.Instance != null)
            {
                slider.value = SoundManager.Instance.MasterVolumeScaler;
            }
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnVolumeChanged(value);
        }
    }
}
