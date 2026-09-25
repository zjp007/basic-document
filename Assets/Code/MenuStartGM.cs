using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStartGM : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Level1");
    }
}
