using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

[RequireComponent(typeof(Collider))]
public class ScoreAndCollision : MonoBehaviour
{
    // Score settings
    public int score = 0;
    public int maxScore = 100;
    public bool debugMode = true;
    [Header("是否开始更新分数")]
    protected bool isStartFlag = false;
    
    public bool IsStartFlag => isStartFlag;

    [Header("当前关卡结束显示的图像")]
    public Image EndImg;
    private bool isEndShow = false;

    // No-collision score settings
    public float requiredNoHitTime = 3f;
    public int rewardOnNoCollision = 10;
    public int penaltyOnCollision = 5;

    private float timer = 0f;
    private bool levelCompleted = false;
    
    public bool LevelCompleted => levelCompleted;

    private bool collidedThisFrame = false;
    private bool collisionLocked = false;

    // -----------------------------
    // GUIStyle 只创建一次
    // -----------------------------
    private GUIStyle scoreStyle;
    private GUIStyle centerStyle;
    public Wallgen _w;

    // -----------------------------
    // 静态变量，保证只绘制一次
    // -----------------------------
    private static ScoreAndCollision mainInstance;

    void Awake()
    {
        // 如果没有主实例，就设自己为主实例
        if (mainInstance == null)
            mainInstance = this;
    }

    void Start()
    {
        _w = FindObjectOfType<Wallgen>();
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
    }

    void Update()
    {
        if (levelCompleted || !isStartFlag) return;

        if (!collidedThisFrame)
        {
            timer += Time.deltaTime;

            if (timer >= requiredNoHitTime)
            {
                AddScore(rewardOnNoCollision);
                timer = 0f;
            }
        }

        if (collidedThisFrame && !collisionLocked)
        {
            Debug.Log("ScoreAndCollision: " + this.name);
            ReduceScore(penaltyOnCollision);
            collisionLocked = true;
            timer = 0f;
        }

        collidedThisFrame = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collidedThisFrame = true;
            collisionLocked = false;
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) collidedThisFrame = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) collisionLocked = false;
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

    public void SetIsStartFlag(bool isStartFlag)
    {
        this.isStartFlag = isStartFlag;
    }

    private void ReachMaxScore()
    {
        levelCompleted = true;
    }

    // -----------------------------
    // OnGUI 只由主实例绘制
    // -----------------------------
    void OnGUI()
    {
        if (mainInstance != this) return; // 不是主实例就不绘制

        if (debugMode)
        {
            Debug.Log("ScoreAndCollision: " + this.name);
            GUI.Label(new Rect(100, 100, 300, 30), "Score: " + score, scoreStyle);
        }

        if (levelCompleted)
        {
            if (isEndShow)
            {
                if(EndImg.enabled) GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 40, 300, 80), "NEXT LEVEL", centerStyle);
                return;
            }
            if (EndImg)
            {
                isEndShow = true;
                EndImg.enabled = true;
                Tween.Custom(0.0f, 1.0f, 1, onValueChange: x => EndImg.color = new Color(1, 1, 1, x))
                    .OnComplete(target: this, target =>
                    {
                        if(target._w) target._w.StopSpawning();
                        
                        MoveLeft[] monsters = FindObjectsOfType<MoveLeft>();
                        foreach (MoveLeft monster in monsters)
                        {
                            monster.speed = 0;
                        }
                    });
            }
        }
    }
}
