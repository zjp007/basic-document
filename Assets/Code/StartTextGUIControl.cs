using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using TMPro;
using UnityEngine.Events;

public class StartTextGUIControl : MonoBehaviour
{
    [Header("显示的时间 (秒)")] 
    public float showTime = 3f;
    [Header("消失的动画时长")] 
    public float disapperTime = 0.5f;
    [Header("文字显示结束之后执行 方法")] 
    public UnityEvent OnShowCompleteEvent;
    
    // 开始文字是否结束
    protected bool isStartEnd = false;
    
    public bool IsStartEnd => isStartEnd;
    
    private TextMeshProUGUI textMesh;
    
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        Sequence.Create()
            .ChainCallback(target: this, target =>
            {
                isStartEnd = false;
                target.textMesh.enabled = true;
            })
            .ChainDelay(showTime)
            .Chain(
                Tween.Custom(1.0f ,0.0f, disapperTime , onValueChange:x => textMesh.color = new Color(1,1,1,x))
                    .OnComplete(target: this, target =>
                    {
                        isStartEnd = true;
                        target.textMesh.enabled = false;
                        
                        OnShowCompleteEvent?.Invoke();
                        // ScoreAndCollision score = FindObjectOfType<ScoreAndCollision>();
                        // if(score) score.SetIsStartFlag(true);
                    })
                );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
