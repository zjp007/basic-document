using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wallgen : MonoBehaviour
{
    [Header("预制体设置")]
    [SerializeField] private List<GameObject> prefabList = new List<GameObject>();

    [Header("生成设置")]
    public float spawnInterval = 3f;    // 生成间隔时间(秒)
    public bool randomPrefab = true;    // true = 随机选择; false = 按顺序选择
    public bool autoStart = false;       // 是否自动开始生成
    
    private bool isCreated = false;

    [Header("缩放设置")]
    [Tooltip("生成物体的缩放乘数（1 = 原始大小, 0.5 = 缩小一倍）")]
    public float spawnScale = 0.5f;

    [Header("移动设置")]
    public float moveSpeed = 2f;        // 生成后物体移动速度

    private int currentIndex = 0;       // 当前生成的索引(按顺序模式使用)
    private bool isSpawning = false;    // 是否正在生成

    void Start()
    {
        // if (autoStart)
        // {
        //     StartSpawning();
        // }
    }

    private void FixedUpdate()
    {
        if (GameObject.FindAnyObjectByType<StartTextGUIControl>().IsStartEnd)
        {
            if (!isCreated)
            {
                isCreated =  true;
                autoStart = true;
                StartSpawning();
            }
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning && prefabList.Count > 0)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
        else if (prefabList.Count == 0)
        {
            Debug.LogWarning("预制体列表为空,无法开始生成!");
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnPrefab();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnPrefab()
    {
        if (prefabList.Count == 0) return;

        GameObject prefabToSpawn;

        if (randomPrefab)
        {
            int randomIndex = Random.Range(0, prefabList.Count);
            prefabToSpawn = prefabList[randomIndex];
        }
        else
        {
            prefabToSpawn = prefabList[currentIndex];
            currentIndex = (currentIndex + 1) % prefabList.Count;
        }

        if (prefabToSpawn != null)
        {
            GameObject newObj = Instantiate(prefabToSpawn, transform.position, transform.rotation);

            // 将生成物体缩放为原始大小的 spawnScale（例如 0.5f 即缩小一倍）
            newObj.transform.localScale = prefabToSpawn.transform.localScale * spawnScale;

            newObj.AddComponent<MoveLeft>().speed = moveSpeed; // 添加移动组件
        }
        else
        {
            Debug.LogWarning("预制体列表中存在空引用!");
        }
    }
}

// -----------------------------
// 新增组件：让物体沿x轴负方向移动
// -----------------------------
public class MoveLeft : MonoBehaviour
{
    public float speed = 2f;

    void Update()
    {
        transform.position += Vector3.left * (speed * Time.deltaTime);
    }
}