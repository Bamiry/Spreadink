using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Example : MonoBehaviour
{
    public ColorCounter colorCounter;
    public RenderTexture targetRenderTexture;
    public Text resultText;

    [Header("カウントする色のリスト")]
    public List<Color> targetColors = new List<Color>();

    [Header("色の許容誤差")]
    [Range(0, 1)]
    public float tolerance = 0.1f;

    void Start()
    {
        // 事前にリストに色を追加しておく
        targetColors.Add(Color.white); // 0番目
        targetColors.Add(Color.red);   // 1番目
        targetColors.Add(Color.blue);  // 2番目
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 新しいメソッドを呼び出す
            colorCounter.CountEachColor(targetRenderTexture, targetColors, tolerance, OnCountCompleted);
        }
    }

    // uintの配列を受け取るように変更
    private void OnCountCompleted(uint[] counts)
    {
        if (counts.Length != targetColors.Count) return;

        Debug.Log("--- 各色のピクセル数 ---");

        string resultLog = "";

        for (int i = 0; i < counts.Length; i++)
        {
            // どの色が何ピクセルあったかを表示
            string log = $"Color[{i}] ({targetColors[i]}): {counts[i]} pixels";
            Debug.Log(log);
            resultLog += log + "\n";
        }

        if (resultText != null)
        {
            resultText.text = resultLog;
        }
    }
}