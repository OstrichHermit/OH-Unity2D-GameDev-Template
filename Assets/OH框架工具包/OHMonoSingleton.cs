// 单例模式基类
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace OHTools
{
    /// <summary>
    /// 可选单例模式基类：
    /// - GlobalUnique：全局唯一，可选 DontDestroyOnLoad。
    /// - PerSceneUnique：每个场景唯一，场景内重复则后创建的销毁。
    /// </summary>
    public class OHMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public enum SingletonMode
        {
            [InspectorName("禁用")] Disabled,
            [InspectorName("场景单例")] PerSceneUnique,
            [InspectorName("全局单例")] GlobalUnique,
        }
        
        #region Fields & Properties
        [Header("单例选项")]
        [SerializeField, LabelText("单例模式")] protected SingletonMode singletonMode = SingletonMode.GlobalUnique;
        [SerializeField, LabelText("全局 DontDestroyOnLoad")] protected bool dontDestroyOnLoadWhenGlobal = true;

        // 全局实例
        private static T _globalInstance;
        // 按场景实例
        private static readonly Dictionary<int, T> _sceneInstances = new Dictionary<int, T>();
        
        /// <summary>
        /// 获取当前有效的单例实例。
        /// Global 模式返回全局实例；PerScene 模式返回当前场景实例；Disabled 模式返回 null。
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_globalInstance != null) return _globalInstance;

                int sceneHandle = SceneManager.GetActiveScene().handle;
                if (_sceneInstances.TryGetValue(sceneHandle, out var sceneInst))
                    return sceneInst;

                return null;
            }
        }
        #endregion

        #region Lifecycle
        protected virtual void Awake()
        {
            // 若单例模式被禁用则不处理
            if (singletonMode == SingletonMode.Disabled) return;
            
            // 处理单例注册和去重；若被销毁则不再继续初始化
            if (!RegisterSingleton()) return;
        }

        protected virtual void OnDestroy()
        {
            // 若单例模式被禁用则不处理
            if (singletonMode == SingletonMode.Disabled) return;
            
            // 取消注册单例
            UnregisterSingleton();
        }
        #endregion

        #region Singleton Logic
        /// <summary>
        /// 运行时切换单例模式。会立即重新注册或撤销注册。
        /// </summary>
        [Button("切换单例模式")]
        public void SetSingletonMode(SingletonMode mode)
        {
            if (singletonMode == mode) return;
            UnregisterSingleton();
            singletonMode = mode;
            RegisterSingleton();
        }

        /// <summary>
        /// 注册单例。如果由于冲突导致对象被销毁，返回 false。
        /// </summary>
        protected bool RegisterSingleton()
        {
            switch (singletonMode)
            {
                case SingletonMode.GlobalUnique:
                    if (_globalInstance != null && _globalInstance != this)
                    {
                        Destroy(gameObject);
                        return false;
                    }
                    _globalInstance = this as T;
                    if (dontDestroyOnLoadWhenGlobal)
                    {
                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        DontDestroyOnLoad(gameObject);
                    }
                    return true;

                case SingletonMode.PerSceneUnique:
                    int handle = gameObject.scene.handle;
                    if (_sceneInstances.TryGetValue(handle, out var existed) && existed != this)
                    {
                        Destroy(gameObject);
                        return false;
                    }
                    _sceneInstances[handle] = this as T;
                    return true;

                default:
                    return true;
            }
        }

        /// <summary>
        /// 撤销单例注册。
        /// </summary>
        protected void UnregisterSingleton()
        {
            switch (singletonMode)
            {
                case SingletonMode.GlobalUnique:
                    if (_globalInstance == this) _globalInstance = null;
                    break;
                case SingletonMode.PerSceneUnique:
                    int handle = gameObject.scene.handle;
                    if (_sceneInstances.TryGetValue(handle, out var existed) && existed == this)
                        _sceneInstances.Remove(handle);
                    break;
            }
        }
        #endregion
    }
}
