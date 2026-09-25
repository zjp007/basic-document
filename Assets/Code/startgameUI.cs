using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameUI : MonoBehaviour
{
    [Tooltip("要加载的场景名，默认 Level1")]
    public string sceneName = "Level1";

    // 点击按钮时调用
    public void StartGame()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[StartGameUI] 未设置 sceneName。");
            return;
        }

        // 检查场景是否已加入 Build Settings（更稳健的提示）
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"[StartGameUI] 无法加载场景 '{sceneName}'。请确认场景名正确并已在 __File > Build Settings__ 中添加。");
        }
    }
}