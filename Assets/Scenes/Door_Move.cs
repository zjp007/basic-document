using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Move : MonoBehaviour
{
    [Header("门移动设置")]
    public Transform doorAnchor;          // 门的锚点位置（检测玩家距离用）
    public Transform closedAnchor;        // 门关闭时的位置锚点
    public Transform openAnchor;          // 门打开时的位置锚点
    public float moveSpeed = 2f;          // 门移动的速度

    [Header("玩家检测设置")]
    public float detectionRange = 3f;     // 检测玩家的范围
    public Transform player;              // 玩家对象（可选，自动查找）

    private Vector3 closedPosition;       // 门关闭时的位置
    private Vector3 openPosition;         // 门打开时的位置
    private bool isOpen = false;          // 门的状态
    private bool isMoving = false;        // 门是否正在移动

    void Start()
    {
        if (doorAnchor == null)
            doorAnchor = transform;

        if (closedAnchor != null)
            closedPosition = closedAnchor.position;
        else
        {
            closedPosition = transform.position;
            Debug.LogWarning("未设置关闭锚点，使用门的当前位置！");
        }

        if (openAnchor != null)
            openPosition = openAnchor.position;
        else
        {
            openPosition = transform.position + transform.right * 2f;
            Debug.LogWarning("未设置打开锚点，使用默认位置！");
        }

        transform.position = closedPosition;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("未找到tag为'Player'的对象！");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(doorAnchor.position, player.position);

        if (distanceToPlayer <= detectionRange && !isOpen)
            OpenDoor();
        else if (distanceToPlayer > detectionRange && isOpen)
            CloseDoor();
    }

    public void OpenDoor()
    {
        if (!isMoving)
        {
            StopAllCoroutines();
            StartCoroutine(MoveDoor(openPosition));
            isOpen = true;
        }
    }

    public void CloseDoor()
    {
        if (!isMoving)
        {
            StopAllCoroutines();
            StartCoroutine(MoveDoor(closedPosition));
            isOpen = false;
        }
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    private void OnDrawGizmos()
    {
        Transform detectionAnchor = doorAnchor != null ? doorAnchor : transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionAnchor.position, detectionRange);

        if (closedAnchor != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(closedAnchor.position, Vector3.one * 0.3f);
            Gizmos.DrawSphere(closedAnchor.position, 0.15f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(closedAnchor.position + Vector3.up * 0.5f, "关闭位置");
#endif
        }

        if (openAnchor != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(openAnchor.position, Vector3.one * 0.3f);
            Gizmos.DrawSphere(openAnchor.position, 0.15f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(openAnchor.position + Vector3.up * 0.5f, "打开位置");
#endif
        }

        if (closedAnchor != null && openAnchor != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(closedAnchor.position, openAnchor.position);

            Vector3 direction = (openAnchor.position - closedAnchor.position).normalized;
            Vector3 midPoint = (closedAnchor.position + openAnchor.position) / 2f;
            DrawArrow(midPoint, direction, 0.5f);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.25f);
    }

    private void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f)
    {
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * Vector3.forward;

        Gizmos.DrawRay(pos, right * arrowHeadLength);
        Gizmos.DrawRay(pos, left * arrowHeadLength);
    }
}
