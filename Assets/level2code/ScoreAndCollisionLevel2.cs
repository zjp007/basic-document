using UnityEngine;
using UnityEngine.Events;
using PrimeTween;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider))]
public class ScoreAndCollisionLevel2 : MonoBehaviour
{
    // -----------------------------
    // 分数设置
    // -----------------------------
    public int score = 0;
    public int maxScore = 100;
    public bool debugMode = true;

    // -----------------------------
    // 无碰撞加分设置
    // -----------------------------
    public float requiredNoHitTime = 3f;
    public int rewardOnNoCollision = 10;
    public int penaltyOnCollision = 5;

    public UnityEvent onScoreMaxComplte;
    
    private GlobalVolumeControl globalVolumeControl;
    
    // -----------------------------
    // 计时与状态
    // -----------------------------
    private float timer = 0f;
    public bool levelCompleted = false; // 是否达到 maxScore
    private bool collidedThisFrame = false;
    private bool collisionLocked = false;
    
    protected bool isStart = false;

    public bool IsStart
    {
        get => isStart;
        set => isStart = value;
    }
    
    // -----------------------------
    // GUIStyle 只创建一次
    // -----------------------------
    private GUIStyle scoreStyle;
    private GUIStyle centerStyle;

    // -----------------------------
    // 静态变量，保证只绘制一次
    // -----------------------------
    private static ScoreAndCollisionLevel2 mainInstance;

    void Awake()
    {
        if (mainInstance == null)
            mainInstance = this;
    }

    void Start()
    {
        // 初始化 GUIStyle
        scoreStyle = new GUIStyle();
        scoreStyle.fontSize = 24;
        scoreStyle.normal.textColor = Color.white;
        scoreStyle.fontStyle = FontStyle.Bold;

        centerStyle = new GUIStyle();
        centerStyle.fontSize = 48;
        centerStyle.normal.textColor = Color.green;
        centerStyle.fontStyle = FontStyle.Bold;
        centerStyle.alignment = TextAnchor.MiddleCenter;
        
        globalVolumeControl = FindObjectOfType<GlobalVolumeControl>();
    }

    void Update()
    {
        if (levelCompleted || !isStart) return;

        // 无碰撞计时
        if (!collidedThisFrame)
        {
            timer += Time.deltaTime;
            if (timer >= requiredNoHitTime)
            {
                AddScore(rewardOnNoCollision);
                timer = 0f;
            }
        }

        // 碰撞扣分，只执行一次
        if (collidedThisFrame && !collisionLocked)
        {
            Debug.Log("ScoreAndCollisionLevel2 :" + this.name);
            ReduceScore(penaltyOnCollision);
            collisionLocked = true;
            timer = 0f;
            
            if(globalVolumeControl) globalVolumeControl.Shark();
        }

        collidedThisFrame = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collidedThisFrame = true;
        collisionLocked = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        collidedThisFrame = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionLocked = false;
    }

    public void AddScore(int amount)
    {
        if (levelCompleted) return;

        score += amount;
        if (score > maxScore) score = maxScore;

        if (score >= maxScore)
            ReachMaxScore();
    }

    public void ReduceScore(int amount)
    {
        if (levelCompleted) return;

        score -= amount;
        if (score < 0) score = 0;
    }

    private void ReachMaxScore()
    {
        levelCompleted = true;
        
        onScoreMaxComplte?.Invoke();
    }

    // -----------------------------
    // OnGUI 只由主实例绘制
    // -----------------------------
    void OnGUI()
    {
        if (mainInstance != this) return;

        if (debugMode)
        {
            // 左上角分数
            GUI.Label(new Rect(100, 100, 300, 30), "Score: " + score, scoreStyle);
        }

        if (levelCompleted)
        {
            // 屏幕中间 NEXT LEVEL
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 40, 300, 80), "NEXT LEVEL", centerStyle);
        }
    }
}
