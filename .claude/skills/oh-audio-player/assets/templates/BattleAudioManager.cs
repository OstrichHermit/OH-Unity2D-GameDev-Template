using Sirenix.OdinInspector;
using UnityEngine;

namespace OHTools
{
    /// <summary>
    /// 战斗音效管理器，管理战斗系统相关音效
    /// </summary>
    public class BattleAudioManager : OHAudioPlayer
    {
        [FoldoutGroup("攻击音效", expanded: true)]
        [LabelText("普通攻击音效")]
        [SerializeField]
        private AudioClip _normalAttackSound;

        [FoldoutGroup("攻击音效")]
        [LabelText("重击音效")]
        [SerializeField]
        private AudioClip _heavyAttackSound;

        [FoldoutGroup("攻击音效")]
        [LabelText("技能攻击音效")]
        [SerializeField]
        private AudioClip _skillAttackSound;

        [FoldoutGroup("受击音效")]
        [LabelText("玩家受击音效")]
        [SerializeField]
        private AudioClip _playerHitSound;

        [FoldoutGroup("受击音效")]
        [LabelText("敌人受击音效")]
        [SerializeField]
        private AudioClip _enemyHitSound;

        [FoldoutGroup("战斗状态音效")]
        [LabelText("胜利音效")]
        [SerializeField]
        private AudioClip _victorySound;

        [FoldoutGroup("战斗状态音效")]
        [LabelText("失败音效")]
        [SerializeField]
        private AudioClip _defeatSound;

        [FoldoutGroup("音量控制")]
        [LabelText("战斗音效音量")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _volume = 1f;

        protected override void Awake()
        {
            if (!RegisterSingleton()) return;
        }

        #region 攻击音效

        /// <summary>
        /// 播放普通攻击音效
        /// </summary>
        public void PlayNormalAttackSound()
        {
            if (_normalAttackSound != null)
            {
                PlayAudio(_normalAttackSound, _volume);
            }
        }

        /// <summary>
        /// 播放重击音效
        /// </summary>
        public void PlayHeavyAttackSound()
        {
            if (_heavyAttackSound != null)
            {
                PlayAudio(_heavyAttackSound, _volume);
            }
        }

        /// <summary>
        /// 播放技能攻击音效
        /// </summary>
        public void PlaySkillAttackSound()
        {
            if (_skillAttackSound != null)
            {
                PlayAudio(_skillAttackSound, _volume);
            }
        }

        #endregion

        #region 受击音效

        /// <summary>
        /// 播放玩家受击音效
        /// </summary>
        public void PlayPlayerHitSound()
        {
            if (_playerHitSound != null)
            {
                PlayAudio(_playerHitSound, _volume);
            }
        }

        /// <summary>
        /// 播放敌人受击音效
        /// </summary>
        public void PlayEnemyHitSound()
        {
            if (_enemyHitSound != null)
            {
                PlayAudio(_enemyHitSound, _volume);
            }
        }

        #endregion

        #region 战斗状态音效

        /// <summary>
        /// 播放胜利音效
        /// </summary>
        public void PlayVictorySound()
        {
            if (_victorySound != null)
            {
                PlayAudio(_victorySound, _volume);
            }
        }

        /// <summary>
        /// 播放失败音效
        /// </summary>
        public void PlayDefeatSound()
        {
            if (_defeatSound != null)
            {
                PlayAudio(_defeatSound, _volume);
            }
        }

        #endregion

        #region 音量控制

        /// <summary>
        /// 设置战斗音效音量
        /// </summary>
        public void SetVolume(float volume)
        {
            _volume = Mathf.Clamp01(volume);
        }

        #endregion
    }
}