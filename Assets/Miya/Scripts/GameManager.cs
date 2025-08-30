using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;

public class GameManager : MonoBehaviour
{
    #region 参照
    [Header("インク設定")]
    [SerializeField] private Transform inks;
    [SerializeField] private GameObject inkPrefab;
    [SerializeField] private float inkDefaultScale = 0.05f;
    [SerializeField] private float durationToMaxScale = 1f;

    [Header("パレット設定")]
    [SerializeField] private Transform palatte;
    [SerializeField] private GameObject palattePrefab;

    [Header("ステージ設定")]
    [SerializeField] private Image stageImage;
    [SerializeField] private TouchDetector touchDetector;

    [Header("ヘッダー設定")]
    [SerializeField] private Button hamburgerBtn;
    #endregion

    #region プロパティ
    public bool IsGamePaused { get; private set; } = true;
    #endregion

    #region 状態変数
    CompositeMotionHandle handle;
    #endregion

    async UniTaskVoid Start()
    {
        await UniTask.DelayFrame(1);
        InitializeGame();
    }

    void InitializeGame()
    {
        IsGamePaused = false;

        handle = new CompositeMotionHandle();

        touchDetector.OnTouch += (pos) => DropInk(pos);
    }

    void DropInk(Vector2 position)
    {
        // 生成
        var ink = Instantiate(inkPrefab, inks);
        ink.transform.position = position;

        // 色設定
        var image = ink.GetComponent<Image>();
        image.color = Color.Lerp(Color.white, Color.clear, 0.5f);

        // 広げる
        LMotion.Create(inkDefaultScale * Vector2.one, Vector2.one, durationToMaxScale)
            .BindToLocalScaleXY(ink.transform)
            .AddTo(handle);
    }
}
