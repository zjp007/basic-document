using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using PrimeTween;

public class EndImgControl : MonoBehaviour
{
    
    [Header("文字显示结束之后执行 方法")] 
    public UnityEvent OnImgShowEvent;
    
    public Image EndImg;
    
    void Start()
    {
        EndImg  =  GetComponent<Image>();
        EndImg.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ImgShow()
    {
        EndImg.enabled = true;
        Tween.Custom(0.0f, 1.0f, 1, onValueChange: x => EndImg.color = new Color(1, 1, 1, x))
            .OnComplete(target: this, target =>
            {
                OnImgShowEvent?.Invoke();
            });
    }
}
