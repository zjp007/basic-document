using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testcameracontroller : MonoBehaviour
{
    [Header("鼠标灵敏度设置")]
    [Tooltip("水平方向(左右)灵敏度")]
    public float mouseSensitivityX = 2f;

    [Tooltip("垂直方向(上下)灵敏度")]
    public float mouseSensitivityY = 2f;

    [Header("视角限制")]
    [Tooltip("向上看的最大角度")]
    public float maxLookUpAngle = 80f;

    [Tooltip("向下看的最大角度")]
    public float maxLookDownAngle = 80f;

    [Header("其他设置")]
    [Tooltip("是否锁定光标")]
    public bool lockCursor = true;

    private float rotationX = 0f; // 上下旋转角度(pitch)
    private float rotationY = 0f; // 左右旋转角度(yaw)

    void Start()
    {
        // 锁定并隐藏光标
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        // 左右旋转(水平)
        rotationY += mouseX;

        // 上下旋转(垂直) - 注意是减法,因为鼠标向上移动时Y值为正,但我们要向上看(负角度)
        rotationX -= mouseY;

        // 限制上下视角范围,防止翻转
        rotationX = Mathf.Clamp(rotationX, -maxLookUpAngle, maxLookDownAngle);

        // 应用旋转
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // 按ESC键解锁光标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 点击鼠标重新锁定光标
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}