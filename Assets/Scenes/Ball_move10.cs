using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_move10 : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;      // 移动速度

    private Rigidbody rb;             // 刚体组件（如果使用物理）

    // Start is called before the first frame update
    void Start()
    {
        // 尝试获取刚体组件
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // 获取WASD输入
        float horizontal = Input.GetAxis("Horizontal"); // A/D 或 左右箭头
        float vertical = Input.GetAxis("Vertical");     // W/S 或 上下箭头

        // 创建移动向量
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // 如果有刚体组件，使用物理移动
        if (rb != null)
        {
            // 使用速度移动
            rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
        }
        else
        {
            // 没有刚体则直接移动Transform
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}