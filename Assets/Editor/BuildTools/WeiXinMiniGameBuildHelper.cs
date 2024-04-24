
using UnityEditor;
using UnityEngine;
using WeChatWASM;
using static WeChatWASM.WXConvertCore;
#if GAME_PLATFORM_WEIXIN
public class WeiXinMiniGameBuildHelper
{
    [MenuItem("工具/打包工具/微信小游戏/微信小游戏整包")]
    public static void WeiXinMiniGameBuild()
    {
        BuildHelper.GenerateAOTDllListFile();
        BuildHelper.GenerateHotDllListFile();
        BuildHelper.BuildUpdateAsset(BuildTarget.WebGL, "1.0.0");

        EditWeiXinProject();
        WeiXinMiniGameExport();
    }

    [MenuItem("工具/打包工具/微信小游戏/微信小游戏热更")]
    public static void WeiXinMiniGameUpdateAsset()
    {
        BuildHelper.GenerateHotDllListFile();
        BuildHelper.BuildUpdateAsset(BuildTarget.WebGL, "1.0.0");
    }


    //编辑微信工程
    private static void EditWeiXinProject()
    {
        Debug.Log("在微信工程导出之前开始");
        WXEditorScriptObject config = AssetDatabase.LoadAssetAtPath<WXEditorScriptObject>("Assets/WX-WASM-SDK-V2/Editor/MiniGameConfig.asset");
        config.ProjectConf.CDN = "http://10.61.84.115:7595";
        config.ProjectConf.projectName = "TestDemo";
        config.ProjectConf.DST = BuildCfgHelper.WeiXinMiniGameBuildOutPutPath;
        //config.ProjectConf.maxStorage = 900;
        config.ProjectConf.MemorySize = 450;
        config.ProjectConf.needCheckUpdate = true;

        config.CompileOptions.DevelopBuild = false;
        config.CompileOptions.AutoProfile = false;
        config.CompileOptions.ScriptOnly = false;
        config.CompileOptions.Il2CppOptimizeSize = false;
        config.CompileOptions.profilingFuncs = true;
        config.CompileOptions.ProfilingMemory = false;
        config.CompileOptions.Webgl2 = true;
        config.CompileOptions.DeleteStreamingAssets = true;
        config.CompileOptions.CleanBuild = false;
        config.CompileOptions.fbslim = true;
        config.CompileOptions.showMonitorSuggestModal = false;
        config.CompileOptions.enableProfileStats = false;
        config.CompileOptions.enableRenderAnalysis = false;
        config.CompileOptions.enableIOSPerformancePlus = true;
        AssetDatabase.Refresh();
        Debug.Log("在微信工程导出之前结束");
    }

    //微信工程导出
    private static void WeiXinMiniGameExport()
    {
        if (!System.IO.Directory.Exists(BuildCfgHelper.BuildOutPutPath))
        {
            System.IO.Directory.CreateDirectory(BuildCfgHelper.BuildOutPutPath);
        }
        if (!System.IO.Directory.Exists(BuildCfgHelper.WeiXinMiniGameBuildOutPutPath))
        {
            System.IO.Directory.CreateDirectory(BuildCfgHelper.WeiXinMiniGameBuildOutPutPath);
        }
        WXExportError exportError = WXConvertCore.DoExport();
        if (exportError != WXExportError.SUCCEED)
        {
            Debug.LogError("微信工程导出失败");
            return;
        }
    }


}
#endif

