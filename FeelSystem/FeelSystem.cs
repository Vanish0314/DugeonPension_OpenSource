using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using MoreMountains.Tools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        private MMF_Player _textShakePlayer; // 新增Text抖动Player
        private MMF_Player _textShakeWithColorPlayer; // 带颜色变化的抖动
        
        
        [Header("Text Shake Settings")]
        [SerializeField] private float defaultShakeDuration = 0.5f;
        [SerializeField] private float defaultShakeSpeed = 20f;
        [SerializeField] private float defaultShakeRange = 5f;
        [SerializeField] private Vector3 defaultShakeDirection = Vector3.right;
        [SerializeField] private Vector3 defaultAltDirection = Vector3.up;
        [SerializeField] private bool defaultRandomizeDirection = true;
        [SerializeField] private Vector3 defaultNoiseMin = Vector3.zero;
        [SerializeField] private Vector3 defaultNoiseMax = Vector3.one;
        [SerializeField] private AnimationCurve defaultColorCurve;

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
            // Text抖动
            _textShakePlayer = InitMMF_Player("TextShake");
            AddTextShakeFeedback(_textShakePlayer, false);
            _textShakePlayer.ForceTimescaleMode = true;
            _textShakePlayer.ForcedTimescaleMode = TimescaleModes.Unscaled;
            // 带颜色变化的文本抖动
            _textShakeWithColorPlayer = InitMMF_Player("TextShakeWithColor");
            AddTextShakeFeedback(_textShakeWithColorPlayer, true);
            _textShakeWithColorPlayer.ForceTimescaleMode = true;
            _textShakeWithColorPlayer.ForcedTimescaleMode = TimescaleModes.Unscaled;
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

        private void AddTextShakeFeedback(MMF_Player player, bool includeColor)
        {
            // 添加位置抖动
            MMF_PositionShake positionShake = new MMF_PositionShake();
            //positionShake.Timing.TimescaleMode = TimescaleModes.Unscaled; 
            positionShake.Duration = defaultShakeDuration;
            positionShake.ShakeSpeed = defaultShakeSpeed;
            positionShake.ShakeRange = defaultShakeRange;
            positionShake.ShakeMainDirection = defaultShakeDirection;
            positionShake.RandomizeDirection = defaultRandomizeDirection;
            positionShake.ShakeAltDirection = defaultAltDirection;
            positionShake.DirectionalNoiseStrengthMin = defaultNoiseMin;
            positionShake.DirectionalNoiseStrengthMax = defaultNoiseMax;
            positionShake.AddDirectionalNoise = true;
            positionShake.Label = "TextPositionShake";
            player.AddFeedback(positionShake);

            if (includeColor)
            {
                // 使用MMF_TextColor替代MMF_TextMeshProColor
                MMF_TextColor colorFeedback = new MMF_TextColor();
                //colorFeedback.Timing.TimescaleMode = TimescaleModes.Unscaled;
                colorFeedback.ColorMode = MMF_TextColor.ColorModes.Interpolate;
                colorFeedback.Duration = defaultShakeDuration * 0.8f;
                colorFeedback.DestinationColor = Color.red; // 目标颜色
                colorFeedback.ColorCurve = defaultColorCurve;

                colorFeedback.Label = "TextColorFlash";
                player.AddFeedback(colorFeedback);
            }
        }

        // 普通抖动方法（无颜色变化）
        public void ShakeText(
            Text text, 
            float intensity = 1.0f, 
            float? duration = null,
            float? speed = null,
            float? range = null,
            Vector3? mainDirection = null,
            Vector3? altDirection = null,
            bool? randomizeDirection = null,
            Vector3? noiseMin = null,
            Vector3? noiseMax = null)
        {
            if (_textShakePlayer == null || text == null) return;

            // 获取或添加Shaker组件
            MMPositionShaker shaker = text.GetComponent<MMPositionShaker>();
            if (shaker == null)
            {
                shaker = text.gameObject.AddComponent<MMPositionShaker>();
                shaker.Mode = MMPositionShaker.Modes.RectTransform;
                shaker.TargetRectTransform = text.rectTransform;
            }

            // 设置参数
            MMF_PositionShake shakeFeedback = _textShakePlayer.GetFeedbackOfType<MMF_PositionShake>();
            if (shakeFeedback != null)
            {
                shakeFeedback.TargetShaker = shaker;
                shakeFeedback.Duration = duration ?? defaultShakeDuration;
                shakeFeedback.ShakeSpeed = speed ?? defaultShakeSpeed;
                shakeFeedback.ShakeRange = (range ?? defaultShakeRange) * intensity;
                shakeFeedback.ShakeMainDirection = mainDirection ?? defaultShakeDirection;
                shakeFeedback.ShakeAltDirection = altDirection ?? defaultAltDirection;
                shakeFeedback.RandomizeDirection = randomizeDirection ?? defaultRandomizeDirection;
                shakeFeedback.DirectionalNoiseStrengthMin = noiseMin ?? defaultNoiseMin;
                shakeFeedback.DirectionalNoiseStrengthMax = noiseMax ?? defaultNoiseMax;
            }

            _textShakePlayer.PlayFeedbacks();
        }

        // 带颜色变化的抖动方法
        public void ShakeTextWithColor(
            Text text, 
            Color flashColor,
            float intensity = 1.0f,
            float? duration = null,
            float? speed = null,
            float? range = null,
            Vector3? mainDirection = null,
            Vector3? altDirection = null,
            bool? randomizeDirection = null,
            Vector3? noiseMin = null,
            Vector3? noiseMax = null)
        {
            if (_textShakeWithColorPlayer == null || text == null) return;

            // 1. 保存初始颜色（如果尚未保存）
            if (!_originalTextColors.ContainsKey(text))
            {
                _originalTextColors[text] = text.color;
            }

            // 2. 取消正在进行的颜色恢复
            if (_activeColorRestorations.TryGetValue(text, out var runningCoroutine))
            {
                if (runningCoroutine != null)
                {
                    StopCoroutine(runningCoroutine);
                }
            }
            
            // 获取或添加Shaker组件
            MMPositionShaker shaker = text.GetComponent<MMPositionShaker>();
            if (shaker == null)
            {
                shaker = text.gameObject.AddComponent<MMPositionShaker>();
                shaker.Mode = MMPositionShaker.Modes.RectTransform;
                shaker.TargetRectTransform = text.rectTransform;
            }

            // 设置位置抖动参数
            MMF_PositionShake shakeFeedback = _textShakeWithColorPlayer.GetFeedbackOfType<MMF_PositionShake>();
            if (shakeFeedback != null)
            {
                shakeFeedback.TargetShaker = shaker;
                shakeFeedback.Duration = duration ?? defaultShakeDuration;
                shakeFeedback.ShakeSpeed = speed ?? defaultShakeSpeed;
                shakeFeedback.ShakeRange = (range ?? defaultShakeRange) * intensity;
                shakeFeedback.ShakeMainDirection = mainDirection ?? defaultShakeDirection;
                shakeFeedback.ShakeAltDirection = altDirection ?? defaultAltDirection;
                shakeFeedback.RandomizeDirection = randomizeDirection ?? defaultRandomizeDirection;
                shakeFeedback.DirectionalNoiseStrengthMin = noiseMin ?? defaultNoiseMin;
                shakeFeedback.DirectionalNoiseStrengthMax = noiseMax ?? defaultNoiseMax;
            }

            // 4. 设置颜色变化参数
            MMF_TextColor colorFeedback = _textShakeWithColorPlayer.GetFeedbackOfType<MMF_TextColor>();
            if (colorFeedback != null)
            {
                colorFeedback.TargetText = text;
                colorFeedback.DestinationColor = flashColor;
                colorFeedback.Duration = (duration ?? defaultShakeDuration) * 0.8f;
            }

            _textShakeWithColorPlayer.PlayFeedbacks();
    
            // 5. 启动新的颜色恢复协程
            _activeColorRestorations[text] = StartCoroutine(
                RestoreTextColorAfterShake(
                    text, 
                    _originalTextColors[text], // 总是使用最初存储的颜色
                    (duration ?? defaultShakeDuration) * 0.8f
                )
            );
        }
        
        // 在类中添加字段存储初始颜色
        private Dictionary<Text, Color> _originalTextColors = new Dictionary<Text, Color>();
        private Dictionary<Text, Coroutine> _activeColorRestorations = new Dictionary<Text, Coroutine>();
        
        // 修改后的恢复协程
        private IEnumerator RestoreTextColorAfterShake(Text text, Color originalColor, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            if (text != null)
            {
                float elapsedTime = 0f;
                float restoreDuration = 0.3f;
                Color currentColor = text.color;
        
                while (elapsedTime < restoreDuration)
                {
                    if (text == null) yield break;
            
                    elapsedTime += Time.unscaledDeltaTime;
                    text.color = Color.Lerp(currentColor, originalColor, elapsedTime / restoreDuration);
                    yield return null;
                }
        
                if (text != null)
                {
                    text.color = originalColor;
                }
            }
    
            // 清理字典
            if (_activeColorRestorations.ContainsKey(text))
            {
                _activeColorRestorations.Remove(text);
            }
        }
    }
}