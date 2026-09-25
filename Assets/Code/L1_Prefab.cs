using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L1_Prefab : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        // 获取组件
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb == null)
        {
            Debug.LogWarning("[L1_Prefab] 未找到 Rigidbody 组件!");
        }

        if (col == null)
        {
            Debug.LogWarning("[L1_Prefab] 未找到 Collider 组件!");
        }

        // 禁用与同类物体的碰撞
        IgnoreSelfCollision();
    }

    void IgnoreSelfCollision()
    {
        // 查找所有相同类型的物体，忽略相互碰撞
        L1_Prefab[] allL1Prefabs = FindObjectsOfType<L1_Prefab>();

        if (col != null)
        {
            foreach (L1_Prefab other in allL1Prefabs)
            {
                if (other != this && other.col != null)
                {
                    Physics.IgnoreCollision(col, other.col);
                }
            }
        }

        Debug.Log($"[L1_Prefab] 已设置忽略 {allL1Prefabs.Length - 1} 个同类物体的碰撞");
    }

    // 只与 Player 标签的对象发生碰撞
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"[L1_Prefab] 与 Player 碰撞: {collision.gameObject.name}");
            // 在这里添加与 Player 碰撞时的逻辑
        }
        else
        {
            // 忽略非 Player 对象的碰撞
            if (col != null && collision.collider != null)
            {
                Physics.IgnoreCollision(col, collision.collider);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // 持续检测，只处理 Player 标签
        if (collision.gameObject.CompareTag("Player"))
        {
            // 在这里添加持续碰撞时的逻辑
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 碰撞结束时的处理
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"[L1_Prefab] Player 离开碰撞: {collision.gameObject.name}");
            // 在这里添加碰撞结束时的逻辑
        }
    }

    // 当生成新的 L1_Prefab 实例时调用（用于运行时生成的预制体）
    public void OnSpawned()
    {
        IgnoreSelfCollision();
    }
}