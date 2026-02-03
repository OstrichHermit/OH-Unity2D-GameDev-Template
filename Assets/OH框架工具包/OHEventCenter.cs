using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace OHTools
{
    #region 事件信息结构 -----------------------------------------------------

    /// <summary>统一不同事件泛型的空接口</summary>
    internal interface IEventInfo { }

    /// <summary>无参数事件包装</summary>
    internal sealed class EventInfo : IEventInfo
    {
        public UnityAction Actions;
        public EventInfo(UnityAction action) => Actions += action;
    }

    /// <summary>单参数事件包装</summary>
    internal sealed class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> Actions;
        public EventInfo(UnityAction<T> action) => Actions += action;
    }

    /// <summary>双参数事件包装</summary>
    internal sealed class EventInfo<T1, T2> : IEventInfo
    {
        public UnityAction<T1, T2> Actions;
        public EventInfo(UnityAction<T1, T2> action) => Actions += action;
    }

    /// <summary>三参数事件包装</summary>
    internal sealed class EventInfo<T1, T2, T3> : IEventInfo
    {
        public UnityAction<T1, T2, T3> Actions;
        public EventInfo(UnityAction<T1, T2, T3> action) => Actions += action;
    }

    #endregion

    /// <summary>
    /// 事件中心（静态调用版）<br/>
    /// 用法示例：<br/>
    ///   OHEventCenter.AddEventListener(OHEvent.GameStart, OnGameStart);<br/>
    ///   OHEventCenter.EventTrigger(OHEvent.GameStart);<br/>
    /// </summary>
    public static class OHEventCenter
    {
        // --- 事件表 ---
        private static readonly Dictionary<OHEvent, IEventInfo> _eventDic = new();

        #region 无参数事件 ----------------------------------------------------

        public static void AddEventListener(OHEvent name, UnityAction action)
        {
            if (_eventDic.TryGetValue(name, out var info))
            {
                if (info is EventInfo ei)
                    ei.Actions += action;
                else
                    Debug.LogWarning($"[OHEventCenter] {name} 已注册但签名不匹配（期望无参）。");
            }
            else
            {
                _eventDic[name] = new EventInfo(action);
            }
        }

        public static void RemoveEventListener(OHEvent name, UnityAction action)
        {
            if (_eventDic.TryGetValue(name, out var info) && info is EventInfo ei)
                ei.Actions -= action;
        }

        public static void EventTrigger(OHEvent name)
        {
            if (_eventDic.TryGetValue(name, out var info) && info is EventInfo ei)
                ei.Actions?.Invoke();
        }

        #endregion

        #region 单参数事件 ----------------------------------------------------

        public static void AddEventListener<T>(OHEvent name, UnityAction<T> action)
        {
            if (_eventDic.TryGetValue(name, out var info))
            {
                if (info is EventInfo<T> ei)
                    ei.Actions += action;
                else
                    Debug.LogWarning($"[OHEventCenter] {name} 已注册但签名不匹配（期望 <{typeof(T)}>)。");
            }
            else
            {
                _eventDic[name] = new EventInfo<T>(action);
            }
        }

        public static void RemoveEventListener<T>(OHEvent name, UnityAction<T> action)
        {
            if (_eventDic.TryGetValue(name, out var info) && info is EventInfo<T> ei)
                ei.Actions -= action;
        }

        public static void EventTrigger<T>(OHEvent name, T arg)
        {
            if (_eventDic.TryGetValue(name, out var info) && info is EventInfo<T> ei)
                ei.Actions?.Invoke(arg);
        }

        #endregion

        #region 双参数事件 ----------------------------------------------------

        public static void AddEventListener<T1, T2>(OHEvent name, UnityAction<T1, T2> action)
        {
            if (_eventDic.TryGetValue(name, out var info))
            {
                if (info is EventInfo<T1, T2> ei)
                    ei.Actions += action;
                else
                    Debug.LogWarning($"[OHEventCenter] {name} 已注册但签名不匹配（期望 <{typeof(T1)}, {typeof(T2)}>）。");
            }
            else
            {
                _eventDic[name] = new EventInfo<T1, T2>(action);
            }
        }

        public static void RemoveEventListener<T1, T2>(OHEvent name, UnityAction<T1, T2> action)
        {
            if (_eventDic.TryGetValue(name, out var info) && info is EventInfo<T1, T2> ei)
                ei.Actions -= action;
        }

        public static void EventTrigger<T1, T2>(OHEvent name, T1 arg1, T2 arg2)
        {
            if (_eventDic.TryGetValue(name, out var info) && info is EventInfo<T1, T2> ei)
                ei.Actions?.Invoke(arg1, arg2);
        }

        #endregion

        #region 三参数事件 ----------------------------------------------------

        public static void AddEventListener<T1, T2, T3>(OHEvent name, UnityAction<T1, T2, T3> action)
        {
            if (_eventDic.TryGetValue(name, out var info))
            {
                if (info is EventInfo<T1, T2, T3> ei)
                    ei.Actions += action;
                else
                    Debug.LogWarning($"[OHEventCenter] {name} 已注册但签名不匹配（期望 <{typeof(T1)}, {typeof(T2)}, {typeof(T3)}>）。");
            }
            else
            {
                _eventDic[name] = new EventInfo<T1, T2, T3>(action);
            }
        }

        public static void RemoveEventListener<T1, T2, T3>(OHEvent name, UnityAction<T1, T2, T3> action)
        {
            if (_eventDic.TryGetValue(name, out var info) && info is EventInfo<T1, T2, T3> ei)
                ei.Actions -= action;
        }

        public static void EventTrigger<T1, T2, T3>(OHEvent name, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_eventDic.TryGetValue(name, out var info) && info is EventInfo<T1, T2, T3> ei)
                ei.Actions?.Invoke(arg1, arg2, arg3);
        }

        #endregion

        /// <summary>移除指定对象的所有事件监听 </summary>
        public static void RemoveAllEventListenersFor(object target)
        {
            foreach (var kv in _eventDic)
            {
                var info = kv.Value;
                var infoType = info.GetType();
                // 拿到 Actions 字段
                var field = infoType.GetField("Actions", 
                    BindingFlags.Instance | BindingFlags.Public);
                if (field == null) continue;

                var del = field.GetValue(info) as Delegate;
                if (del == null) continue;

                // 遍历每个订阅
                foreach (var d in del.GetInvocationList())
                {
                    if (d.Target == target)
                    {
                        // 从 multicast delegate 中移除这一条
                        var newDel = Delegate.Remove(del, d);
                        field.SetValue(info, newDel);
                    }
                }
            }
        }
        
        /// <summary>清空所有事件监听（如切换场景时调用）</summary>
        public static void RemoveAllEventListeners()
        {
            Debug.Log("[OHEventCenter] Clear all listeners.");
            _eventDic.Clear();
        }
    }
}
