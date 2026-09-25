using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject xrRig;

    void Start()
    {
        if (xrRig != null && spawnPoint != null)
        {
            xrRig.transform.position = spawnPoint.position;
            xrRig.transform.rotation = spawnPoint.rotation;
        }
    }
}
