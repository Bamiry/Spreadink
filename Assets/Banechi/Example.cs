using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Example : MonoBehaviour
{
    public ColorCounter colorCounter;
    public RenderTexture targetRenderTexture;
    public TextMeshProUGUI resultText;

    [Header("カウントする色のリスト")]
    public List<Color> targetColors = new List<Color>();

    [Header("色の許容誤差")]
    [Range(0, 1)]
    public float tolerance = 0.1f;

    void Start()
    {
        // 事前にリストに色を追加しておく
        targetColors.Add(Color.white); // 0番目
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 新しいメソッドを呼び出す
            colorCounter.CountEachColor(targetRenderTexture, targetColors, tolerance, OnCountCompleted);
        }
    }

    public void CountButtonPressed()
    {
        colorCounter.CountEachColor(targetRenderTexture, targetColors, tolerance, OnCountCompleted);
    }

    // uintの配列と実行時間(ms)を受け取るように変更
    private void OnCountCompleted(uint[] counts, long elapsedMs)
    {
        if (counts.Length != targetColors.Count) return;

        string resultLog = "";

        for (int i = 0; i < counts.Length; i++)
        {
            float ratio = counts[i] / (float)targetRenderTexture.width / (float)targetRenderTexture.height;
            string log = $"Color[{i}] ({targetColors[i]}): {counts[i]} pixels ({ratio:P1})";
            resultLog += log + "\n";
        }

        resultLog += $"\ntime: {elapsedMs} ms";

        if (resultText != null)
        {
            resultText.text = resultLog;
        }
        else
        {
            Debug.LogWarning("resultTextが設定されていません。");
        }
    }
}