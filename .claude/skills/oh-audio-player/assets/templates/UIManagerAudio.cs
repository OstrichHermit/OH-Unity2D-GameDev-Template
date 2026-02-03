using Sirenix.OdinInspector;
using UnityEngine;

namespace OHTools
{
    /// <summary>
    /// UI 音效管理器，管理所有 UI 相关音效
    /// </summary>
    public class UIManagerAudio : OHAudioPlayer
    {
        [FoldoutGroup("UI 音效", expanded: true)]
        [LabelText("点击音效")]
        [SerializeField]
        private AudioClip _clickSound;

        [FoldoutGroup("UI 音效")]
        [LabelText("悬停音效")]
        [SerializeField]
        private AudioClip _hoverSound;

        [FoldoutGroup("UI 音效")]
        [LabelText("确认音效")]
        [SerializeField]
        private AudioClip _confirmSound;

        [FoldoutGroup("UI 音效")]
        [LabelText("取消音效")]
        [SerializeField]
        private AudioClip _cancelSound;

        [FoldoutGroup("UI 音效")]
        [LabelText("开启音效")]
        [SerializeField]
        private AudioClip _toggleOnSound;

        [FoldoutGroup("UI 音效")]
        [LabelText("关闭音效")]
        [SerializeField]
        private AudioClip _toggleOffSound;

        [FoldoutGroup("音量控制")]
        [LabelText("UI 音效音量")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _volume = 1f;

        protected override void Awake()
        {
            if (!RegisterSingleton()) return;
        }

        #region 公共播放方法

        /// <summary>
        /// 播放点击音效
        /// </summary>
        public void PlayClickSound()
        {
            if (_clickSound != null)
            {
                PlayAudio(_clickSound, _volume);
            }
        }

        /// <summary>
        /// 播放悬停音效
        /// </summary>
        public void PlayHoverSound()
        {
            if (_hoverSound != null)
            {
                PlayAudio(_hoverSound, _volume);
            }
        }

        /// <summary>
        /// 播放确认音效
        /// </summary>
        public void PlayConfirmSound()
        {
            if (_confirmSound != null)
            {
                PlayAudio(_confirmSound, _volume);
            }
        }

        /// <summary>
        /// 播放取消音效
        /// </summary>
        public void PlayCancelSound()
        {
            if (_cancelSound != null)
            {
                PlayAudio(_cancelSound, _volume);
            }
        }

        /// <summary>
        /// 播放开启音效
        /// </summary>
        public void PlayToggleOnSound()
        {
            if (_toggleOnSound != null)
            {
                PlayAudio(_toggleOnSound, _volume);
            }
        }

        /// <summary>
        /// 播放关闭音效
        /// </summary>
        public void PlayToggleOffSound()
        {
            if (_toggleOffSound != null)
            {
                PlayAudio(_toggleOffSound, _volume);
            }
        }

        #endregion

        #region 音量控制

        /// <summary>
        /// 设置 UI 音效音量
        /// </summary>
        public void SetVolume(float volume)
        {
            _volume = Mathf.Clamp01(volume);
        }

        #endregion
    }
}