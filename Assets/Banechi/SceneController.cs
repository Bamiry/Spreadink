using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    // Titleで決定されたIDを保持
    public int StageID { get; private set; } = 1;

    [SerializeField] private string titleSceneName = "Title";
    // [SerializeField] private string inGameSceneName = "InGame";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Managerシーンに配置して、最初から残す
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 起動時に Title を Additive で読み込む
        if (!SceneManager.GetSceneByName(titleSceneName).isLoaded)
        {
            SceneManager.LoadScene(titleSceneName, LoadSceneMode.Additive);
        }
    }

    /// <summary>
    /// TitleでIDが選ばれたときに呼ぶ
    /// </summary>
    public void SetStageIDAndStartGame(int id)
    {
        StageID = id;
        StartCoroutine(LoadInGameScene());
    }

    private System.Collections.IEnumerator LoadInGameScene()
    {
        // Titleシーンをアンロード
        yield return SceneManager.UnloadSceneAsync(titleSceneName);

        // InGameを追加ロード
        // yield return SceneManager.LoadSceneAsync(inGameSceneName, LoadSceneMode.Additive);

        // InGameをアクティブに設定
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName(inGameSceneName));
    }
}
