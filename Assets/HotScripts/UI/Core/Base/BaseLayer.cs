using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFrameWork
{
    /// <summary>
    /// Layer的类型
    /// </summary>
    public enum LayerType
    {
        Panel = 0,
        Window
    }

    /// <summary>
    /// Layer属性
    /// </summary>
    [System.Serializable]
    public class LayerProperties
    {
        /// <summary>
        /// LayerId 每个Layer的唯一ID
        /// </summary>
        public int LayerId;
        /// <summary>
        /// Layer的类型 Panel 或者 Window
        /// </summary>
        public LayerType LayerType;
        /// <summary>
        /// Layer的节点
        /// </summary>
        public Transform LayerTransform;
    }

    //基础Layer
    public abstract class BaseLayer
    {
        /// <summary>
        /// 当前存在的Screen
        /// </summary>
        protected List<BaseScreen> m_ExistScreens;

        /// <summary>
        /// 屏幕销毁事件
        /// 如果这个屏幕由于某种原因被破坏，它必须警告它的层
        /// </summary>
        /// <value>销毁请求.</value>
        private Action<BaseScreen> m_ScreenDestroyed { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            m_ExistScreens = new List<BaseScreen>();
        }

        /// <summary>
        /// 显示屏幕
        /// </summary>
        public abstract void ShowScreen(BaseScreen screen, BaseScreenData screenData, bool isAni);

        /// <summary>
        /// 关闭屏幕
        /// </summary>
        public abstract void CloseScreen(BaseScreen screen, bool isAni);


        /// <summary>
        /// 关闭面板层上的所有屏幕,除了指定屏幕
        /// </summary>
        /// 
        public void CloseAllPanelsExceptLoadingUi(string exceptUiName)
        {
            for (int i = m_ExistScreens.Count - 1; i >= 0; i--)
            {
                if (!m_ExistScreens[i].Name.Equals(exceptUiName))
                {
                    m_ExistScreens[i].CloseScreen(false);
                }
            }
        }

        /// <summary>
        /// 关闭所有
        /// </summary>
        public virtual void CloseAll()
        {
            for (int i = m_ExistScreens.Count - 1; i >= 0; i--)
            {
                m_ExistScreens[i].CloseScreen(false);
            }
        }

        /// <summary>
        /// 添加ui
        /// </summary>
        protected void RegisterScreen(BaseScreen screen)
        {
            if (!m_ExistScreens.Contains(screen))
            {
                ProcessScreenRegister(screen);
            }
            else
            {
                UIFrameDebug.LogError($"UI重复注册  {screen.name} ");
            }
        }

        /// <summary>
        /// 移除ui
        /// </summary>
        protected void UnregisterScreen(BaseScreen screen)
        {
            if (m_ExistScreens.Contains(screen))
            {
                ProcessScreenUnregister(screen);
            }
            else
            {
                UIFrameDebug.LogError($"UI注销不存在 {screen.name}");
            }
        }

        /// <summary>
        /// 是否存在ui
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        public bool IsExistScreens(BaseScreen screen)
        {
            return m_ExistScreens.Contains(screen);
        }

        //通过名字找ui
        public BaseScreen FindScreenByName(string screenName)
        {
            for (int i = 0; i < m_ExistScreens.Count; i++)
            {
                if (m_ExistScreens[i].name.Equals(screenName))
                {
                    return m_ExistScreens[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 添加ui
        /// </summary>
        protected virtual void ProcessScreenRegister(BaseScreen screen)
        {
            screen.OnScreenDestroyed += UnregisterScreen;
            m_ExistScreens.Add(screen);
        }

        /// <summary>
        /// 移除ui
        /// </summary>
        protected virtual void ProcessScreenUnregister(BaseScreen screen)
        {
            screen.OnScreenDestroyed -= UnregisterScreen;
            m_ExistScreens.Remove(screen);
            m_ScreenDestroyed?.Invoke(screen);
        }

        /// <summary>
        /// 添加屏幕移除事件
        /// </summary>
        /// <param name="action"></param>
        public void AddScreenDestroyed(Action<BaseScreen> action)
        {
            m_ScreenDestroyed += action;
        }


        /// <summary>
        /// 移除屏幕移除事件
        /// </summary>
        /// <param name="action"></param>
        public void RemoveScreenDestroyed(Action<BaseScreen> action)
        {
            m_ScreenDestroyed -= action;
        }

    }
}
