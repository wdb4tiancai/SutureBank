using UnityEngine;
using System.IO;

public class BuildCfgHelper
{

    /// <summary>
    /// 工程目录路径，Assets上一层
    /// </summary>
    public static string ProjectPath = Application.dataPath.Replace("Assets", "");

    /// <summary>
    /// AOTDll来源目录
    /// </summary>
    public static string HotAotDllSourcePath = "HybridCLRData/AssembliesPostIl2CppStrip/";
    /// <summary>
    /// 热更代码Dll来源目录
    /// </summary>
    public static string HotGameDllSourcePath = "HybridCLRData/HotUpdateDlls/";

    /// <summary>
    /// AOTDll目标目录
    /// </summary>
    public static string HotAotDllTargetPath = Path.Combine(Application.dataPath, "HotAssets/AOTDll/");
    /// <summary>
    /// 热更代码Dll目标目录
    /// </summary>
    public static string HotGameDllTargetPath = Path.Combine(Application.dataPath, "HotAssets/HotUpdateDll/");

    /// <summary>
    /// AOTDll配置文件名字
    /// </summary>
    public const string HotAotDllCfgFileName = "AotDllCfg.json";
    /// <summary>
    ///热更代码Dll配置文件名字
    /// </summary>
    public const string HotGameDllCfgFileName = "HotUpdateDllCfg.json";

    //打包上级目录
    public static string BuildOutPutPath = ProjectPath + "/OutPut";

    //安卓打包目录
    public static string AndroidBuildOutPutPath = BuildOutPutPath + "/Apk";

    //微信打包目录
    public static string WeiXinMiniGameBuildOutPutPath = BuildOutPutPath + "/WeiXin";

}
