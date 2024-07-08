using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
namespace Game.UI
{
    public class UiCfg
    {
        public const string MainUi = "MainUi";
        public const string LoadingUi = "LoadingUi";
        public const string LoginUi = "LoginUi";



        //ui配置表自动添加
        public static Dictionary<string, BaseScreenInfo> UIInfo = new Dictionary<string, BaseScreenInfo>();
        public static void AddUIInfo(int layerId, string name, string resPath, bool hideOnForegroundLost = true, bool isPopup = false)
        {
            if (UIInfo.ContainsKey(name))
            {
                Debug.LogError($"重复添加UI {name}");
            }
            BaseScreenInfo screenInfo = new BaseScreenInfo();
            UIInfo.Add(name, screenInfo);
            screenInfo.SetLayerId(layerId);
            screenInfo.SetName(name);
            screenInfo.SetResPath(resPath);
            screenInfo.SetIsHideOnForegroundLost(hideOnForegroundLost);
            screenInfo.SetIsPopup(isPopup);
        }
    }
}
