using System;
using System.Collections.Generic;
using UnityEngine;
namespace UIFramework
{
    public class WindowLayer : BaseLayer
    {
        /// <summary>
        /// 当前窗口
        /// </summary>
        private BaseScreen m_CurrentScreen { get; set; }

        /// <summary>
        /// 窗口历史记录
        /// </summary>
        private Stack<WindowHistoryEntry> m_WindowHistory;

        /// <summary>
        /// 屏幕转换是否正在进行
        /// </summary>
        private HashSet<BaseScreen> m_ScreensTransitioning;

        /// <summary>
        /// 请求屏幕阻挡锁
        /// </summary>
        public event Action RequestScreenBlock;

        /// <summary>
        /// 请求屏幕阻挡解锁
        /// </summary>
        public event Action RequestScreenUnblock;

        /// <summary>
        /// 屏幕转换的数量
        /// </summary>
        private bool IsScreenTransitionInProgress
        {
            get { return m_ScreensTransitioning.Count != 0; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            m_WindowHistory = new Stack<WindowHistoryEntry>();
            m_ScreensTransitioning = new HashSet<BaseScreen>();
        }

        /// <summary>
        /// 屏幕打开注册
        /// </summary>
        /// <param name="screen"></param>
        protected override void ProcessScreenRegister(BaseScreen screen)
        {
            base.ProcessScreenRegister(screen);
            screen.InTransitionFinished += OnInAnimationFinished;
            screen.OutTransitionFinished += OnOutAnimationFinished;
        }

        /// <summary>
        /// 屏幕关闭取消注册
        /// </summary>
        /// <param name="screen"></param>
        protected override void ProcessScreenUnregister(BaseScreen screen)
        {
            base.ProcessScreenUnregister(screen);
            screen.InTransitionFinished -= OnInAnimationFinished;
            screen.OutTransitionFinished -= OnOutAnimationFinished;
        }

        /// <summary>
        /// 显示Screen
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="screenData"></param>
        public override void ShowScreen(BaseScreen screen, BaseScreenData screenData)
        {
            //注册当前界面
            RegisterScreen(screen);
            //去显示逻辑
            DoShow(new WindowHistoryEntry(screen, screenData));
        }

        /// <summary>
        /// 关闭Screen
        /// </summary>
        /// <param name="screen"></param>
        public override void CloseScreen(BaseScreen screen)
        {
            if (screen == m_CurrentScreen)
            {
                //历史界面弹出当前UI
                m_WindowHistory.Pop();
                //当前界面添加到待转换的列表
                AddTransition(screen);
                //关闭当前界面
                screen.CloseScreen();
                //设置当前窗口
                m_CurrentScreen = null;

                if (m_WindowHistory.Count > 0)
                {
                    //如果有历史窗口，显示历史
                    ShowPreviousInHistory();
                }
            }
            else
            {
                Debug.LogError(
                    string.Format(
                        " 关闭请求的界面{0}，但这不是当前打开的一个({1})!忽略请求。", screen.Name,
                        m_CurrentScreen != null ? m_CurrentScreen.Name : "当前界面不存在"));
            }
        }

        /// <summary>
        /// 隐藏所有Screen
        /// </summary>
        public override void CloseAll()
        {
            base.CloseAll();
            //注销当前场景
            m_CurrentScreen = null;
            //清除历史记录
            m_WindowHistory.Clear();
        }



        /// <summary>
        /// 显示历史记录
        /// </summary>
        private void ShowPreviousInHistory()
        {
            if (m_WindowHistory.Count > 0)
            {
                //取出历史记录
                WindowHistoryEntry window = m_WindowHistory.Pop();
                //显示历史记录
                DoShow(window);
            }
        }


        /// <summary>
        /// 显示指定screen
        /// </summary>
        /// <param name="windowEntry"></param>
        private void DoShow(WindowHistoryEntry windowEntry)
        {
            if (m_CurrentScreen == windowEntry.Screen)
            {
                Debug.LogWarning("打开的界面和当前界面一致  " + m_CurrentScreen.Name);
            }
            else if (m_CurrentScreen != null
                     && m_CurrentScreen.HideOnForegroundLost
                     && !m_CurrentScreen.IsPopup)
            {
                //当前窗口存在，且要隐藏，新打开的界面不是弹出窗口
                //隐藏当前窗口
                m_CurrentScreen.HideScreen();
            }
            //将要打开的界面加到历史窗口
            m_WindowHistory.Push(windowEntry);
            //将要打开的界面添加到转换窗口
            AddTransition(windowEntry.Screen);

            //显示当前窗口
            windowEntry.Show();
            //设置当前窗口
            m_CurrentScreen = windowEntry.Screen;
        }

        /// <summary>
        /// 进入动画完成
        /// </summary>
        /// <param name="screen"></param>
        private void OnInAnimationFinished(BaseScreen screen)
        {
            RemoveTransition(screen);
        }

        /// <summary>
        /// 关闭动画完成
        /// </summary>
        /// <param name="screen"></param>
        private void OnOutAnimationFinished(BaseScreen screen)
        {
            RemoveTransition(screen);
        }

        /// <summary>
        /// 加入待转换的列表
        /// </summary>
        /// <param name="screen"></param>
        private void AddTransition(BaseScreen screen)
        {

            if (!IsScreenTransitionInProgress)
            {
                if (RequestScreenBlock != null)
                {
                    RequestScreenBlock();
                }
            }
            m_ScreensTransitioning.Add(screen);
        }

        /// <summary>
        /// 删除待转换的列表
        /// </summary>
        /// <param name="screen"></param>
        private void RemoveTransition(BaseScreen screen)
        {
            m_ScreensTransitioning.Remove(screen);
            if (!IsScreenTransitionInProgress)
            {
                if (RequestScreenUnblock != null)
                {
                    RequestScreenUnblock();
                }
            }
        }
    }
}
