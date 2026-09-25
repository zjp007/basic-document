using UnityEngine;

public class detectiondistance : MonoBehaviour
{
    [Header("Ŀ�꣨��ң�")]
    public Transform player; // ���� Inspector ָ������Ϊ������ʹ�� Main Camera��

    [Header("���Ƶ��")]
    public float checkInterval = 0.5f; // ÿ����������һ�Σ�����ÿ֡�ظ��ӷ�

    [Header("�÷�����")]
    public int scoreBetween0_2And1 = 10; // 0.2m < distance < 1.0m �÷�
    public int scoreLessThan0_2 = 15;    // distance < 0.2m �÷�

    [Header("����")]
    public bool debugLog = false;

    private ScoreAndCollision scoreManager;
    private float timer = 0f;

    void Start()
    {
        if (player == null && Camera.main != null)
            player = Camera.main.transform;

        scoreManager = FindObjectOfType<ScoreAndCollision>();
        if (scoreManager == null && debugLog)
            Debug.LogWarning("[detectiondistance] δ�ҵ� ScoreAndCollision ʵ�������ڳ��������ӻ��ֶ���ֵ��");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckAndScoreByDistance();
        }
    }

    void CheckAndScoreByDistance()
    {
        if (player == null)
        {
            if (debugLog) Debug.LogWarning("[detectiondistance] player δ���ã�������⡣");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (debugLog) Debug.Log($"[detectiondistance] ����: {distance:F3} m");

        // ���� < 0.2m => +15
        if (distance < 0.2f)
        {
            scoreManager?.AddScore(scoreLessThan0_2);
            if (debugLog) Debug.Log($"���� < 0.2m���÷� +{scoreLessThan0_2}");
            return;
        }

        // 0.2m < ���� < 1.0m => +10
        if (distance > 0.2f && distance < 1.0f)
        {
            scoreManager?.AddScore(scoreBetween0_2And1);
            if (debugLog) Debug.Log($"0.2m < ���� < 1.0m���÷� +{scoreBetween0_2And1}");
            return;
        }

        // ���� >= 1.0m�����ӷ�
    }
}