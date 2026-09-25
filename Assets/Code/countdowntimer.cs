using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TextMeshProUGUI timerText;

    void Start()
    {
        // 防止被其他脚本暂停
        Time.timeScale = 1f;

        UpdateTimerText();
        Debug.Log("Timer started");
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            timerText.text = "TIME UP";
            enabled = false; // 停止 Update
            return;
        }

        UpdateTimerText();
    }

    void UpdateTimerText()
    {
        timerText.text = "Time: " + Mathf.Ceil(timeRemaining);
    }
}
