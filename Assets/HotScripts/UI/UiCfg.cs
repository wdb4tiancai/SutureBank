using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;

public class UiCfg
{
    public const string MainUi = "MainUi";
    public const string LoadingUi = "LoadingUi";
    public const string LoginUi = "LoginUi";



    //ui配置表自动添加
    public static Dictionary<string, ScreenInfo> UIInfo = new Dictionary<string, ScreenInfo>();
    public static void AddUIInfo(int layerId, string name, string resPath, bool hideOnForegroundLost = true, bool isPopup = false)
    {
        if (UIInfo.ContainsKey(name))
        {
            Debug.LogError($"重复添加UI {name}");
        }
        ScreenInfo screenInfo = new ScreenInfo();
        UIInfo.Add(name, screenInfo);
        screenInfo.LayerId = layerId;
        screenInfo.Name = name;
        screenInfo.ResPath = resPath;
        screenInfo.HideOnForegroundLost = hideOnForegroundLost;
        screenInfo.IsPopup = isPopup;
    }
}
