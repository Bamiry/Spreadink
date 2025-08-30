using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [Header("UI")] [SerializeField] private GameObject _resultCanvas;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private ResultManager()
    {
    }

    public static ResultManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        // アニメーションつきで表示
        _scoreText.text = $"{score} %";
        _resultCanvas.SetActive(true);
    }
    
    public void OnPressRetryButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("InGame");
    }

    public void OnPressScreenShotButton()
    {
        Debug.Log("スクリーンショットボタンが押されました");
    }

    public void OnPressTitleButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// スコアを計算する
    /// </summary>
    /// <param name="colorCounts">色, (お題の割合, プレイヤーの割合)</param>
    /// <returns></returns>
    private static int CalculateScore(Dictionary<ColorType, (float, float)> colorCounts)
    {
        int score = 100; // スコアは簡単のためintとする（仕様）
        StringBuilder log = new StringBuilder("Score Calculation:\n");
        foreach (var colorCount in colorCounts)
        {
            var (odaiPercent, playerRatio) = colorCount.Value;
            var odaiRatio = odaiPercent / 100f;
            var diffRatio = Mathf.Abs(odaiRatio - playerRatio);
            var diffPercent = (int)(diffRatio * 100f); // パーセントに変換し，intに丸める
            score -= diffPercent;
            log.Append($"色：{colorCount.Key}, お題の割合: {odaiRatio:P1}, プレイヤーの割合: {playerRatio:P1}, 差分: {diffRatio:P1}, 減点: {diffPercent}\n");
        }
        
        Debug.Log(log.ToString());

        return score;
    }
}