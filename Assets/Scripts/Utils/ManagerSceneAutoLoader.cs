using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class ManagerSceneAutoLoader
{
    public static bool IsManagerSceneLoaded { get; private set; } = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadManagerSceneAsync()
    {
        UniTask.Void(async () =>
        {
            string managerSceneName = "ManagerScene";

            if (!SceneManager.GetSceneByName(managerSceneName).IsValid())
            {
                var op = SceneManager.LoadSceneAsync(managerSceneName, LoadSceneMode.Additive);
                await op; // ManagerSceneがロード完了するまで待つ
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(managerSceneName));
            }

            IsManagerSceneLoaded = true;
        });
    }
}
