using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEngine.Rendering;

public class ColorCounter : MonoBehaviour
{
    public ComputeShader computeShader;
    private ComputeBuffer _resultBuffer;
    private ComputeBuffer _colorsBuffer;
    private int _kernelIndex;
    private float _tolerance = 0.3f;

    void Start()
    {
        _kernelIndex = computeShader.FindKernel("CSMain");
    }

    void OnDestroy()
    {
        _resultBuffer?.Release();
        _colorsBuffer?.Release();
    }

    // 色のリストを受け取り、各色の割合(float)と実行時間(ms)を返すように変更
    public void CountEachColor(RenderTexture targetTexture, List<Color> colors, Action<float[], long> onCompleted)
    {
        if (colors == null || colors.Count == 0)
        {
            onCompleted?.Invoke(new float[0], 0);
            return;
        }

        int colorCount = colors.Count;

        var sw = Stopwatch.StartNew();

        // 1. 色リスト用のバッファを準備
        _colorsBuffer?.Release();
        _colorsBuffer = new ComputeBuffer(colorCount, sizeof(float) * 4); // float4
                                                                          // ColorからVector4の配列に変換してバッファにセット
        _colorsBuffer.SetData(colors.Select(c => (Vector4)c).ToArray());

        // 2. 結果格納用のバッファを準備（サイズを色の数に合わせる）
        _resultBuffer?.Release();
        _resultBuffer = new ComputeBuffer(colorCount, sizeof(uint));
        _resultBuffer.SetData(new uint[colorCount]); // 0で初期化

        // 3. シェーダーにデータを設定
        computeShader.SetTexture(_kernelIndex, "InputTexture", targetTexture);
        computeShader.SetBuffer(_kernelIndex, "TargetColors", _colorsBuffer);
        computeShader.SetInt("TargetColorCount", colorCount);
        computeShader.SetFloat("Tolerance", _tolerance);
        computeShader.SetBuffer(_kernelIndex, "ResultBuffer", _resultBuffer);

        // 4. 実行と結果取得
        int threadGroupsX = Mathf.CeilToInt(targetTexture.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(targetTexture.height / 8.0f);
        computeShader.Dispatch(_kernelIndex, threadGroupsX, threadGroupsY, 1);

        // --- 5. 非同期でGPUデータ読み出し ---
        AsyncGPUReadback.Request(_resultBuffer, (req) =>
        {
            sw.Stop();

            if (req.hasError)
            {
                UnityEngine.Debug.LogError("AsyncGPUReadback failed.");
                onCompleted?.Invoke(new float[colorCount], sw.ElapsedMilliseconds);
                return;
            }

            uint[] pixelCounts = req.GetData<uint>().ToArray();
            float[] ratios = CalculateRatios(pixelCounts, targetTexture.width, targetTexture.height);
            onCompleted?.Invoke(ratios, sw.ElapsedMilliseconds);
        });
    }

    /// <summary>
    /// ピクセルカウントから割合を算出
    /// </summary>
    private float[] CalculateRatios(uint[] pixelCounts, int width, int height)
    {
        float[] ratios = new float[pixelCounts.Length];
        int totalPixels = width * height;
        for (int i = 0; i < pixelCounts.Length; i++)
        {
            ratios[i] = totalPixels > 0 ? (float)pixelCounts[i] / totalPixels : 0f;
        }
        return ratios;
    }
}