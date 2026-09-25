using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using PrimeTween;

public class Level2SpawnerController : MonoBehaviour
{
    [Header("生成设置")]
    public List<Transform> spawnPoints;        // 12个固定位置
    public GameObject objectPrefab;            // 要生成的物体
    public float spawnDuration = 3f;           // 每个物体显示时间
    public float interval = 1f;                // 每个物体消失后间隔时间

    [Header("绑定对象列表")]
    public List<GameObject> bindableObjects;   // 需要绑定的对象列表(初始隐藏)

    [Header("分数系统")]
    public ScoreAndCollisionLevel2 scoreSystem;

    [Header("倒计时设置")]
    public float levelTime = 60f;

    private GlobalVolumeControl globalVolumeControl;

    private float timer;
    private bool timerStopped = false;
    private bool showGameOver = false;

    private GameObject currentObject;
    private GameObject currentBoundObject;     // 当前绑定的对象

    // GUIStyle
    private GUIStyle timerStyle;
    private GUIStyle centerStyle;
    private GUIStyle scoreStyle;

    void Start()
    {
        timer = levelTime;

        // 初始化 GUIStyle
        timerStyle = new GUIStyle();
        timerStyle.fontSize = 24;
        timerStyle.normal.textColor = Color.yellow;
        timerStyle.fontStyle = FontStyle.Bold;
        timerStyle.alignment = TextAnchor.UpperCenter;

        centerStyle = new GUIStyle();
        centerStyle.fontSize = 48;
        centerStyle.normal.textColor = Color.red;
        centerStyle.fontStyle = FontStyle.Bold;
        centerStyle.alignment = TextAnchor.MiddleCenter;

        scoreStyle = new GUIStyle();
        scoreStyle.fontSize = 24;
        scoreStyle.normal.textColor = Color.white;
        scoreStyle.fontStyle = FontStyle.Bold;
        scoreStyle.alignment = TextAnchor.UpperLeft;

        if (scoreSystem == null)
            scoreSystem = FindObjectOfType<ScoreAndCollisionLevel2>();

        // 确保所有绑定对象初始隐藏
        foreach (GameObject obj in bindableObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        StartCoroutine(SpawnRoutine());

        globalVolumeControl = FindObjectOfType<GlobalVolumeControl>();
    }

    void Update()
    {
        if (timerStopped) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            timerStopped = true;
            if (scoreSystem != null && scoreSystem.score < scoreSystem.maxScore)
            {
                showGameOver = true;
                scoreSystem.levelCompleted = true;
            }
        }

        if (scoreSystem != null && scoreSystem.score >= scoreSystem.maxScore)
        {
            timerStopped = true;
        }
    }

    // 找到距离指定位置最近的隐藏对象
    GameObject FindNearestHiddenObject(Vector3 position)
    {
        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject obj in bindableObjects)
        {
            if (obj != null && !obj.activeSelf) // 只查找隐藏的对象
            {
                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }
        }

        return nearest;
    }

    IEnumerator SpawnRoutine()
    {
        while (!timerStopped)
        {
            // 随机选择一个位置
            if (spawnPoints.Count == 0 || objectPrefab == null) yield break;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            currentObject = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);

            // 找到最近的隐藏对象并激活
            currentBoundObject = FindNearestHiddenObject(spawnPoint.position);
            if (currentBoundObject != null)
            {
                currentBoundObject.SetActive(true);
            }

            // 等待显示时间
            float elapsed = 0f;
            bool touched = false;

            while (elapsed < spawnDuration)
            {
                elapsed += Time.deltaTime;

                // 检测碰撞逻辑可在objectPrefab上挂脚本处理触碰加分
                // 这里可通过触发事件设置 touched = true

                yield return null;
            }

            // 物体消失时判断是否触碰
            if (!touched && scoreSystem != null)
            {
                scoreSystem.ReduceScore(5); // 未触碰扣分

                globalVolumeControl.Shark();
            }

            // 销毁Cube并隐藏绑定对象
            Destroy(currentObject);
            if (currentBoundObject != null)
            {
                currentBoundObject.SetActive(false);
            }

            // 等待间隔
            float intervalElapsed = 0f;
            while (intervalElapsed < interval)
            {
                intervalElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    void OnGUI()
    {
        // // 显示倒计时
        // int minutes = Mathf.FloorToInt(timer / 60f);
        // int seconds = Mathf.FloorToInt(timer % 60f);
        // string timerText = string.Format("{0:00}:{1:00}", minutes, seconds);
        // GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 30), timerText, timerStyle);
        //
        // // 显示分数
        // if (scoreSystem != null)
        //     GUI.Label(new Rect(10, 10, 200, 30), "Score: " + scoreSystem.score, scoreStyle);
        //
        // // 显示 Game Over
        // if (showGameOver)
        // {
        //     GUI.Label(new Rect(0, Screen.height / 2 - 40, Screen.width, 80), "GAME OVER", centerStyle);
        // }
        //
        // // 显示 Next Level
        // if (scoreSystem != null && scoreSystem.score >= scoreSystem.maxScore)
        // {
        //     GUI.Label(new Rect(0, Screen.height / 2 - 40, Screen.width, 80), "NEXT LEVEL", centerStyle);
        // }
    }
}