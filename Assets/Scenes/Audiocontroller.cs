using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audiocontroller : MonoBehaviour
{
    [Header("±≥æ∞“Ù¿÷…Ë÷√")]
    public AudioClip backgroundMusic;    // ±≥æ∞“Ù¿÷∆¨∂Œ
    public float volume = 0.5f;          // “Ù¡ø¥Û–° (0-1)
    public bool loopMusic = true;        //  «∑Ò—≠ª∑≤•∑≈

    private AudioSource audioSource;     // “Ù∆µ‘¥◊Èº˛

    // Start is called before the first frame update
    void Start()
    {
        // ªÒ»°ªÚÃÌº”AudioSource◊Èº˛
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ≈‰÷√AudioSource
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = loopMusic;
        audioSource.playOnAwake = false;

        // ≤•∑≈±≥æ∞“Ù¿÷
        if (backgroundMusic != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("±≥æ∞“Ù¿÷Œ¥…Ë÷√£°«Î‘⁄Inspector÷–∑÷≈‰AudioClip°£");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// ≤•∑≈±≥æ∞“Ù¿÷
    /// </summary>
    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// ‘›Õ£±≥æ∞“Ù¿÷
    /// </summary>
    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    /// <summary>
    /// Õ£÷π±≥æ∞“Ù¿÷
    /// </summary>
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// …Ë÷√“Ù¡ø
    /// </summary>
    /// <param name="newVolume">–¬“Ù¡ø÷µ (0-1)</param>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}