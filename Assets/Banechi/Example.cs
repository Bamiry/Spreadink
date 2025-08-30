using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Example : MonoBehaviour
{
    public ColorCounter colorCounter;
    public RenderTexture targetRenderTexture;
    public TextMeshProUGUI resultText;

    [Header("カウントする色のリスト (ColorType)")]
    public List<ColorType> targetColorTypes = new List<ColorType>();

    // ColorTypeから変換したColor配列（関数呼び出し用）
    private List<Color> convertedColors = new List<Color>();


    void Start()
    {
        // 事前にリストに白色を追加しておく
        targetColorTypes.Add(ColorType.White); // 白
        UpdateConvertedColors();

    }

    // targetColorTypesからconvertedColorsへ変換
    private void UpdateConvertedColors()
    {
        convertedColors = targetColorTypes.ConvertAll(InkColorUtil.GetColor);
        Debug.Log($"Converted {convertedColors.Count} colors.");
        // convertedColorsを１つ一つログに出力
        foreach (var color in convertedColors)
        {
            //タイプもログに出力
            var colorType = targetColorTypes[convertedColors.IndexOf(color)];
            Debug.Log($"Converted Color: {color} (Type: {colorType})");
        }
    }

    void Update()
    {
    }

    public void CountButtonPressed()
    {
        UpdateConvertedColors();
        colorCounter.CountEachColor(targetRenderTexture, convertedColors, OnCountCompleted);
    }

    // 各色の割合(float)と実行時間(ms)を受け取るように変更
    private void OnCountCompleted(float[] ratios, long elapsedMs)
    {
        if (ratios.Length != targetColorTypes.Count) return;

        string resultLog = "\n";

        for (int i = 0; i < ratios.Length; i++)
        {
            string log = $"Color[{i}] ({targetColorTypes[i]}): {ratios[i]}";
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