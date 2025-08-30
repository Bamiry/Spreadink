using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject _titleCanvas;
    [SerializeField] private GameObject _levelSelectCanvas;

    // Start is called before the first frame update
    void Start()
    {
        _titleCanvas.SetActive(true);
        _levelSelectCanvas.SetActive(false);
        // TODO: BGM再生
    }

    public void OnPressStartButton()
    {
        TransitionToLevelSelect().Forget();
    }

    private UniTaskVoid TransitionToLevelSelect()
    {
        _titleCanvas.SetActive(false);
        _levelSelectCanvas.SetActive(true);
        return new UniTaskVoid();
    }
}