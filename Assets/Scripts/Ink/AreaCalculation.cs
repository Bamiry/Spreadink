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

    /// <summary>
    /// 色リストを受け取り、各色の割合(float)と実行時間(ms)を返す
    /// </summary>
    public void CountEachColor(RenderTexture targetTexture, List<Color> colors, Action<float[], long> onCompleted)
    {
        if (colors == null || colors.Count == 0)
        {
            onCompleted?.Invoke(new float[0], 0);
            return;
        }

        int colorCount = colors.Count;

        var sw = Stopwatch.StartNew();

        // --- 1. バッファの準備 ---
        _colorsBuffer?.Release();
        _colorsBuffer = new ComputeBuffer(colorCount, sizeof(float) * 4);
        _colorsBuffer.SetData(colors.Select(c => (Vector4)c).ToArray());

        _resultBuffer?.Release();
        _resultBuffer = new ComputeBuffer(colorCount, sizeof(uint));
        _resultBuffer.SetData(new uint[colorCount]);

        // --- 2. シェーダー設定 ---
        computeShader.SetTexture(_kernelIndex, "InputTexture", targetTexture);
        computeShader.SetBuffer(_kernelIndex, "TargetColors", _colorsBuffer);
        computeShader.SetInt("TargetColorCount", colorCount);
        computeShader.SetFloat("Tolerance", _tolerance);
        computeShader.SetBuffer(_kernelIndex, "ResultBuffer", _resultBuffer);

        // --- 3. 実行 ---
        int threadGroupsX = Mathf.CeilToInt(targetTexture.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(targetTexture.height / 8.0f);
        computeShader.Dispatch(_kernelIndex, threadGroupsX, threadGroupsY, 1);

#if UNITY_WEBGL
        // ✅ WebGL (WebGPU)では非同期読み出しを使用
        UnityEngine.Debug.Log("Using AsyncGPUReadback for WebGL");
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
#else
        UnityEngine.Debug.Log("Using synchronous readback");
        // ✅ 通常環境では同期読み出しでOK
        uint[] pixelCounts = new uint[colorCount];
        _resultBuffer.GetData(pixelCounts);

        float[] ratios = CalculateRatios(pixelCounts, targetTexture.width, targetTexture.height);
        sw.Stop();
        onCompleted?.Invoke(ratios, sw.ElapsedMilliseconds);
#endif
    }

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
