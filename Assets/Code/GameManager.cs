using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PrimeTween;

public class GameManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float gameTime = 60f;           // 总时间
    public TMP_Text timerText;             // UI 显示

    [HideInInspector]
    public bool gameEnded = false;         // 游戏是否结束
    
    [Header("当前关卡结束显示的图像")]
    public Image EndImg;
    
    public Wallgen wallgen;
    public ScoreAndCollision score;
    
    private bool isEndShow = false;
    
    
    
    void start()
    {
        // score = FindObjectOfType<ScoreAndCollision>();
        // wallgen = FindObjectOfType<Wallgen>();
    }

    void Update()
    {
        Debug.Log("score.IsStartFlag : " + score.IsStartFlag);
        if (gameEnded) return;
        if(!score.IsStartFlag) return;

        // 倒计时
        gameTime -= Time.deltaTime;
        if (gameTime <= 0f)
        {
            gameTime = 0f;
            EndGame();
        }

        // 更新 UI
        if (timerText)
            timerText.text = "Time: " + Mathf.CeilToInt(gameTime);
    }

    public void EndGame()
    {
        gameEnded = true;
        if(isEndShow) return;
        isEndShow = true;
        if(score) score.SetIsStartFlag(false);
        EndImg.enabled = true;
        Tween.Custom(0.0f, 1.0f, 1, onValueChange: x => EndImg.color = new Color(1, 1, 1, x))
            .OnComplete(target: this, target =>
            {
                if(target.wallgen) target.wallgen.StopSpawning();
                
                MoveLeft[] monsters = FindObjectsOfType<MoveLeft>();
                foreach (MoveLeft monster in monsters)
                {
                    monster.speed = 0;
                }
            });;
        Debug.Log("Game Over!");
        // TODO: 显示结算面板或切换场景
        
        
    }
}
