﻿using System;
using UnityEngine;

namespace UIFramework
{
    public abstract class BaseScreen : MonoBehaviour
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        public ScreenInfo ScreenInfo { get; private set; }

        /// <summary>
        /// Screen的名字
        /// </summary>
        public string Name { get { return ScreenInfo.Name; } }

        /// <summary>
        /// 当其他窗口占据前景时，是否应该隐藏此窗口？
        /// </summary>
        public bool HideOnForegroundLost { get { return ScreenInfo.HideOnForegroundLost; } }

        /// <summary>
        /// 是否弹出窗口
        /// </summary>
        public bool IsPopup { get { return ScreenInfo.IsPopup; } }

        /// <summary>
        /// 屏幕当前可见吗？
        /// </summary>
        /// <value>true 可见 false不可见.</value>
        public bool IsVisible { get; private set; }


        /// <summary>
        /// 在进入动画转换完成时发生。
        /// </summary>
        public Action<BaseScreen> InTransitionFinished { get; set; }

        /// <summary>
        /// 在退出动画转换完成时发生。
        /// </summary>
        public Action<BaseScreen> OutTransitionFinished { get; set; }

        /// <summary>
        /// 屏幕销毁事件
        /// 如果这个屏幕由于某种原因被破坏，它必须警告它的层
        /// </summary>
        public Action<BaseScreen> ScreenDestroyed { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param>场景信息.</param>
        public void InitScreen(ScreenInfo screenInfo)
        {
            ScreenInfo = screenInfo;
#if UI_FRAME_DEBUG
            Debug.Log("初始化 " + Name);
#endif
            OnInitScreen();
            IsVisible = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 派生类初始化逻辑
        /// </summary>
        /// <param name="screenInfo"></param>
        protected abstract void OnInitScreen();

        /// <summary>
        /// 销毁Screen
        /// </summary>
        private void DestroyedScreen()
        {
#if UI_FRAME_DEBUG
            Debug.Log("销毁 " + Name);
#endif
            InTransitionFinished = null;
            OutTransitionFinished = null;
            IsVisible = false;
            OnDestroyedScreen();
            GameObject.Destroy(gameObject);
            ScreenDestroyed?.Invoke(this);
        }
        /// <summary>
        /// 派生类销毁逻辑
        /// </summary>
        protected abstract void OnDestroyedScreen();

        /// <summary>
        /// 显示Screen
        /// </summary>
        public void ShowScreen(BaseScreenData screenData)
        {
#if UI_FRAME_DEBUG
            Debug.Log("显示 " + Name);
#endif
            HierarchyFixOnShow();
            OnShowScreen(screenData);
        }

        /// <summary>
        /// 派生类显示逻辑
        /// </summary>
        /// <param name="screenData"></param>
        protected abstract void OnShowScreen(BaseScreenData screenData);

        /// <summary>
        /// 关闭Screen
        /// </summary>
        public void CloseScreen()
        {
#if UI_FRAME_DEBUG
            Debug.Log("关闭 " + Name);
#endif
            OnCloseScreen();
        }

        /// <summary>
        /// 派生类关闭逻辑
        /// </summary>
        protected abstract void OnCloseScreen();

        /// <summary>
        /// 隐藏Screen
        /// </summary>
        public void HideScreen()
        {
#if UI_FRAME_DEBUG
            Debug.Log("隐藏 " + Name);
#endif
            OnHideScreen();
            IsVisible = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 派生类隐藏逻辑
        /// </summary>
        protected abstract void OnHideScreen();

        /// <summary>
        /// Screen进入事件
        /// </summary>
        protected void OnTransitionInFinished()
        {
#if UI_FRAME_DEBUG
            Debug.Log("进入事件 " + Name);
#endif
            IsVisible = true;
            gameObject.SetActive(true);
            if (InTransitionFinished != null)
            {
                InTransitionFinished(this);
            }
        }

        /// <summary>
        /// Screen退出事件
        /// </summary>
        protected void OnTransitionOutFinished()
        {
#if UI_FRAME_DEBUG
            Debug.Log("退出事件 " + Name);
#endif
            IsVisible = false;
            gameObject.SetActive(false);

            if (OutTransitionFinished != null)
            {
                OutTransitionFinished(this);
            }
            DestroyedScreen();
        }

        /// <summary>
        /// Screen打开时层级调整
        /// </summary>
        protected virtual void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();
        }
    }
}
