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
    [SerializeField] private Camera calcCamera;

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
    private CompositeMotionHandle handle;
    private PalatteInk currentPalatteInk;
    #endregion

    async UniTaskVoid Start()
    {
        await UniTask.DelayFrame(1);
        InitializeGame();
    }

    void InitializeGame()
    {
        IsGamePaused = false;

        CreatePalatte();

        handle = new CompositeMotionHandle();
        touchDetector.OnTouch += (pos) => DropInk(pos);
    }

    void CreatePalatte()
    {
        for (int i = 0; i < 4; i++)
        {
            var palatteInk = Instantiate(palattePrefab, palatte).GetComponent<PalatteInk>();
            palatteInk.OnTouch += (palatteInk) =>
            {
                if (!palatteInk.IsUsed)
                    currentPalatteInk = palatteInk;
            };
            palatteInk.Set(Color.HSVToRGB(i / 4f, 1f, 1f));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            handle.Cancel();
        }
    }

    void DropInk(Vector2 position)
    {
        if (currentPalatteInk == null) return;
        if (currentPalatteInk.IsUsed) return;

        // 生成
        var ink = Instantiate(inkPrefab, inks);
        ink.transform.position = calcCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, 10f));

        // 色設定
        var image = ink.GetComponent<Image>();
        image.color = currentPalatteInk.Color;

        // 広げる
        LMotion.Create(inkDefaultScale * Vector2.one, Vector2.one, durationToMaxScale)
            .BindToLocalScaleXY(ink.transform)
            .AddTo(handle);

        currentPalatteInk.OnUse();
    }
}
