using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levelmanager : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // 在开始时禁用列表中的所有对象
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // 启动协程,2秒后激活所有对象
        StartCoroutine(ActivateObjectsAfterDelay(2f));
    }

    // 延迟激活对象的协程
    private IEnumerator ActivateObjectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 激活列表中的所有对象
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}