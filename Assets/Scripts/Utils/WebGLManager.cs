using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLManager : MonoBehaviour
{
    private bool isLandscape = false;
    private float screenAspectRatio = 0f;
    [SerializeField] private GameObject landscapeCheckPannel;
    [SerializeField] private GameObject webGPUCheckPannel;
    private bool isWebGPUAvailable = true;


#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int IsWebGPUAvailable();
#endif

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (IsWebGPUAvailable() == 0)
        {
            webGPUCheckPannel.SetActive(true);
            isWebGPUAvailable = false;
        }
#else
        Debug.Log("WebGPU check only available in WebGL build.");
#endif
    }
    private void Update()
    {
        if (!isWebGPUAvailable)
        {
            return;
        }
        // 毎フレーム横画面判定を実行
        CheckScreenOrientation();
    }
    
    /// <summary>
    /// スクリーンが横長（横画面）かどうかを判定する
    /// </summary>
    private void CheckScreenOrientation()
    {
        // スクリーンのアスペクト比を計算
        screenAspectRatio = (float)Screen.width / Screen.height;
        
        // アスペクト比が1.0より大きければ横画面
        isLandscape = screenAspectRatio > 1.0f;
        
        // デバッグログ（必要に応じてコメントアウト）
        if (!isLandscape)
        {
            landscapeCheckPannel.SetActive(true);
        }
        else
        {
            landscapeCheckPannel.SetActive(false);
        }
    }


    /// <summary>
    /// デバッグ情報をコンソールに出力
    /// </summary>
    [ContextMenu("デバッグ情報を出力")]
    public void LogDebugInfo()
    {
        Debug.Log($"=== WebGLManager デバッグ情報 ===");
        Debug.Log($"スクリーンサイズ: {Screen.width} x {Screen.height}");
        Debug.Log($"アスペクト比: {screenAspectRatio:F2}");
        Debug.Log($"横画面モード: {isLandscape}");
        Debug.Log($"プラットフォーム: {Application.platform}");
    }
}
