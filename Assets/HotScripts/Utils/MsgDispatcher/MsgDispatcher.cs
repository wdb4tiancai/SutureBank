using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
public delegate void Callback<T, U, V, W>(T arg1, U arg2, V arg3, W arg4);
public delegate void Callback<T, U, V, W, X>(T arg1, U arg2, V arg3, W arg4, X arg5);
public delegate void Callback<T, U, V, W, X, Y>(T arg1, U arg2, V arg3, W arg4, X arg5, Y arg6);
namespace Game.Util
{
    public class MsgDispatcher : SingletonBase<MsgDispatcher>
    {
        //订阅的消息
        public Dictionary<int, Delegate> m_EventTable;
        //永久的订阅消息
        public List<int> m_PermanentMessages;
        public MsgDispatcher()
        {
            m_EventTable = new Dictionary<int, Delegate>(100);
            m_PermanentMessages = new List<int>(20);
        }

        /// <summary>
        /// 添加永久的订阅消息标记
        /// </summary>
        /// <param name="eventType"></param>
        public void MarkAsPermanent(int eventType)
        {
            if (!m_PermanentMessages.Contains(eventType))
            {
                m_PermanentMessages.Add(eventType);
            }
        }

        /// <summary>
        /// 删除所有的订阅消息
        /// 包括m_PermanentMessages
        /// </summary>
        public void ForceCleanup()
        {
            m_PermanentMessages.Clear();
            Cleanup();
        }

        /// <summary>
        /// 删除所有的订阅消息
        /// 除了m_PermanentMessages
        /// </summary>
        public void Cleanup()
        {
            List<int> messagesToRemove = new List<int>();

            foreach (KeyValuePair<int, Delegate> pair in m_EventTable)
            {
                bool wasFound = false;

                foreach (int message in m_PermanentMessages)
                {
                    if (pair.Key == message)
                    {
                        wasFound = true;
                        break;
                    }
                }
                if (!wasFound)
                    messagesToRemove.Add(pair.Key);
            }

            foreach (int message in messagesToRemove)
            {
                m_EventTable.Remove(message);
            }
        }

        #region 消息订阅
        private void OnListenerAdding(int eventType, Delegate listenerBeingAdded)
        {
            if (!m_EventTable.ContainsKey(eventType))
            {
                m_EventTable.Add(eventType, null);
            }

            Delegate d = m_EventTable[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        public void AddListener(int eventType, Callback handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback)m_EventTable[eventType] + handler;
        }

        public void AddListener<T>(int eventType, Callback<T> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T>)m_EventTable[eventType] + handler;
        }
        public void AddListener<T, U>(int eventType, Callback<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T, U>)m_EventTable[eventType] + handler;
        }
        public void AddListener<T, U, V>(int eventType, Callback<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T, U, V>)m_EventTable[eventType] + handler;
        }
        public void AddListener<T, U, V, W>(int eventType, Callback<T, U, V, W> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T, U, V, W>)m_EventTable[eventType] + handler;
        }
        public void AddListener<T, U, V, W, X>(int eventType, Callback<T, U, V, W, X> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T, U, V, W, X>)m_EventTable[eventType] + handler;
        }

        public void AddListener<T, U, V, W, X, Y>(int eventType, Callback<T, U, V, W, X, Y> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T, U, V, W, X, Y>)m_EventTable[eventType] + handler;
        }
        #endregion

        #region 消息取消订阅

        private void OnListenerRemoved(int eventType)
        {
            if (m_EventTable[eventType] == null)
            {
                m_EventTable.Remove(eventType);
            }
        }

        private bool OnListenerRemoving(int eventType, Delegate listenerBeingRemoved)
        {
            if (m_EventTable.ContainsKey(eventType))
            {
                Delegate d = m_EventTable[eventType];

                if (d == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void RemoveListener(int eventType, Callback handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        private void RemoveListener<T>(int eventType, Callback<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        private void RemoveListener<T, U>(int eventType, Callback<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T, U>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        private void RemoveListener<T, U, V>(int eventType, Callback<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T, U, V>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        private void RemoveListener<T, U, V, W>(int eventType, Callback<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T, U, V, W>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        private void RemoveListener<T, U, V, W, X>(int eventType, Callback<T, U, V, W, X> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T, U, V, W, X>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        private void RemoveListener<T, U, V, W, X, Y>(int eventType, Callback<T, U, V, W, X, Y> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T, U, V, W, X, Y>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region 消息发布
        public void Broadcast(int eventType)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback callback = d as Callback;
                callback?.Invoke();
            }
        }

        public void Broadcast<T>(int eventType, T arg1)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T> callback = d as Callback<T>;
                callback?.Invoke(arg1);
            }
        }
        public void Broadcast<T, U>(int eventType, T arg1, U arg2)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U> callback = d as Callback<T, U>;
                callback?.Invoke(arg1, arg2);
            }
        }
        public void Broadcast<T, U, V>(int eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V> callback = d as Callback<T, U, V>;
                callback?.Invoke(arg1, arg2, arg3);
            }
        }

        public void Broadcast<T, U, V, W>(int eventType, T arg1, U arg2, V arg3, W arg4)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V, W> callback = d as Callback<T, U, V, W>;
                callback?.Invoke(arg1, arg2, arg3, arg4);
            }
        }
        public void Broadcast<T, U, V, W, X, Y>(int eventType, T arg1, U arg2, V arg3, W arg4, X arg5)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V, W, X> callback = d as Callback<T, U, V, W, X>;
                callback?.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
        }

        public void Broadcast<T, U, V, W, X, Y>(int eventType, T arg1, U arg2, V arg3, W arg4, X arg5, Y arg6)
        {
            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V, W, X, Y> callback = d as Callback<T, U, V, W, X, Y>;
                callback?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }
        }
        #endregion

        #region 异常提示
        public class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }
        #endregion
    }
}

