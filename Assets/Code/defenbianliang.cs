using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chupengjianfen : MonoBehaviour
{
    [Header("分数与亮度设置")]
    [Tooltip("到达此分数时达到最亮")]
    public float maxScore = 100f;
    [Tooltip("当前分数（可通过脚本 SetScore / AddScore 修改）")]
    public float currentScore = 0f;

    [Header("颜色/发光")]
    [Tooltip("分数最低时的颜色（较暗的黄色）")]
    public Color dimColor = new Color(0.5f, 0.4f, 0f);
    [Tooltip("分数最高时的颜色（明亮的黄色）")]
    public Color brightColor = Color.yellow;
    [Tooltip("最小发光强度（0 = 无发光）")]
    public float minEmission = 0f;
    [Tooltip("最大发光强度")]
    public float maxEmission = 2f;
    [Tooltip("是否使用材质发光（适用于 3D MeshRenderer）")]
    public bool useEmission = true;

    [Tooltip("如果为 true，会每帧基于 currentScore 更新；否则请通过 SetScore / AddScore 更新")]
    public bool updateEveryFrame = false;

    Renderer[] renderers;
    SpriteRenderer[] spriteRenderers;

    void Start()
    {
        // 获取渲染组件（支持 3D 与 2D）
        renderers = GetComponentsInChildren<Renderer>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 如果使用发光，确保材质开启了 Emission 关键字（每个 Renderer 的材质实例化）
        if (useEmission)
        {
            foreach (var r in renderers)
            {
                var mats = r.materials;
                foreach (var m in mats)
                {
                    m.EnableKeyword("_EMISSION");
                }
            }
        }

        // 初始一次更新外观
        UpdateAppearance();
    }

    void Update()
    {
        if (updateEveryFrame)
            UpdateAppearance();
    }

    // 将分数设置为指定值并立即更新外观
    public void SetScore(float score)
    {
        currentScore = score;
        UpdateAppearance();
    }

    // 增加分数并更新
    public void AddScore(float delta)
    {
        SetScore(currentScore + delta);
    }

    void UpdateAppearance()
    {
        // 规范化 0..1
        float t = (maxScore > 0f) ? Mathf.Clamp01(currentScore / maxScore) : 0f;

        // 颜色插值（暗 -> 亮）
        Color targetColor = Color.Lerp(dimColor, brightColor, t);

        // 计算发光颜色（用颜色乘以强度）
        float emissionIntensity = Mathf.Lerp(minEmission, maxEmission, t);
        Color emissionColor = targetColor * Mathf.LinearToGammaSpace(emissionIntensity);

        // 更新 SpriteRenderer（2D）
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = targetColor;
        }

        // 更新 3D Renderer 的材质颜色和发光（注意：使用 material(s) 会实例化材质）
        for (int i = 0; i < renderers.Length; i++)
        {
            var mats = renderers[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                var m = mats[j];
                // 常规颜色（若材质存在 _Color 属性）
                if (m.HasProperty("_Color"))
                    m.SetColor("_Color", targetColor);
                // 发光（若材质支持 _EmissionColor）
                if (useEmission && m.HasProperty("_EmissionColor"))
                {
                    m.SetColor("_EmissionColor", emissionColor);
                }
            }
        }
    }
}