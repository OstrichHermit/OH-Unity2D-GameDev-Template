using Sirenix.OdinInspector;
using UnityEngine;

namespace OHTools
{
    /// <summary>
    /// 背景音乐管理器，管理游戏中所有背景音乐
    /// </summary>
    public class BGMManager : OHAudioPlayer
    {
        [FoldoutGroup("背景音乐", expanded: true)]
        [LabelText("主菜单 BGM")]
        [SerializeField]
        private AudioClip _menuBGM;

        [FoldoutGroup("背景音乐")]
        [LabelText("战斗 BGM")]
        [SerializeField]
        private AudioClip _battleBGM;

        [FoldoutGroup("背景音乐")]
        [LabelText("游戏 BGM")]
        [SerializeField]
        private AudioClip _gameBGM;

        [FoldoutGroup("背景音乐")]
        [LabelText("胜利 BGM")]
        [SerializeField]
        private AudioClip _victoryBGM;

        [FoldoutGroup("背景音乐")]
        [LabelText("失败 BGM")]
        [SerializeField]
        private AudioClip _defeatBGM;

        [FoldoutGroup("音量控制")]
        [LabelText("BGM 音量")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _volume = 0.5f;

        // 当前播放的 BGM
        private AudioClip _currentBGM;

        protected override void Awake()
        {
            if (!RegisterSingleton()) return;
        }

        #region 公共播放方法

        /// <summary>
        /// 播放主菜单 BGM
        /// </summary>
        public void PlayMenuBGM()
        {
            PlayBGMInternal(_menuBGM);
        }

        /// <summary>
        /// 播放战斗 BGM
        /// </summary>
        public void PlayBattleBGM()
        {
            PlayBGMInternal(_battleBGM);
        }

        /// <summary>
        /// 播放游戏 BGM
        /// </summary>
        public void PlayGameBGM()
        {
            PlayBGMInternal(_gameBGM);
        }

        /// <summary>
        /// 播放胜利 BGM
        /// </summary>
        public void PlayVictoryBGM()
        {
            PlayBGMInternal(_victoryBGM);
        }

        /// <summary>
        /// 播放失败 BGM
        /// </summary>
        public void PlayDefeatBGM()
        {
            PlayBGMInternal(_defeatBGM);
        }

        /// <summary>
        /// 停止当前 BGM
        /// </summary>
        public void StopCurrentBGM()
        {
            if (_currentBGM != null)
            {
                StopPlayerWithClip(_currentBGM);
                _currentBGM = null;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 播放 BGM 内部实现
        /// </summary>
        private void PlayBGMInternal(AudioClip clip)
        {
            if (clip == null) return;

            // 如果播放的是同一首 BGM，不做处理
            if (_currentBGM == clip) return;

            // 停止当前 BGM
            StopCurrentBGM();

            // 播放新 BGM
            PlayBGM(clip, _volume);
            _currentBGM = clip;
        }

        #endregion

        #region 音量控制

        /// <summary>
        /// 设置 BGM 音量
        /// </summary>
        public void SetVolume(float volume)
        {
            _volume = Mathf.Clamp01(volume);

            // 如果正在播放 BGM，实时更新音量
            if (_currentBGM != null)
            {
                var player = GetPlayerWithClip(_currentBGM);
                if (player != null)
                {
                    player.volume = _volume;
                }
            }
        }

        #endregion
    }
}