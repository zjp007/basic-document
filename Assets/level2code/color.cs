using UnityEngine;
using UnityEngine.UI;

public class FailureScreenFlash : MonoBehaviour
{
    [Header("分数系统")]
    public ScoreAndCollision scoreSystem; // 拖入你的分数脚本

    [Header("UI设置")]
    public Image flashImage; // 全屏红色UI Image
    public float baseFlashDuration = 0.2f; // 初始闪烁时间
    public float baseFlashFrequency = 1f;  // 初始闪烁频率（每秒闪烁次数）
    public int maxFailures = 10;            // 最大失败次数限制

    private float flashTimer = 0f;
    private bool isFlashing = false;
    private int failureCount = 0;

    void Start()
    {
        if (scoreSystem == null)
            scoreSystem = FindObjectOfType<ScoreAndCollision>();

        if (flashImage != null)
        {
            Color c = flashImage.color;
            c.a = 0f; // 初始化透明
            flashImage.color = c;
        }
    }

    void Update()
    {
        if (scoreSystem == null || flashImage == null) return;

        // 检测失败条件（分数减少）
        if (scoreSystem.debugMode && scoreSystem.score < 0) // 可改为你的失败判定
        {
            RegisterFailure();
        }

        if (isFlashing)
        {
            flashTimer -= Time.deltaTime;
            float alpha = Mathf.PingPong(Time.time * GetCurrentFrequency(), 1f);
            Color c = flashImage.color;
            c.a = alpha;
            flashImage.color = c;

            if (flashTimer <= 0f)
            {
                isFlashing = false;
                Color reset = flashImage.color;
                reset.a = 0f;
                flashImage.color = reset;
            }
        }
    }

    void RegisterFailure()
    {
        failureCount = Mathf.Min(failureCount + 1, maxFailures);
        isFlashing = true;
        flashTimer = GetCurrentDuration();
    }

    float GetCurrentDuration()
    {
        // 闪烁持续时间随失败次数增加
        return baseFlashDuration + failureCount * 0.1f;
    }

    float GetCurrentFrequency()
    {
        // 闪烁频率随失败次数增加
        return baseFlashFrequency + failureCount * 0.5f;
    }
}
