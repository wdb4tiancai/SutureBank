﻿
namespace UIFramework
{
    public class PanelLayer : BaseLayer
    {
        /// <summary>
        /// 显示Screen
        /// </summary>
        public override void ShowScreen(BaseScreen screen, BaseScreenData screenData)
        {
            RegisterScreen(screen);
            screen.ShowScreen(screenData);
        }

        /// <summary>
        /// 关闭Screen
        /// </summary>
        public override void CloseScreen(BaseScreen screen)
        {
            screen.CloseScreen();
        }

        /// <summary>
        /// 检测panle是否显示
        /// </summary>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public bool IsPanelVisible(string panelName)
        {
            foreach (var item in m_ExistScreens)
            {
                if (item.Name.Equals(panelName))
                {
                    return item.IsVisible;
                }
            }
            return false;
        }
    }
}
