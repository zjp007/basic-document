using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position_StateMachine : MonoBehaviour
{
    [Header("子物体顺序配置")]
    [Tooltip("按照此列表顺序依次激活子物体")]
    public List<Transform> activationSequence = new List<Transform>();

    [Header("切换设置")]
    [Tooltip("切换间隔时间（秒）")]
    public float switchInterval = 3f;

    [Tooltip("是否循环播放")]
    public bool loopSequence = true;

    [Tooltip("是否在开始时立即激活第一个")]
    public bool activateFirstImmediately = false;

    [Header("自动填充")]
    [Tooltip("勾选后会在 Start 时自动按层级顺序填充列表")]
    public bool autoFillFromChildren = false;

    [Header("调试信息")]
    [Tooltip("显示调试日志")]
    public bool showDebugLog = true;

    private int currentSequenceIndex = -1; // 当前在序列中的索引
    private bool hasFinishedSequence = false;
    private Coroutine switchCoroutine;

    private void Start()
    {
        // 如果启用自动填充,则从子物体填充列表
        if (autoFillFromChildren && activationSequence.Count == 0)
        {
            AutoFillSequenceFromChildren();
        }

        // 验证序列列表
        if (activationSequence.Count == 0)
        {
            Debug.LogWarning("[Position_StateMachine] 激活序列列表为空！请在 Inspector 中添加子物体或启用自动填充。");
            return;
        }

        // 移除列表中的 null 项
        activationSequence.RemoveAll(item => item == null);

        if (activationSequence.Count == 0)
        {
            Debug.LogWarning("[Position_StateMachine] 激活序列列表中没有有效的物体！");
            return;
        }

        if (showDebugLog)
        {
            Debug.Log($"[Position_StateMachine] 激活序列包含 {activationSequence.Count} 个物体:");
            for (int i = 0; i < activationSequence.Count; i++)
            {
                Debug.Log($"  [{i}] {activationSequence[i].name}");
            }
        }

        // 一开始将序列中所有物体设为未激活状态
        DeactivateAllInSequence();

        // 如果设置了立即激活第一个
        if (activateFirstImmediately)
        {
            ActivateNext();
        }

        // 开始按顺序切换的协程
        switchCoroutine = StartCoroutine(SwitchSequentially());
    }

    /// <summary>
    /// 从子物体自动填充激活序列
    /// </summary>
    private void AutoFillSequenceFromChildren()
    {
        activationSequence.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            activationSequence.Add(transform.GetChild(i));
        }

        if (showDebugLog)
        {
            Debug.Log($"[Position_StateMachine] 自动填充了 {activationSequence.Count} 个子物体到序列中");
        }
    }

    /// <summary>
    /// 禁用序列中的所有物体
    /// </summary>
    private void DeactivateAllInSequence()
    {
        foreach (Transform obj in activationSequence)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 按顺序切换的协程
    /// </summary>
    private IEnumerator SwitchSequentially()
    {
        // 如果已经立即激活了第一个,则先等待一次
        if (activateFirstImmediately)
        {
            yield return new WaitForSeconds(switchInterval);
        }

        while (true)
        {
            // 如果序列已完成且不循环,则停止
            if (hasFinishedSequence && !loopSequence)
            {
                if (showDebugLog)
                {
                    Debug.Log("[Position_StateMachine] 序列已完成,停止切换");
                }
                yield break;
            }

            // 等待指定时间
            yield return new WaitForSeconds(switchInterval);

            // 激活下一个
            ActivateNext();
        }
    }

    /// <summary>
    /// 激活序列中的下一个物体
    /// </summary>
    private void ActivateNext()
    {
        if (activationSequence.Count == 0) return;

        // 禁用当前激活的物体(如果有)
        if (currentSequenceIndex >= 0 && currentSequenceIndex < activationSequence.Count)
        {
            Transform current = activationSequence[currentSequenceIndex];
            if (current != null)
            {
                current.gameObject.SetActive(false);
                if (showDebugLog)
                {
                    Debug.Log($"[Position_StateMachine] 禁用: [{currentSequenceIndex}] {current.name}");
                }
            }
        }

        // 移动到下一个索引
        currentSequenceIndex++;

        // 检查是否到达末尾
        if (currentSequenceIndex >= activationSequence.Count)
        {
            if (loopSequence)
            {
                // 循环播放,回到开头
                currentSequenceIndex = 0;
                if (showDebugLog)
                {
                    Debug.Log("[Position_StateMachine] ━━━ 循环重新开始 ━━━");
                }
            }
            else
            {
                // 不循环,标记序列完成
                hasFinishedSequence = true;
                currentSequenceIndex = -1;
                if (showDebugLog)
                {
                    Debug.Log("[Position_StateMachine] ✓ 序列播放完成");
                }
                return;
            }
        }

        // 激活新的物体
        Transform next = activationSequence[currentSequenceIndex];
        if (next != null)
        {
            next.gameObject.SetActive(true);
            if (showDebugLog)
            {
                Debug.Log($"[Position_StateMachine] ► 激活: [{currentSequenceIndex}] {next.name}");
            }
        }
        else
        {
            Debug.LogWarning($"[Position_StateMachine] 序列索引 {currentSequenceIndex} 的物体为 null!");
        }
    }

    #region 公共控制方法

    /// <summary>
    /// 手动激活序列中指定索引的物体
    /// </summary>
    public void ActivateBySequenceIndex(int index)
    {
        if (index < 0 || index >= activationSequence.Count)
        {
            Debug.LogWarning($"[Position_StateMachine] 索引 {index} 超出范围! 有效范围: 0-{activationSequence.Count - 1}");
            return;
        }

        // 禁用当前激活的物体
        if (currentSequenceIndex >= 0 && currentSequenceIndex < activationSequence.Count)
        {
            Transform current = activationSequence[currentSequenceIndex];
            if (current != null)
            {
                current.gameObject.SetActive(false);
            }
        }

        // 激活指定索引的物体
        currentSequenceIndex = index;
        Transform target = activationSequence[currentSequenceIndex];
        if (target != null)
        {
            target.gameObject.SetActive(true);
            if (showDebugLog)
            {
                Debug.Log($"[Position_StateMachine] 手动激活: [{index}] {target.name}");
            }
        }
    }

    /// <summary>
    /// 手动激活下一个物体
    /// </summary>
    public void ManualNext()
    {
        ActivateNext();
    }

    /// <summary>
    /// 重置序列到开头
    /// </summary>
    public void ResetSequence()
    {
        // 禁用当前激活的物体
        if (currentSequenceIndex >= 0 && currentSequenceIndex < activationSequence.Count)
        {
            Transform current = activationSequence[currentSequenceIndex];
            if (current != null)
            {
                current.gameObject.SetActive(false);
            }
        }

        currentSequenceIndex = -1;
        hasFinishedSequence = false;

        if (showDebugLog)
        {
            Debug.Log("[Position_StateMachine] 序列已重置");
        }
    }

    /// <summary>
    /// 停止自动切换
    /// </summary>
    public void StopAutoSwitch()
    {
        if (switchCoroutine != null)
        {
            StopCoroutine(switchCoroutine);
            switchCoroutine = null;
            if (showDebugLog)
            {
                Debug.Log("[Position_StateMachine] 已停止自动切换");
            }
        }
    }

    /// <summary>
    /// 重新开始自动切换
    /// </summary>
    public void StartAutoSwitch()
    {
        if (switchCoroutine != null)
        {
            StopCoroutine(switchCoroutine);
        }
        switchCoroutine = StartCoroutine(SwitchSequentially());
        if (showDebugLog)
        {
            Debug.Log("[Position_StateMachine] 已重新开始自动切换");
        }
    }

    /// <summary>
    /// 获取当前激活物体的序列索引
    /// </summary>
    public int GetCurrentSequenceIndex()
    {
        return currentSequenceIndex;
    }

    /// <summary>
    /// 获取序列长度
    /// </summary>
    public int GetSequenceLength()
    {
        return activationSequence.Count;
    }

    /// <summary>
    /// 获取当前激活的物体
    /// </summary>
    public Transform GetCurrentActive()
    {
        if (currentSequenceIndex >= 0 && currentSequenceIndex < activationSequence.Count)
        {
            return activationSequence[currentSequenceIndex];
        }
        return null;
    }

    /// <summary>
    /// 添加物体到序列末尾
    /// </summary>
    public void AddToSequence(Transform obj)
    {
        if (obj != null && !activationSequence.Contains(obj))
        {
            activationSequence.Add(obj);
            if (showDebugLog)
            {
                Debug.Log($"[Position_StateMachine] 已添加 {obj.name} 到序列");
            }
        }
    }

    /// <summary>
    /// 从序列中移除物体
    /// </summary>
    public void RemoveFromSequence(Transform obj)
    {
        if (activationSequence.Remove(obj))
        {
            if (showDebugLog)
            {
                Debug.Log($"[Position_StateMachine] 已从序列中移除 {obj.name}");
            }
        }
    }

    #endregion

    #region Editor 辅助方法

    /// <summary>
    /// 在 Inspector 中右键菜单: 自动填充子物体
    /// </summary>
    [ContextMenu("自动填充子物体到序列")]
    private void MenuAutoFill()
    {
        AutoFillSequenceFromChildren();
        Debug.Log($"[Position_StateMachine] 已自动填充 {activationSequence.Count} 个子物体");
    }

    /// <summary>
    /// 在 Inspector 中右键菜单: 清空序列
    /// </summary>
    [ContextMenu("清空激活序列")]
    private void MenuClearSequence()
    {
        activationSequence.Clear();
        Debug.Log("[Position_StateMachine] 激活序列已清空");
    }

    /// <summary>
    /// 在 Inspector 中右键菜单: 显示序列信息
    /// </summary>
    [ContextMenu("显示序列信息")]
    private void MenuShowSequenceInfo()
    {
        Debug.Log($"=== 激活序列信息 ===");
        Debug.Log($"序列长度: {activationSequence.Count}");
        Debug.Log($"当前索引: {currentSequenceIndex}");
        Debug.Log($"循环模式: {(loopSequence ? "开启" : "关闭")}");
        Debug.Log($"切换间隔: {switchInterval} 秒");
        Debug.Log($"序列内容:");
        for (int i = 0; i < activationSequence.Count; i++)
        {
            if (activationSequence[i] != null)
            {
                string marker = (i == currentSequenceIndex) ? "► " : "  ";
                Debug.Log($"{marker}[{i}] {activationSequence[i].name}");
            }
            else
            {
                Debug.Log($"  [{i}] (null)");
            }
        }
    }

    #endregion
}