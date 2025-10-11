using UnityEngine;

/// <summary>
/// 横画面固定（左右両対応）にするマネージャー
/// ManagerSceneにアタッチして使用
/// </summary>
public class ScreenOrientationManager : MonoBehaviour
{
    void Start()
    {
        // 縦向きは無効化
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        // 横向きを有効化
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // 自動回転に設定
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}
