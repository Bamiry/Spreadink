using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [Header("UI")][SerializeField] private GameObject _resultCanvas;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _barPlotOdai;
    [SerializeField] private GameObject _barPlotPlayer;
    [SerializeField] private GameObject _barPlotPrefab;

    [SerializeField] private TextMeshProUGUI _scoreDebugText;

    private void Start()
    {
        _resultCanvas.SetActive(false);
    }

    public void StartResult(StageInfo stageInfo, float[] ratios)
    {
        // スコアを算出
        var colorCounts = new Dictionary<ColorType, (float, float)>();
        for (int i = 0; i < stageInfo.Odai.Keys.Count && i < ratios.Length; i++)
        {
            colorCounts.Add(stageInfo.Odai.Keys.ToArray()[i],
                (stageInfo.Odai.Values.ToArray()[i], ratios[i]));
        }

        var score = CalculateScore(colorCounts);

        // 棒グラフを用意
        CreateBarPlot(_barPlotOdai, stageInfo.Odai.ToDictionary(x => x.Key, x => x.Value / 100f),
            TextPositionType.Top);
        CreateBarPlot(_barPlotPlayer, colorCounts.ToDictionary(
                x => x.Key,
                x => x.Value.Item2 / colorCounts.Values.Sum(pair => pair.Item2)),
            TextPositionType.Bottom); // プレイヤーの割合を正規化);

        // アニメーションつきで表示
        _scoreText.text = $"{score}点";
        ShowResult().Forget();
    }

    public async void OnPressRetryButton()
    {
        await SceneManager.LoadSceneAsync("InGame", LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync("InGame");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("InGame"));
    }

    public void OnPressScreenShotButton()
    {
        Debug.Log("スクリーンショットボタンが押されました");
    }

    public async void OnPressTitleButton()
    {
        await SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync("InGame");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Title"));
    }

    /// <summary>
    /// スコアを計算する
    /// </summary>
    /// <param name="colorCounts">色, (お題の割合, プレイヤーの割合)</param>
    /// <returns></returns>
    private int CalculateScore(Dictionary<ColorType, (float, float)> colorCounts)
    {
        float score = 100; // スコアは簡単のためintとする（仕様）
        var log = new StringBuilder("Score Calculation:\n");
        var sumPlayerRatio = colorCounts.Values.Sum(x => x.Item2);
        foreach (var colorCount in colorCounts)
        {
            var (odaiPercent, playerRatioRaw) = colorCount.Value;
            var playerRatio = playerRatioRaw / sumPlayerRatio; // プレイヤーの割合を正規化
            var odaiRatio = odaiPercent / 100f;
            var diffRatio = Mathf.Abs(odaiRatio - playerRatio);
            var diffPercent = Mathf.RoundToInt(diffRatio * 100f); // パーセントに変換し，intに丸める
            Debug.Log(
                $"色: {colorCount.Key}, 差分: {diffPercent}");
            if (diffPercent > 10)
            {
                Debug.Log($"大幅な差分がありました");
                score -= (diffPercent - 10) * 2;
                diffPercent = 10;
            }
            if (diffPercent > 5)
            {
                Debug.Log($"差分がありました");
                score -= (diffPercent - 5);
                diffPercent = 5;
            }
            score -= diffPercent * 0.5f;
            log.Append($"色：{colorCount.Key}, お題: {odaiRatio:P1}, プレイヤー: {playerRatio:P1}, 差分: {diffRatio:P1}, 減点: {diffPercent}\n");
        }

        Debug.Log(log.ToString());
        _scoreDebugText.text = log.ToString();
        Debug.Log($"最終スコア: {score}");

        score = Mathf.Clamp(score, 0, 100);
        Debug.Log($"最終スコア: {score}");
        int intScore = Mathf.RoundToInt(score);
        return intScore;
    }

    private void CreateBarPlot(GameObject parent, Dictionary<ColorType, float> colorCounts,
        TextPositionType textPositionType)
    {
        var currentRatio = 0f;
        foreach (var (colorType, ratio) in colorCounts)
        {
            var barPlot = Instantiate(_barPlotPrefab, parent.transform);

            // サイズと位置を調整
            var rectTransform = barPlot.GetComponent<RectTransform>();
            var currentSize = rectTransform.sizeDelta;
            rectTransform.sizeDelta = new Vector2(currentSize.x * ratio, currentSize.y);
            var currentPosition = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition =
                new Vector2(currentPosition.x + currentSize.x * currentRatio, currentPosition.y);

            // 色を設定
            var image = barPlot.GetComponent<Image>();
            image.color = InkColorUtil.GetColor(colorType);

            // パーセント表示テキストを設定
            var barPlotComponent = barPlot.GetComponent<BarPlotComponent>();
            barPlotComponent.PercentageText.text = $"{Mathf.Round(ratio * 100f)}%";
            barPlotComponent.TextPosition = textPositionType;

            currentRatio += ratio;
        }
    }

    private async UniTask ShowResult()
    {
        // 準備
        _resultCanvas.SetActive(true);
        _scoreText.gameObject.SetActive(false);
        var barPlotPlayerRect = _barPlotPlayer.GetComponent<RectTransform>();
        barPlotPlayerRect.sizeDelta = new Vector2(0f, barPlotPlayerRect.sizeDelta.y);
        await UniTask.WaitForSeconds(0.5f);
        
        // 棒グラフを表示
        await LMotion.Create(0f, 1300f, 1.0f)
            .WithEase(Ease.OutCubic)
            .BindToSizeDeltaX(barPlotPlayerRect);
        
        // 棒グラフのパーセントテキストを表示
        barPlotPlayerRect.sizeDelta = new Vector2(barPlotPlayerRect.sizeDelta.x, 720f);
        await UniTask.WaitForSeconds(1.0f);
        
        // スコアを表示
        _scoreText.gameObject.SetActive(true);
    }
}