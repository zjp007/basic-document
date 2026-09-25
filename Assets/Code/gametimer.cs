using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [Header("计时器设置")]
    public float levelTime = 60f; // 1 分钟
    private float timer;
    private bool timerRunning = true;
    private bool showGameOver = false;

    [Header("分数系统")]
    public ScoreAndCollision scoreSystem;

    private GUIStyle timerStyle;
    private GUIStyle gameOverStyle;

    void Start()
    {
        timer = levelTime;

        // 自动查找 ScoreAndCollision 脚本
        if (scoreSystem == null)
        {
            scoreSystem = FindObjectOfType<ScoreAndCollision>();
            if (scoreSystem == null)
                Debug.LogWarning("未找到 ScoreAndCollision 脚本，请拖入 LevelTimer 的 scoreSystem 字段");
        }

        // 初始化 GUI 样式
        timerStyle = new GUIStyle();
        timerStyle.fontSize = 24;
        timerStyle.normal.textColor = Color.yellow;
        timerStyle.fontStyle = FontStyle.Bold;
        timerStyle.alignment = TextAnchor.UpperCenter;

        gameOverStyle = new GUIStyle();
        gameOverStyle.fontSize = 48;
        gameOverStyle.normal.textColor = Color.red;
        gameOverStyle.fontStyle = FontStyle.Bold;
        gameOverStyle.alignment = TextAnchor.MiddleCenter;
    }

    void Update()
    {
        if (!timerRunning) return;

        // 分数达到 100，停止倒计时
        if (scoreSystem != null && scoreSystem.score >= scoreSystem.maxScore)
        {
            timerRunning = false;
            return;
        }

        // 倒计时
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            timerRunning = false;

            // 倒计时结束，分数未达到 100
            if (scoreSystem != null && scoreSystem.score < scoreSystem.maxScore)
            {
                showGameOver = true;
            }
        }
    }

    void OnGUI()
    {
        // 显示倒计时在屏幕正上方
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);
        GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 30), timeText, timerStyle);

        // 显示 GAME OVER
        if (showGameOver)
        {
            GUI.Label(new Rect(0, Screen.height / 2 - 40, Screen.width, 80), "GAME OVER", gameOverStyle);
        }
    }
}
