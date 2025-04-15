using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon.Evnents;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Video;

namespace Dungeon
{
    public class OpenningLogoController : MonoBehaviour
    {
        public VideoPlayer videoPlayer;

        void Start()
        {
            if (videoPlayer == null)
            {
                videoPlayer = GetComponent<VideoPlayer>();
            }
            
            videoPlayer.isLooping = true;
            videoPlayer.loopPointReached += OnVideoFinished;

            videoPlayer.Play();
        }

        private void OnVideoFinished(VideoPlayer source)
        {
            GameFrameworkLog.Info("开场Logo播放完毕");

            DungeonGameEntry.DungeonGameEntry.Event.Fire(this, new OnOpenningLogoEndEvent());   
        }
    }
}
