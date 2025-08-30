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
    [SerializeField] private StagesScriptableObject stagesSO;
    [SerializeField] private Image stageImage;
    [SerializeField] private Image renderStageImage; 
    [SerializeField] private TouchDetector touchDetector;

    [Header("ヘッダー設定")]
    [SerializeField] private Button hamburgerBtn;
    [SerializeField] private Transform odaiTextViews;
    [SerializeField] private GameObject odaiTextViewPrefab;

    [Header("面積計算")]
    [SerializeField] private ColorCounter colorCounter;
    [SerializeField] private RenderTexture renderTexture;
    #endregion

    #region プロパティ
    public bool IsGamePaused { get; private set; } = true;
    public StageInfo CurrentStage { get; private set; } = null;
    #endregion

    #region 状態変数
    private CompositeMotionHandle handle;
    private PalatteInk currentPalatteInk;
    private List<Color> odaiColors = new();
    #endregion

    async UniTaskVoid Start()
    {
        await UniTask.DelayFrame(1);
        InitializeGame();
    }

    void Update()
    {
        if (!IsGamePaused)
            colorCounter.CountEachColor(renderTexture, odaiColors, OnCountCompleted);
    }

    void InitializeGame()
    {
        CurrentStage = GetStage();
        odaiColors = CurrentStage.AvailableColors;
        odaiColors.Add(Color.white);
        CreateOdaiTextViews();
        CreateStage();
        CreatePalatte();

        handle = new CompositeMotionHandle();
        touchDetector.OnTouch += (pos) => DropInk(pos);

        hamburgerBtn.onClick.AddListener(() =>
        {
            if (IsGamePaused)
                ResumeInkSpreading();
            else
                PauseInkSpreading();
        });

        IsGamePaused = false;
    }

    StageInfo GetStage()
    {
        int id = StageIDHolder.Instance.StageID;
        if (stagesSO.StageDict.TryGetValue(id, out var stage))
        {
            return stage;
        }
        else
        {
            Debug.LogError($"Stage ID {id} not found.");
            return null;
        }
    }

    void CreateOdaiTextViews()
    {
        foreach (var (colorType, ratio) in CurrentStage.Odai)
        {
            var color = InkColorUtil.GetColor(colorType);
            var odaiTextView = Instantiate(odaiTextViewPrefab, odaiTextViews).GetComponent<OdaiTextView>();
            odaiTextView.Set(color, ratio);
        }
    }

    void CreateStage()
    {
        if (CurrentStage == null) return;

        stageImage.sprite = CurrentStage.Image;
        renderStageImage.sprite = CurrentStage.Image;
    }

    void CreatePalatte()
    {
        foreach (var color in CurrentStage.AvailableColors)
        {
            var palatteInk = Instantiate(palattePrefab, palatte).GetComponent<PalatteInk>();
            palatteInk.OnTouch += (palatteInk) =>
            {
                currentPalatteInk?.TurnSelected();
                if (!palatteInk.IsUsed)
                    currentPalatteInk = palatteInk;
            };
            palatteInk.Set(color);
        }
    }

    void DropInk(Vector2 position)
    {
        if (IsGamePaused) return;

        if (currentPalatteInk == null) return;
        if (currentPalatteInk.IsUsed) return;

        // 生成
        var ink = Instantiate(inkPrefab, inks);
        ink.transform.position = calcCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, 10f));

        // 色設定
        var image = ink.GetComponent<Image>();
        image.color = currentPalatteInk.Color;

        // 広げる
        LMotion.Create(inkDefaultScale * Vector2.one, Vector2.one, durationToMaxScale * CurrentStage.SpreadSpeedRate)
            .BindToLocalScaleXY(ink.transform)
            .AddTo(handle);

        currentPalatteInk.OnUse();
    }

    void StopInkSpreading()
    {
        handle.Cancel();
        IsGamePaused = true;
    }

    void PauseInkSpreading()
    {
        foreach (var motion in handle)
        {
            motion.PlaybackSpeed = 0;
        }
        IsGamePaused = true;
    }

    void ResumeInkSpreading()
    {
        foreach (var motion in handle)
        {
            motion.PlaybackSpeed = 1;
        }
        IsGamePaused = false;
    }

    void OnCountCompleted(float[] ratios, long elapsedMs)
    {
        if (ratios[^1] <= 0)
        {
            StopInkSpreading();
            ResultManager.Instance.StartResult(CurrentStage, ratios);
        }
    }
}
