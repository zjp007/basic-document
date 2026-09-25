using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用左右键（或 A/D）控制物体在 X 轴移动。
/// 简单、无依赖，适合直接挂到需要控制的 GameObject 上。
/// </summary>
public class SimpleCpntroller : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("移动速度（单位：单位/秒）")]
    public float moveSpeed = 5f;

    [Tooltip("是否启用 X 轴边界限制")]
    public bool useBounds = false;

    [Tooltip("X 轴最小值（启用边界时生效）")]
    public float minX = -5f;

    [Tooltip("X 轴最大值（启用边界时生效）")]
    public float maxX = 5f;

    [Tooltip("是否使用物理刚体移动（若物体上有 Rigidbody 建议开启以避免穿透）")]
    public bool useRigidbody = false;

    private Rigidbody rb;

    void Awake()
    {
        if (useRigidbody)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning($"[SimpleCpntroller] useRigidbody 为 true，但未找到 Rigidbody，将改为非物理移动。物体: {gameObject.name}", gameObject);
                useRigidbody = false;
            }
            else
            {
                // 如果使用刚体，禁用重力对水平移动的影响（仅建议）
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    void Update()
    {
        // 获取水平输入：左右箭头 或 A/D，范围 [-1, 1]
        float h = Input.GetAxisRaw("Horizontal"); // 使用 GetAxisRaw 保持响应性

        if (Mathf.Approximately(h, 0f))
            return;

        Vector3 delta = Vector3.right * h * moveSpeed * Time.deltaTime;
        if (useRigidbody && rb != null)
        {
            // 将移动放到 FixedUpdate 更合适，但为简单起见这里用 MovePosition（仍在 Update 中计算目标）
            Vector3 target = rb.position + delta;
            if (useBounds)
                target.x = Mathf.Clamp(target.x, minX, maxX);
            rb.MovePosition(target);
        }
        else
        {
            Vector3 pos = transform.position + delta;
            if (useBounds)
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }
    }
}