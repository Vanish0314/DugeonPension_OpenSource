using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

namespace Dungeon
{
    public class FeelSystem : MonoBehaviour
    {
        //单例
        public static FeelSystem Instance;

        //这个是震动设置，没想到好方法加载进来，干脆就public直接托了
        public NoiseSettings noiseSettings;

        //全局虚拟相机
        private CinemachineVirtualCamera virtualCamera;

        private MMFloatingTextSpawner m_MMFloatingTextSpawner;

        //feels
        private MMF_Player _screenShakePlayer;
        private MMF_Player _screenZoomPlayer;
        private MMF_Player _timeFreezePlayer;
        private MMF_Player _timeScalePlayer;
        private MMF_Player _floatingTextPlayer;

        private void Awake()
        {
            //InitCinemachine();
            m_MMFloatingTextSpawner = gameObject.GetComponentInChildren<MMFloatingTextSpawner>();
            // 单例初始化
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 保持单例跨场景
                //DontDestroyOnLoad(virtualCamera);
            }
            else
            {
                Destroy(gameObject); // 防止重复创建
               // Destroy(virtualCamera);
            }

           // SceneManager.sceneLoaded += OnSceneLoaded;

            // 屏幕震动
            _screenShakePlayer = InitMMF_Player("ScreenShake");
            AddScreenShakeFeedback(_screenShakePlayer);
            // 屏幕放大
            _screenZoomPlayer = InitMMF_Player("ScreenZoom");
            AddScreenZoomFeedback(_screenZoomPlayer);
            // 时间冻结
            _timeFreezePlayer = InitMMF_Player("TimeFreeze");
            AddTimeFreezeFeedback(_timeFreezePlayer);
            // 时间放缓
            _timeScalePlayer = InitMMF_Player("TimeScale");
            AddTimeScaleFeedback(_timeScalePlayer);
            // 冒气泡
            _floatingTextPlayer = InitMMF_Player("FloatingText");
            AddFloatingTextFeedback(_floatingTextPlayer);
        }

        private void OnDestroy()
        {
           // SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Camera.main.GetComponent<CinemachineBrain>();
        }

        private MMF_Player InitMMF_Player(string playerName)
        {
            // 创建 MMF_Player 对象并设置为 FeelSystem 的子物体
            GameObject mmfPlayerObj = new GameObject(playerName);
            mmfPlayerObj.transform.SetParent(transform);

            // 添加 MMF_Player 组件并初始化
            MMF_Player player = mmfPlayerObj.AddComponent<MMF_Player>();
            player.AutoPlayOnStart = false; // 禁用自动播放
            player.InitialDelay = 0f; // 无初始延迟
            player.DurationMultiplier = 1.0f; // 设置持续时间倍数

            return player;
        }

        private void AddScreenShakeFeedback(MMF_Player player)
        {
            // 创建屏幕震动反馈
            MMF_CinemachineImpulse shakeFeedback = new MMF_CinemachineImpulse();

            // 配置屏幕震动参数
            shakeFeedback.Channel = 0; // 使用默认通道
            shakeFeedback.Velocity = new Vector3(1f, 1f, 0f); // 震动方向
            shakeFeedback.FeedbackDuration = 0.5f; // 震动持续时间
            shakeFeedback.m_ImpulseDefinition = new CinemachineImpulseDefinition();
            shakeFeedback.m_ImpulseDefinition.m_RawSignal = noiseSettings;
            shakeFeedback.Label = "ScreenShake"; // 设置反馈标签

            player.AddFeedback(shakeFeedback);
        }

        private void AddScreenZoomFeedback(MMF_Player player)
        {
            // 创建屏幕震动反馈
            MMF_CameraZoom zoomFeedback = new MMF_CameraZoom();

            // 配置屏幕放大参数
            zoomFeedback.Channel = 0; // 使用默认通道
            zoomFeedback.ZoomDuration = 1f; // 放大持续时间
            zoomFeedback.FeedbackDuration = 1f; // 反馈持续时间
            zoomFeedback.ZoomFieldOfView = 20f; // 放大系数
            zoomFeedback.ZoomMode = MMCameraZoomModes.For; // 放大类型
            zoomFeedback.Label = "ScreenZoom"; // 设置反馈标签

            player.AddFeedback(zoomFeedback);
        }

        private void AddTimeFreezeFeedback(MMF_Player player)
        {
            MMF_FreezeFrame freezeFeedback = new MMF_FreezeFrame();

            freezeFeedback.Channel = 0;
            freezeFeedback.FreezeFrameDuration = 0.5f;
            freezeFeedback.MinimumTimescaleThreshold = 0.1f;
            freezeFeedback.Label = "TimeFreeze";

            player.AddFeedback(freezeFeedback);
        }

        private void AddTimeScaleFeedback(MMF_Player player)
        {
            MMF_TimescaleModifier scaleFeedback = new MMF_TimescaleModifier();

            scaleFeedback.Channel = 0;
            scaleFeedback.TimeScaleDuration = 1f;
            scaleFeedback.TimeScale = 0.1f;
            scaleFeedback.Label = "TimeScale";

            player.AddFeedback(scaleFeedback);
        }

