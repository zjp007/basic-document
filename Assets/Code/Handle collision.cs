using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 启动时将场景中名为 `controller_base` 的物体上的组件（代码与序列化字段）
/// 同步注入到当前物体上。
/// </summary>
public class HandleCollision : MonoBehaviour
{
    [Tooltip("在场景中查找的源手柄物体名（默认 controller_base）")]
    public string sourceObjectName = "controller_base";

    [Tooltip("是否同时复制子对象层级")]
    public bool copyChildren = false;

    [Tooltip("启动时是否自动同步")]
    public bool syncOnStart = true;

    [Tooltip("是否输出详细日志")]
    public bool verboseLog = false;

    void Start()
    {
        if (syncOnStart)
        {
            SyncFromSource();
        }
    }

    /// <summary>
    /// 从 sourceObjectName 指定的对象同步组件到本对象
    /// </summary>
    public void SyncFromSource()
    {
        GameObject src = GameObject.Find(sourceObjectName);
        if (src == null)
        {
            Debug.LogWarning($"[HandleCollision] 未找到 '{sourceObjectName}'");
            return;
        }

        CopyComponents(src, gameObject);

        if (copyChildren)
        {
            CopyChildrenHierarchy(src.transform, transform);
        }

        if (verboseLog)
        {
            Debug.Log("[HandleCollision] 同步完成");
        }
    }

    private void CopyComponents(GameObject src, GameObject dst)
    {
        Component[] components = src.GetComponents<Component>();

        foreach (Component comp in components)
        {
            if (comp == null) continue;
            if (comp is Transform) continue;

            Type type = comp.GetType();
            Component target = dst.GetComponent(type);

            if (target == null)
            {
                try
                {
                    target = dst.AddComponent(type);
                    if (verboseLog)
                        Debug.Log($"添加组件 {type.Name}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"无法添加组件 {type.Name}: {e.Message}");
                    continue;
                }
            }

            CopyInstanceFields(comp, target);
        }
    }

    private void CopyInstanceFields(Component src, Component dst)
    {
        Type type = src.GetType();

        FieldInfo[] fields =
            type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.IsStatic) continue;

            try
            {
                field.SetValue(dst, field.GetValue(src));
            }
            catch
            {
                if (verboseLog)
                    Debug.LogWarning($"字段复制失败: {field.Name}");
            }
        }
    }

    private void CopyChildrenHierarchy(Transform srcRoot, Transform dstRoot)
    {
        for (int i = 0; i < srcRoot.childCount; i++)
        {
            Transform srcChild = srcRoot.GetChild(i);
            Transform dstChild = dstRoot.Find(srcChild.name);

            GameObject newChild;

            if (dstChild == null)
            {
                newChild = new GameObject(srcChild.name);
                newChild.transform.SetParent(dstRoot, false);
                newChild.transform.localPosition = srcChild.localPosition;
                newChild.transform.localRotation = srcChild.localRotation;
                newChild.transform.localScale = srcChild.localScale;
            }
            else
            {
                newChild = dstChild.gameObject;
            }

            CopySerializedComponentIfExists<Renderer>(srcChild.gameObject, newChild);
            CopySerializedComponentIfExists<MeshFilter>(srcChild.gameObject, newChild);
            CopySerializedComponentIfExists<Collider>(srcChild.gameObject, newChild);

            CopyChildrenHierarchy(srcChild, newChild.transform);
        }
    }

    private void CopySerializedComponentIfExists<T>(GameObject src, GameObject dst)
        where T : Component
    {
        T srcComp = src.GetComponent<T>();
        if (srcComp == null) return;

        T dstComp = dst.GetComponent<T>();
        if (dstComp == null)
        {
            try
            {
                dstComp = dst.AddComponent<T>();
            }
            catch
            {
                return;
            }
        }

        CopyInstanceFields(srcComp, dstComp);
    }
}
