using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class Plane_VideoPlayer : MonoBehaviour
{
    public VideoClip videoClip;   // 在 Inspector 里拖视频
    public bool playOnAwake = true;
    public bool loop = true;

    private VideoPlayer videoPlayer;

    void Awake()
    {
        Debug.Log("Plane_VideoPlayer Awake 被调用了");

        // 获取同一个物体上的 VideoPlayer
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("没有找到 VideoPlayer 组件！");
            return;
        }

        // 基础设置
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;
        videoPlayer.isLooping = loop;

        if (videoClip == null)
        {
            Debug.LogError("⚠️ VideoClip 没有绑定！");
            return;
        }

        if (playOnAwake)
        {
            videoPlayer.Play();
            Debug.Log("✅ VideoPlayer.Play() 已调用，开始播放视频");
        }
    }
}
