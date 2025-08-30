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

    [Header("レベル選択画面")][SerializeField] private GameObject _levelSelectCanvas;

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

    public void OnPressLevelButton(int levelId)
    {
        TransitionToInGame(levelId).Forget();
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