        private void AddFloatingTextFeedback(MMF_Player player)
        {
            MMF_FloatingText flaotingFeedback = new MMF_FloatingText();

            flaotingFeedback.Channel = 0;
            flaotingFeedback.Value = "0";
            flaotingFeedback.PositionMode = MMF_FloatingText.PositionModes.TargetTransform;
            flaotingFeedback.ForceColor = true;
            flaotingFeedback.Label = "FloatingText";

            player.AddFeedback(flaotingFeedback);
        }

        // 屏幕震动方法
        public void ShakeScreen(float intensity = 1.0f, float duration = 0.5f)
        {
            Camera.main.GetComponent<CinemachineBrain>().enabled = true;
            if (_screenShakePlayer == null)
            {
                Debug.LogWarning("ScreenShakePlayer 未初始化！");
                return;
            }

            // 动态调整震动参数
            MMF_CinemachineImpulse shakeFeedback = _screenShakePlayer.GetFeedbackOfType<MMF_CinemachineImpulse>();
            if (shakeFeedback != null)
            {
                shakeFeedback.Velocity *= intensity; // 设置震动强度
                shakeFeedback.FeedbackDuration = duration; // 设置震动持续时间
            }

            // 播放屏幕震动反馈
            _screenShakePlayer.PlayFeedbacks();
            shakeFeedback.Velocity /= intensity;
        }

        // 屏幕放大方法
        public void ZoomScreen(float zoomduration = 1.0f, float feelduration = 1.0f, float zoomfieldofview = 20.0f,
            MMCameraZoomModes zoommode = MMCameraZoomModes.For)
        {
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;

            if (Camera.main.GetComponent<MMCameraZoom>() == null)
            {
                Camera.main.GetComponent<MMCameraZoom>();
            }

            if (_screenZoomPlayer == null)
            {
                Debug.LogWarning("ScreenShakePlayer 未初始化！");
                return;
            }

            // 动态调整震动参数
            MMF_CameraZoom zoomFeedback = _screenZoomPlayer.GetFeedbackOfType<MMF_CameraZoom>();
            if (zoomFeedback != null)
            {
                zoomFeedback.ZoomDuration = zoomduration; // 设置放大持续时间
                zoomFeedback.FeedbackDuration = feelduration; // 设置反馈持续时间
                zoomFeedback.ZoomFieldOfView = zoomfieldofview; // 设置放大系数
                zoomFeedback.ZoomMode = zoommode; // 设置放大类型
            }

            // 播放屏幕震动反馈
            _screenZoomPlayer.PlayFeedbacks();
        }

        // 时间冻结方法/抓帧
        public void FreezeTime(float duration = 0.5f, float threshold = 0.1f)
        {
            MMF_FreezeFrame freezeFeedback = _timeFreezePlayer.GetFeedbackOfType<MMF_FreezeFrame>();
            if (freezeFeedback != null)
            {
                freezeFeedback.FreezeFrameDuration = duration;
                freezeFeedback.MinimumTimescaleThreshold = threshold;
            }

            _timeFreezePlayer.PlayFeedbacks();
        }

        // 时间放缓方法
        public void ScaleTime(float duration = 1.0f, float scale = 0.1f)
        {
            MMF_TimescaleModifier scaleFeedback = _timeScalePlayer.GetFeedbackOfType<MMF_TimescaleModifier>();
            if (scaleFeedback != null)
            {
                scaleFeedback.TimeScaleDuration = duration;
                scaleFeedback.TimeScale = scale;
            }

            _timeScalePlayer.PlayFeedbacks();
        }

        // 冒气泡
        public void FloatingText(string text, Transform target, Gradient gradient)
        {
            MMF_FloatingText floatingFeedback = _floatingTextPlayer.GetFeedbackOfType<MMF_FloatingText>();
            if (floatingFeedback != null)
            {
                floatingFeedback.Value = text;
                floatingFeedback.TargetTransform = target;
                floatingFeedback.AnimateColorGradient = gradient;
            }

            _floatingTextPlayer.PlayFeedbacks();
        }

        // 初始化 Cinemachine 虚拟相机和 Impulse Listener
        private void InitCinemachine()
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

            if (virtualCamera == null)
            {
                // 如果没有找到虚拟相机，就创建一个新的
                GameObject camObj = new GameObject("Cinemachine Virtual Camera");
                virtualCamera = camObj.AddComponent<CinemachineVirtualCamera>();

                // 设置默认Lens属性
                virtualCamera.m_Lens.Orthographic = false;
                virtualCamera.m_Lens.FieldOfView = 60f;
                virtualCamera.transform.position = Camera.main.transform.position;

                Debug.Log("创建了新的Cinemachine Virtual Camera");
            }
            else
            {
                Debug.Log("已经包含Cinemachine Virtual Camera");
            }

            // 检查是否有 Impulse Listener 组件
            CinemachineImpulseListener impulseListener = virtualCamera.GetComponent<CinemachineImpulseListener>();

            if (impulseListener == null)
            {
                // 如果没有，则添加
                impulseListener = virtualCamera.gameObject.AddComponent<CinemachineImpulseListener>();
                impulseListener.m_Gain = 1.0f; // 设置默认增益
                impulseListener.m_Use2DDistance = false; // 适用于3D模式

                Debug.Log("在虚拟相机上添加了 Cinemachine Impulse Listener");
            }
            else
            {
                Debug.Log("虚拟相机已经包含 Cinemachine Impulse Listener");
            }
        }

    }
}