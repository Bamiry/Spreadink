using UnityEngine;

/// <summary>
/// ステージIDを保持するだけのクラス
/// </summary>
public class StageIDHolder : MonoBehaviour
{
    // 他のスクリプトからアクセスできるようにする
    public static StageIDHolder Instance { get; private set; }

    // 現在のステージID
    public int StageID { get; private set; } = -1;

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

    /// <summary>
    /// ステージIDを設定
    /// </summary>
    public void SetStageID(int id)
    {
        StageID = id;
    }
}
