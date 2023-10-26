namespace UIFramework
{

    /// <summary>
    /// 用于控制窗口历史记录和队列的条目
    /// </summary>
    public struct WindowHistoryEntry
    {
        BaseScreen m_Screen;
        BaseScreenData m_ScreenData;

        public BaseScreen Screen { get => m_Screen; }

        public WindowHistoryEntry(BaseScreen screen, BaseScreenData screenData)
        {
            m_Screen = screen;
            m_ScreenData = screenData;
        }

        public void Show()
        {
            m_Screen.ShowScreen(m_ScreenData);
        }
    }
}
