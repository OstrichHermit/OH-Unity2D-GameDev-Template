using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace OHTools
{
    /// <summary>
    /// 音频播放器，继承自 OHMonoSingleton 以支持多种单例模式。
    /// </summary>
    public class OHAudioPlayer : OHMonoSingleton<OHAudioPlayer>
    {
        // ===== 以下为你原有的播放/对象池逻辑 =====

        // 池中可复用的 AudioSource 实例
        private ObjectPool<AudioSource> pool;

        // 跟踪每个 clip 正在使用的 AudioSource
        private readonly Dictionary<AudioClip, List<AudioSource>> activePlayers =
            new Dictionary<AudioClip, List<AudioSource>>();

        protected override void Awake()
        {
            // 先处理单例注册和去重；若被销毁则不再继续初始化
            if (!RegisterSingleton()) return;

            pool = new ObjectPool<AudioSource>(
                createFunc: CreateNewAudioSource,
                actionOnGet: src => src.gameObject.SetActive(true),
                actionOnRelease: ResetAudioSource,
                actionOnDestroy: src => Destroy(src.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// 创建一个新的子级 GameObject，只添加 AudioSource，用于播放音频。
        /// </summary>
        private AudioSource CreateNewAudioSource()
        {
            var go = new GameObject("AudioPlayer");
            go.transform.SetParent(transform, worldPositionStays: false);
            var src = go.AddComponent<AudioSource>();
            return src;
        }

        /// <summary>
        /// 停止给定的 AudioSource，并将其释放入池。
        /// </summary>
        private void ReleaseAudioSource(AudioSource src)
        {
            if (src == null) return;

            var clip = src.clip;
            if (clip != null && activePlayers.TryGetValue(clip, out var list))
            {
                list.Remove(src);
                if (list.Count == 0)
                    activePlayers.Remove(clip);
            }

            pool.Release(src);
        }

        /// <summary>
        /// 重置并禁用释放入池的 AudioSource。
        /// </summary>
        private void ResetAudioSource(AudioSource src)
        {
            src.Stop();
            src.clip = null;
            src.loop = false;
            src.volume = 1;
            src.gameObject.SetActive(false);
        }

        /// <summary>
        /// 播放一次指定 clip，结束后自动回收。
        /// </summary>
        protected void PlayAudio(AudioClip clip, float volume = 1f)
        {
            Play(clip, false, volume);
        }

        /// <summary>
        /// 播放指定 clip，并循环。
        /// </summary>
        protected void PlayBGM(AudioClip clip, float volume = 1f)
        {
            Play(clip, true, volume);
        }

        /// <summary>
        /// 播放指定 clip，可选择是否循环。若 loop 为 false，播放完成后自动回收；
        /// 否则持续循环，需手动 StopPlayerWithClip()。
        /// </summary>
        private void Play(AudioClip clip, bool loop, float volume)
        {
            if (clip == null || pool == null) return;

            var src = pool.Get();
            src.clip = clip;
            src.loop = loop;
            src.volume = volume;
            src.Play();

            if (!activePlayers.TryGetValue(clip, out var list))
            {
                list = new List<AudioSource>();
                activePlayers[clip] = list;
            }
            list.Add(src);

            if (!loop)
                StartCoroutine(RecycleAfterDelay(src, clip.length));
        }

        private System.Collections.IEnumerator RecycleAfterDelay(AudioSource src, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReleaseAudioSource(src);
        }

        /// <summary>
        /// 获取任意一个正在播放该 clip 的 AudioSource 实例。
        /// </summary>
        protected AudioSource GetPlayerWithClip(AudioClip clip)
        {
            if (clip == null) return null;

            if (activePlayers.TryGetValue(clip, out var list) && list.Count > 0)
            {
                return list[0];
            }

            return null;
        }

        /// <summary>
        /// 停止并回收任意一个正在播放该 clip 的 AudioSource 实例。
        /// </summary>
        protected void StopPlayerWithClip(AudioClip clip)
        {
            ReleaseAudioSource(GetPlayerWithClip(clip));
        }
    }
}
