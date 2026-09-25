using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class bianliang : MonoBehaviour
{
    [Header("基色（只读取色相与饱和度）")]
    public Color baseYellow = new Color(1f, 0.75f, 0f); // 主色（黄）
    [Header("亮度范围（0..1）")]
    [Range(0f, 1f)] public float minBrightness = 0.25f; // 暗黄
    [Range(0f, 1f)] public float maxBrightness = 1.0f;  // 亮黄

    [Header("平滑与自发光")]
    public float lerpSpeed = 5f; // 颜色平滑速度
    public float emissionMin = 0.0f;
    public float emissionMax = 1.0f;

    [Header("分数来源（优先 Inspector 指定，否则自动查找）")]
    public ScoreAndCollisionLevel2 scoreSource;

    private Renderer rend;
    private Material instanceMaterial;
    private SpriteRenderer spriteR;
    private Color initialEmission = Color.black;
    private bool hasEmission = false;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        spriteR = GetComponent<SpriteRenderer>();
        if (rend != null)
        {
            // 实例化材质，避免修改 sharedMaterial（需要在 OnDestroy 中销毁）
            instanceMaterial = rend.material;
            if (instanceMaterial != null && instanceMaterial.HasProperty("_EmissionColor"))
            {
                hasEmission = true;
                initialEmission = instanceMaterial.GetColor("_EmissionColor");
            }
        }
    }

    void Start()
    {
        // 如果在 Inspector 中没有指定，尝试自动查找
        if (scoreSource == null)
        {
            scoreSource = FindObjectOfType<ScoreAndCollisionLevel2>();
        }

        if (scoreSource == null)
            Debug.LogWarning("[bianliang] 未找到 ScoreAndCollisionLevel2，颜色将保持默认。");
    }

    void Update()
    {
        if (scoreSource == null || scoreSource.maxScore <= 0) return;

        // 归一化得分（0..1），参考代码中的 public int score
        float t = Mathf.Clamp01((float)scoreSource.score / scoreSource.maxScore);

        // 从 baseYellow 中提取色相与饱和度，按得分插值亮度（Value）
        Color.RGBToHSV(baseYellow, out float h, out float s, out _);
        float v = Mathf.Lerp(minBrightness, maxBrightness, t);
        Color target = Color.HSVToRGB(h, s, v);

        // 平滑过渡到目标颜色
        float smooth = Time.deltaTime * lerpSpeed;

        // 2D Sprite 支持
        if (spriteR != null)
        {
            spriteR.color = Color.Lerp(spriteR.color, target, smooth);
            return;
        }

        // 3D 材质颜色与自发光
        if (instanceMaterial != null)
        {
            if (instanceMaterial.HasProperty("_Color"))
            {
                Color current = instanceMaterial.GetColor("_Color");
                Color next = Color.Lerp(current, target, smooth);
                instanceMaterial.SetColor("_Color", next);
            }
            else
            {
                instanceMaterial.color = Color.Lerp(instanceMaterial.color, target, smooth);
            }

            if (hasEmission)
            {
                instanceMaterial.EnableKeyword("_EMISSION");
                float e = Mathf.Lerp(emissionMin, emissionMax, t);
                Color emission = target * e;
                Color currentE = instanceMaterial.GetColor("_EmissionColor");
                instanceMaterial.SetColor("_EmissionColor", Color.Lerp(currentE, emission, smooth));
            }
        }
    }

    void OnDestroy()
    {
        // 销毁在 Awake 中由 rend.material 创建的实例化材质，避免内存泄漏
        if (instanceMaterial != null)
        {
            // 在编辑模式下使用 DestroyImmediate 更安全，但运行时 Destroy 足够
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(instanceMaterial);
            else
                Destroy(instanceMaterial);
        }
    }
}