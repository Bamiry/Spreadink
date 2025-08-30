using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitMotion;
using LitMotion.Extensions;

public class TitleManager : MonoBehaviour
{
    [Header("タイトル画面")][SerializeField] private GameObject _titleCanvas;

    [SerializeField] private CanvasGroup _titleCanvasGroup;
    [SerializeField] private List<GameObject> _inkList;
    [SerializeField] private GameObject _hamburgerMenu;
    [SerializeField] private GameObject _hamburgerButton;

    [Header("レベル選択画面")][SerializeField] private GameObject _levelSelectCanvas;

    // Start is called before the first frame update
    void Start()
    {
        _titleCanvas.SetActive(true);
        _hamburgerMenu.SetActive(false);
        _levelSelectCanvas.SetActive(false);
        // TODO: BGM再生
    }

    public void OnPressStartButton()
    {
        TransitionToLevelSelect().Forget();
    }

    public void OnPressLevelButton(int levelId)
    {
        TransitionToInGame(levelId).Forget();
    }

    public void OnPressHamburgerButton()
    {
        _hamburgerMenu.SetActive(!_hamburgerMenu.activeSelf);
        _hamburgerButton.SetActive(!_hamburgerMenu.activeSelf);
    }

    public void OnPressCloseHamburgerButton()
    {
        _hamburgerMenu.SetActive(false);
        _hamburgerButton.SetActive(true);
    }

    public void OnPressReturnToTitleButton()
    {
        _hamburgerMenu.SetActive(false);
        _hamburgerButton.SetActive(true);
        _levelSelectCanvas.SetActive(false);
        _titleCanvas.SetActive(true);
    }

    private async UniTaskVoid TransitionToLevelSelect()
    {
        // なんか関数呼ばれてからアニメーション始まるまでタイムラグがある
        // LMotion.Create(1f, 0f, 0.5f).WithEase(Ease.InQuint).BindToAlpha(_titleCanvasGroup);
        // foreach (var ink in _inkList)
        // {
        //     LMotion
        //         .Create(ink.transform.localScale, ink.transform.localScale * 3.0f, 0.5f)
        //         .BindToLocalScale(ink.transform);
        // }
        // await UniTask.Delay(1000);
        _titleCanvas.SetActive(false);
        _levelSelectCanvas.SetActive(true);
    }

    private async UniTaskVoid TransitionToInGame(int levelId)
    {
        StageIDHolder.Instance.SetStageID(levelId);
        await SceneManager.LoadSceneAsync("InGame", LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync("Title");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("InGame"));

    }
}