using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using PrimeTween;

[DisallowMultipleComponent]
public class Cube_Score : MonoBehaviour
{
    [Header("Player 设置")]
    [Tooltip("未指定时会尝试使用 Tag 为 'Player' 的对象")]
    public Transform player;

    [Header("检测设置")]
    [Tooltip("激活后等待的秒数")]
    public float checkDelay = 0.99f;
    [Tooltip("阈值距离 (小于等于 为加分，否则扣分)")]
    public float thresholdDistance = 2f;

    [Header("得分设置（仅在未使用 Inspector 事件时生效）")]
    public int rewardAmount = 1;
    public int penaltyAmount = 1;

    [Header("选择要修改的分数维度（仅在未使用 Inspector 事件时生效）")]
    public ScoreDimension targetDimension = ScoreDimension.A;

    [Header("Inspector 可绑定事件")]
    [Tooltip("当检测到在阈值内（视为加分）时触发，可在 Inspector 中像 UI Button 的 OnClick 一样绑定方法")]
    public UnityEvent onWithinThreshold;
    [Tooltip("当检测到超出阈值（视为扣分）时触发，可在 Inspector 中像 UI Button 的 OnClick 一样绑定方法")]
    public UnityEvent onOutsideThreshold;

    [Header("行为选项")]
    [Tooltip("如果勾选：仅触发 Inspector 事件（onWithinThreshold / onOutsideThreshold）。\n未勾选：仍然仅触发 Inspector 事件，但 Console 会给出提示（为避免依赖外部 PublicInt 代码）。")]
    public bool useInspectorEventsOnly = true;

    // 内部状态：表示上一次检测结果（true = 在阈值内 => 加分；false = 超出阈值 => 扣分）
    private bool lastCheckWasWithinThreshold = false;

    public enum ScoreDimension
    {
        A,
        B,
        C,
        D,
        E,
        Total,    // 总分（会按总分变化，使用 Add 到 Total 的语义不适用时请选单项）
        Average   // 平均值（同上）
    }

    private Coroutine checkCoroutine = null;
    
    private GlobalVolumeControl globalVolumeControl;

    void Awake()
    {
        // 确保 UnityEvent 不为 null（便于在运行时安全触发）
        if (onWithinThreshold == null) onWithinThreshold = new UnityEvent();
        if (onOutsideThreshold == null) onOutsideThreshold = new UnityEvent();
    }

    void OnEnable()
    {
        // 开始一次延迟检测
        if (checkCoroutine != null)
            StopCoroutine(checkCoroutine);
        checkCoroutine = StartCoroutine(CheckDistanceAfterDelay());
    }

    private void Start()
    {
        globalVolumeControl = FindObjectOfType<GlobalVolumeControl>();
    }

    void OnDisable()
    {
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            checkCoroutine = null;
        }
    }

    private IEnumerator CheckDistanceAfterDelay()
    {
        yield return new WaitForSeconds(checkDelay);

        // 找到 player（如果未手动指定）
        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null)
                player = go.transform;
        }

        if (player == null)
        {
            // 尝试查找场景中的第一个包含 "Player" 名称的 Transform（兜底）
            var allTransforms = FindObjectsOfType<Transform>();
            foreach (var t in allTransforms)
            {
                if (t.name.ToLower().Contains("player"))
                {
                    player = t;
                    break;
                }
            }
        }

        if (player == null)
        {
            Debug.LogError($"[Cube_Score] 未找到 Player，请在 Inspector 指定或为 Player 设置 Tag: 'Player'。物体: {gameObject.name}", gameObject);
            yield break;
        }

        // 计算距离并记录结果
        float dist = Vector3.Distance(transform.position, player.position);
        lastCheckWasWithinThreshold = dist <= thresholdDistance;
        
        // 屏幕闪烁
        if (!lastCheckWasWithinThreshold && globalVolumeControl)
        {
            globalVolumeControl.Shark();
        }
        
        Debug.Log($"[Cube_Score] 检测完成，距离: {dist:F2}, 阈值: {thresholdDistance:F2}, 结果: {(lastCheckWasWithinThreshold ? "加分" : "扣分")}");

        // 表现为 Onclick()：调用统一入口方法（便于 Inspector 绑定或代码调用）
        Onclick();

        checkCoroutine = null;
    }

    /// <summary>
    /// 表现为 Onclick()：根据上一次检测结果触发 Inspector 事件。
    /// 已移除对外部 PublicInt 的直接引用，避免依赖其它代码（编译错误）。
    /// 可直接在 Inspector 将该方法绑定到 Button 的 OnClick。
    /// </summary>
    public void Onclick()
    {
        // 始终通过 Inspector 事件驱动加/扣分行为，避免依赖项目中可能不存在的 PublicInt 接口
        if (lastCheckWasWithinThreshold)
            onWithinThreshold?.Invoke();
        else
            onOutsideThreshold?.Invoke();

        // 当用户希望非仅 Inspector 事件时，给出提示（不做额外修改以保持无依赖）
        if (!useInspectorEventsOnly)
        {
            Debug.LogWarning("[Cube_Score] useInspectorEventsOnly 为 false，但脚本已移除对外部分数 API 的直接调用。请在 Inspector 中绑定 onWithinThreshold/onOutsideThreshold 来处理加/扣分逻辑。", gameObject);
        }

        Debug.Log($"[Cube_Score] (Inspector 事件) 触发: {(lastCheckWasWithinThreshold ? "onWithinThreshold" : "onOutsideThreshold")}，物体: {gameObject.name}");
    }
}