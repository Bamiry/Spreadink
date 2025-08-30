using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class ColorCounter : MonoBehaviour
{
    public ComputeShader computeShader;
    private ComputeBuffer _resultBuffer;
    private ComputeBuffer _colorsBuffer;
    private int _kernelIndex;

    void Start()
    {
        _kernelIndex = computeShader.FindKernel("CSMain");
    }

    void OnDestroy()
    {
        _resultBuffer?.Release();
        _colorsBuffer?.Release();
    }

    // 色のリストを受け取り、各色のピクセル数と実行時間(ms)を返すように変更
    public void CountEachColor(RenderTexture targetTexture, List<Color> colors, float tolerance, Action<uint[], long> onCompleted)
    {
        if (colors == null || colors.Count == 0)
        {
            onCompleted?.Invoke(new uint[0], 0);
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
        computeShader.SetFloat("Tolerance", tolerance);
        computeShader.SetBuffer(_kernelIndex, "ResultBuffer", _resultBuffer);

        // 4. 実行と結果取得
        int threadGroupsX = Mathf.CeilToInt(targetTexture.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(targetTexture.height / 8.0f);
        computeShader.Dispatch(_kernelIndex, threadGroupsX, threadGroupsY, 1);

        uint[] result = new uint[colorCount];
        _resultBuffer.GetData(result);

        sw.Stop();
        onCompleted?.Invoke(result, sw.ElapsedMilliseconds);
    }
}