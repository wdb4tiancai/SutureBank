namespace UIFrameWork
{

    /// <summary>
    /// 用于控制窗口历史记录和队列的条目
    /// </summary>
    public struct WindowHistoryEntry
    {
        BaseScreen m_Screen;
        BaseScreenData m_ScreenData;
        bool m_IsAni;

        public BaseScreen Screen { get => m_Screen; }

        public WindowHistoryEntry(BaseScreen screen, BaseScreenData screenData, bool isAni)
        {
            m_Screen = screen;
            m_ScreenData = screenData;
            m_IsAni = isAni;
        }

        public void Show()
        {
            m_Screen.ShowScreen(m_ScreenData, m_IsAni);
        }
    }
}
