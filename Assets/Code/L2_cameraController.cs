using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2_cameraController : MonoBehaviour
{
    [Header("鼠标灵敏度")]
    public float mouseSensitivityX = 2f; // 左右旋转灵敏度
    public float mouseSensitivityY = 2f; // 上下旋转灵敏度

    [Header("视角限制")]
    public float maxLookUpAngle = 80f;   // 抬头最大角度
    public float maxLookDownAngle = 80f; // 低头最大角度

    [Header("第一人称设置")]
    [Tooltip("玩家身体对象（拖拽Player父物体到这里）。\n如果不填，则只旋转相机自身。")]
    public Transform playerBody;

    [Header("平滑设置")]
    public bool smoothRotation = false;  // 是否开启平滑
    public float smoothSpeed = 15f;      // 平滑速度

    [Header("光标设置")]
    public bool lockCursor = true;       // 是否在开始时锁定光标

    // 内部变量
    private float rotationX = 0f; // 记录当前的 X 轴旋转 (Pitch)
    private float rotationY = 0f; // 记录当前的 Y 轴旋转 (Yaw)

    private float targetRotationX = 0f;
    private float targetRotationY = 0f;

    void Start()
    {
        // 1. 初始化角度，防止视角跳变
        Vector3 currentRot = transform.localEulerAngles;
        rotationX = currentRot.x;

        // 处理 Unity 角度 0-360 的问题，转换为 -180 到 180
        if (rotationX > 180) rotationX -= 360;

        // 如果有身体，Y轴角度以身体为准；否则以相机为准
        if (playerBody != null)
            rotationY = playerBody.localEulerAngles.y;
        else
            rotationY = currentRot.y;

        // 初始化目标角度
        targetRotationX = rotationX;
        targetRotationY = rotationY;

        // 2. 锁定光标
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // 处理光标锁定/解锁逻辑
        HandleCursorLock();

        // 只有光标锁定时才允许旋转视角
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleMouseLook();
        }
    }

    // 在 LateUpdate 中应用旋转，防止相机抖动
    void LateUpdate()
    {
        ApplyRotation();
    }

    void HandleMouseLook()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        // 计算目标角度
        // Y轴（左右）：鼠标左右移动 -> 增加/减少 Y 角度
        targetRotationY += mouseX;

        // X轴（上下）：鼠标向上移动 -> 减少 X 角度（抬头是负角度）
        targetRotationX -= mouseY;

        // 限制抬头低头范围
        targetRotationX = Mathf.Clamp(targetRotationX, -maxLookUpAngle, maxLookDownAngle);

        // 如果不开启平滑，直接更新当前角度
        if (!smoothRotation)
        {
            rotationX = targetRotationX;
            rotationY = targetRotationY;
        }
    }

    void ApplyRotation()
    {
        // 平滑插值逻辑
        if (smoothRotation)
        {
            rotationX = Mathf.Lerp(rotationX, targetRotationX, Time.deltaTime * smoothSpeed);
            rotationY = Mathf.Lerp(rotationY, targetRotationY, Time.deltaTime * smoothSpeed);
        }

        // 核心逻辑：分离旋转
        if (playerBody != null)
        {
            // 1. 左右旋转应用给【身体】(Player Body)
            // 这样身体正面会跟着鼠标转，按 W 前进时方向才正确
            playerBody.localRotation = Quaternion.Euler(0f, rotationY, 0f);

            // 2. 上下旋转应用给【相机】(Camera)
            // 身体不应该上下倾斜，只有头（相机）动
            transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
        else
        {
            // 如果没有身体引用，相机自己处理所有旋转（类似飞行模式或观察模式）
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
    }

    void HandleCursorLock()
    {
        // 按 ESC 解锁
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 点击鼠标左键重新锁定
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