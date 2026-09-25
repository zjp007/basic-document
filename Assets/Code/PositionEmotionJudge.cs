using UnityEngine;

public class PositionEmotionJudge : MonoBehaviour
{
    [Header("XR")]
    public Transform head;       // Main Camera
    public float maxX = 0.5f;    // 最大左右有效距离（米）

    [Header("Emotion Target")]
    [Range(-1f, 1f)]
    public float targetEmotion; // -1 = 开心，+1 = 愤怒

    [Header("Score")]
    public int score;

    public float checkInterval = 0.5f;

    [Header("Judge Range")]
    public float perfectRange = 0.1f;
    public float goodRange = 0.25f;
    public float punishRange = 0.6f;

    [Header("Score Value")]
    public int perfectScore = 10;
    public int goodScore = 3;
    public int punishScore = 5;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            JudgeEmotion();
        }
    }

    void JudgeEmotion()
    {
        // 1. 读取玩家头部左右位置（XR Origin 为 0）
        float playerX = Mathf.Clamp(
            head.localPosition.x / maxX,
            -1f,
            1f
        );

        // 2. 计算与目标情绪的距离
        float distance = Mathf.Abs(playerX - targetEmotion);

        // 3. 根据距离加分 / 扣分
        if (distance < perfectRange)
        {
            score += perfectScore;
            Debug.Log("Perfect 对齐情绪 + " + perfectScore);
        }
        else if (distance < goodRange)
        {
            score += goodScore;
            Debug.Log("Good 情绪接近 + " + goodScore);
        }
        else if (distance > punishRange)
        {
            score -= punishScore;
            Debug.Log("情绪错误 - " + punishScore);
        }
    }
}
