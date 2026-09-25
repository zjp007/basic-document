using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public int targetScore = 100;

    public GameObject nextLevelUI; // 拖你的 NextLevelText 或 Panel

    void Start()
    {
        // 游戏开始时隐藏
         nextLevelUI.SetActive(true);
    }

    public void AddScore(int value)
    {
        score += value;

        if (score >= targetScore)
        {
            ShowNextLevel();
        }
    }

    void ShowNextLevel()
    {
        nextLevelUI.SetActive(true);
        Debug.Log("Score reached 100 - Next Level shown");
    }
}
