using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("タイトル画面")][SerializeField] private GameObject _titleCanvas;

    [SerializeField] private CanvasGroup _titleCanvasGroup;
    [SerializeField] private List<GameObject> _inkList;
    [SerializeField] private GameObject _hamburgerMenu;
    [SerializeField] private GameObject _hamburgerButton;

    [Header("レベル選択画面")]
    [SerializeField] private GameObject _levelSelectCanvas;
    [SerializeField] private Button[] levelButtons;
    
    public static bool PleaseOpenLevelSelect { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        if (PleaseOpenLevelSelect)
        {
            PleaseOpenLevelSelect = false;
            _titleCanvas.SetActive(false);
            _levelSelectCanvas.SetActive(true);
        }
        else
        {
            _titleCanvas.SetActive(true);
            _levelSelectCanvas.SetActive(false);
        }
        _hamburgerMenu.SetActive(false);
        _levelSelectCanvas.SetActive(false);
        for (int i=0; i<levelButtons.Length; i++)
        {
            int levelId = i; // クロージャ対策
            Debug.Log(levelButtons[i].gameObject);
            levelButtons[i].onClick.AddListener(
                () =>
                {
                    OnPressLevelButton(levelId + 1, levelButtons[levelId].gameObject);
                });
        }
    }

    public void OnPressStartButton()
    {
        TransitionToLevelSelect().Forget();
    }

    public void OnPressLevelButton(int levelId, GameObject button)
    {
        TransitionToInGame(levelId, button).Forget();
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
        await UniTask.DelayFrame(1);
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

    private async UniTaskVoid TransitionToInGame(int levelId, GameObject button)
    {
        StageIDHolder.Instance.SetStageID(levelId);
        await TransitionManager.Instance.Invisible(0.5f, button.GetComponent<Image>().color);
        await SceneManager.LoadSceneAsync("InGame", LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync("Title");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("InGame"));
        await TransitionManager.Instance.Visible(0.5f);

    }
}