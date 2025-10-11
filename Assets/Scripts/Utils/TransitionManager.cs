using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private Image fadeImage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        fadeImage.gameObject.SetActive(false);
    }
    
    public async UniTask Visible(float duration)
    {
        await LMotion.Create(1f, 0f, duration)
            .WithEase(Ease.InSine)
            .BindToColorA(fadeImage)
            .ToUniTask(this.GetCancellationTokenOnDestroy());
        fadeImage.gameObject.SetActive(false);
    }

    public async UniTask Invisible(float duration, Color color)
    {
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(true);
        await LMotion.Create(0f, 1f, duration)
            .WithEase(Ease.InSine)
            .BindToColorA(fadeImage)
            .ToUniTask(this.GetCancellationTokenOnDestroy());
    }
}
