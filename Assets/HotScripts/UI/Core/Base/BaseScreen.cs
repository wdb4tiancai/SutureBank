using System;
using UnityEngine;

namespace UIFrameWork
{
    public abstract class BaseScreen : MonoBehaviour
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        public BaseScreenInfo ScreenInfo { get; private set; }

        /// <summary>
        /// Screen的名字
        /// </summary>
        public string Name { get { return ScreenInfo.Name; } }

        /// <summary>
        /// 当其他窗口占据前景时，是否应该隐藏此窗口？
        /// </summary>
        public bool IsHideOnForegroundLost { get { return ScreenInfo.IsHideOnForegroundLost; } }

        /// <summary>
        /// 是否弹出窗口
        /// </summary>
        public bool IsPopup { get { return ScreenInfo.IsPopup; } }

        /// <summary>
        /// 屏幕当前可见吗
        /// </summary>
        /// <value>true 可见 false不可见.</value>
        public bool IsVisible { get; private set; }


        /// <summary>
        /// 在进入动画播放完成事件
        /// </summary>
        public Action<BaseScreen> InTransitionFinished { get; set; }

        /// <summary>
        /// 在退出动画播放完成时事件
        /// </summary>
        public Action<BaseScreen> OutTransitionFinished { get; set; }

        /// <summary>
        /// 屏幕销毁事件
        /// 如果这个屏幕由于某种原因被破坏，它必须警告它的层
        /// </summary>
        public Action<BaseScreen> OnScreenDestroyed { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param>场景信息.</param>
        public void InitScreen(BaseScreenInfo screenInfo)
        {
            IsVisible = false;
            gameObject.SetActive(false);
            ScreenInfo = screenInfo;
            UIFrameDebug.Log($"初始化 {Name} 界面");
            OnInitScreen();
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
            Action<BaseScreen> screenDestroyedAction = OnScreenDestroyed;
            UIFrameDebug.Log($"销毁 {Name} 界面");
            InTransitionFinished = null;
            OutTransitionFinished = null;
            OnScreenDestroyed = null;
            IsVisible = false;
            gameObject.SetActive(false);
            OnDestroyedScreen();
            GameObject.Destroy(gameObject);
            screenDestroyedAction?.Invoke(this);
        }
        /// <summary>
        /// 派生类销毁逻辑
        /// </summary>
        protected abstract void OnDestroyedScreen();

        /// <summary>
        /// 显示Screen
        /// </summary>
        /// <param name="screenData">界面数据</param>
        /// <param name="isAni">是否动画</param>
        public void ShowScreen(BaseScreenData screenData, bool isAni)
        {
            UIFrameDebug.Log($"显示 {Name} 界面 ");
            HierarchyFixOnShow();
            IsVisible = true;
            gameObject.SetActive(true);
            OnShowScreen(screenData, isAni);
        }

        /// <summary>
        /// 派生类显示逻辑
        /// </summary>
        /// <param name="screenData">界面数据</param>
        /// <param name="isAni">是否动画</param>
        protected abstract void OnShowScreen(BaseScreenData screenData, bool isAni);

        /// <summary>
        /// 关闭Screen
        /// </summary>
        /// <param name="isAni">是否动画</param>
        public void CloseScreen(bool isAni)
        {
            UIFrameDebug.Log($"关闭 {Name} 界面");
            OnCloseScreen(isAni);
        }

        /// <summary>
        /// 派生类关闭逻辑
        /// </summary>
        /// <param name="isAni">是否动画</param>
        protected abstract void OnCloseScreen(bool isAni);

        /// <summary>
        /// 隐藏Screen
        /// </summary>
        public void HideScreen()
        {
            UIFrameDebug.Log($"隐藏 {Name} 界面");
            OnHideScreen();
            IsVisible = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 派生类隐藏逻辑
        /// </summary>
        protected abstract void OnHideScreen();

        /// <summary>
        /// 进入动画播放完成
        /// </summary>
        protected void OnTransitionInFinished()
        {
            UIFrameDebug.Log($"界面 {Name} 进入动画播放完成 ");
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
            UIFrameDebug.Log($"界面 {Name} 退出动画播放完成 ");
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
