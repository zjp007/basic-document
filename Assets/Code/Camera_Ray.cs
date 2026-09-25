using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Ray : MonoBehaviour
{
    [Header("检测设置")]
    [Tooltip("目标物体的Tag")]
    public string targetTag = "Cube";

    [Tooltip("持续看向目标多少秒后销毁")]
    public float lookDuration = 0.5f;

    [Tooltip("屏幕边缘缓冲区(0-0.5)，越大则有效区域越小")]
    [Range(0f, 0.5f)]
    public float screenMargin = 0.1f;

    [Header("分数设置")]
    [Tooltip("销毁物体时增加的分数")]
    public int scorePerDestroy = 10;

    [Header("调试设置")]
    [Tooltip("显示详细调试信息")]
    public bool showDetailedDebug = true;

    [Header("引用设置")]
    [Tooltip("分数管理器(如果为空会自动查找)")]
    public ScoreAndCollisionLevel2 scoreManager;

    [Tooltip("用于检测的相机(如果为空会使用主相机)")]
    public Camera detectionCamera;

    private GameObject currentTarget;
    private float lookTimer = 0f;

    void Start()
    {
        // 获取相机
        if (detectionCamera == null)
        {
            detectionCamera = Camera.main;
            if (detectionCamera == null)
            {
                Debug.LogError("未找到相机!");
                return;
            }
        }

        // 如果没有手动指定分数管理器,尝试自动查找
        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreAndCollisionLevel2>();

            if (scoreManager == null)
            {
                Debug.LogWarning("未找到 ScoreAndCollisionLevel2 组件!");
            }
            else
            {
                Debug.Log("成功找到 ScoreAndCollisionLevel2 组件!");
            }
        }

        // 检查场景中是否有目标物体
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        Debug.Log($"场景中找到 {targets.Length} 个 Tag 为 '{targetTag}' 的物体");
    }

    void Update()
    {
        DetectTargetInScreen();
    }

    void DetectTargetInScreen()
    {
        // 查找所有目标物体
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject target in targets)
        {
            if (IsInScreenBounds(target))
            {
                // 找到距离屏幕中心最近的目标
                float distance = Vector3.Distance(detectionCamera.transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }
        }

        // 如果找到屏幕内的目标
        if (closestTarget != null)
        {
            if (currentTarget == closestTarget)
            {
                lookTimer += Time.deltaTime;

                if (showDetailedDebug)
                {
                    Debug.Log($"[瞄准] 目标 {closestTarget.name} 在屏幕内, 时间: {lookTimer:F2}s / {lookDuration}s");
                }

                if (lookTimer >= lookDuration)
                {
                    DestroyTargetAndAddScore(closestTarget);
                }
            }
            else
            {
                currentTarget = closestTarget;
                lookTimer = 0f;

                if (showDetailedDebug)
                {
                    Debug.Log($"[锁定] 锁定新目标: {closestTarget.name}");
                }
            }
            return;
        }

        // 没有找到目标,重置
        if (currentTarget != null && showDetailedDebug)
        {
            Debug.Log("[重置] 目标离开屏幕,重置计时器");
        }

        currentTarget = null;
        lookTimer = 0f;
    }

    /// <summary>
    /// 检查物体是否在屏幕范围内
    /// </summary>
    bool IsInScreenBounds(GameObject target)
    {
        Vector3 viewportPos = detectionCamera.WorldToViewportPoint(target.transform.position);

        // 检查是否在相机前方
        if (viewportPos.z <= 0)
            return false;

        // 检查是否在屏幕范围内(考虑边缘缓冲)
        bool inScreen = viewportPos.x >= screenMargin && viewportPos.x <= (1 - screenMargin) &&
                        viewportPos.y >= screenMargin && viewportPos.y <= (1 - screenMargin);

        return inScreen;
    }

    void DestroyTargetAndAddScore(GameObject target)
    {
        Debug.Log($"<color=green>[✓成功] 准备销毁 {target.name},增加 {scorePerDestroy} 分!</color>");

        if (scoreManager != null)
        {
            scoreManager.AddScore(scorePerDestroy);
            Debug.Log($"<color=green>[✓成功] 分数已增加! 当前分数: {scoreManager.score}</color>");
        }
        else
        {
            Debug.LogError("<color=red>[✗失败] ScoreManager 为空,无法增加分数!</color>");
        }

        Destroy(target);
        Debug.Log($"<color=green>[✓成功] {target.name} 已销毁!</color>");

        currentTarget = null;
        lookTimer = 0f;
    }

    void OnGUI()
    {
        // if (!showDetailedDebug) return;
        //
        // GUIStyle style = new GUIStyle();
        // style.fontSize = 18;
        // style.normal.textColor = Color.yellow;
        // style.fontStyle = FontStyle.Bold;
        //
        // string debugInfo = "=== 屏幕范围检测调试 ===\n";
        // debugInfo += $"当前目标: {(currentTarget != null ? currentTarget.name : "无")}\n";
        // debugInfo += $"计时器: {lookTimer:F2}s / {lookDuration}s\n";
        // debugInfo += $"屏幕边缘缓冲: {screenMargin * 100}%";
        //
        // GUI.Label(new Rect(10, 100, 400, 150), debugInfo, style);
    }
}