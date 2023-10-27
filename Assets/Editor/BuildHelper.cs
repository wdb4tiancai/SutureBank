
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using LitJson;

public class BuildHelper
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

    //public static string PackageExportPath = string.Format("{0}/BuildPacakage/", ProjectPath);

    //public static string HotUpdateAssetsPath = string.Format("{0}/HotUpdateAssets/", Application.dataPath);

    //public static string HotUpdateDllPath = string.Format("{0}HotUpdateDll/", HotUpdateAssetsPath);

    [MenuItem("工具/打整包")]
    public static void Test()
    {

    }

    [MenuItem("工具/生成AOTDll并复制进文件夹")]
    public static bool GenerateAOTDllListFile()
    {
        //先生成AOT文件
        PrebuildCommand.GenerateAll();

        //后拷贝文件
        string aotDllSourcePath = Path.Combine(ProjectPath, HotAotDllSourcePath, GetTargetPath());
        Debug.Log($"AOTDll来源目录 {aotDllSourcePath}");

        string aotDllTargetPath = Path.Combine(ProjectPath, HotAotDllTargetPath);
        Debug.Log($"AOTDll目标目录 {aotDllTargetPath}");

        // AOTDll配置文件名字
        string aotDllCfgFilePath = Path.Combine(aotDllTargetPath, HotAotDllCfgFileName);
        Debug.Log($"AOTDll配置文件名字 {aotDllCfgFilePath}");

        if (!Directory.Exists(aotDllTargetPath))
        {
            Debug.LogError($"AOTDll目标目录 {aotDllTargetPath} 不存在");
            return false;
        }

        if (!File.Exists(aotDllCfgFilePath))
        {
            Debug.LogError($"AOTDll配置文件 {aotDllCfgFilePath}不存在");
            return false;
        }

        //移除旧的AotDLL文件
        foreach (string fileNmae in Directory.GetFiles(aotDllTargetPath))
        {
            if (!fileNmae.Contains(".json"))
            {
                File.Delete(fileNmae);
            }
        }

        //根据配置列表拷贝新的AOTDLL
        List<string> aotDllFileName = JsonMapper.ToObject<List<string>>(File.ReadAllText(aotDllCfgFilePath));
        for (int i = 0; i < aotDllFileName.Count; i++)
        {
            string copySourcePath = Path.Combine(aotDllSourcePath, aotDllFileName[i]);
            string copyTagetPath = Path.Combine(aotDllTargetPath, aotDllFileName[i] + ".bytes");
            if (!File.Exists(copySourcePath))
            {
                Debug.LogError($"{copySourcePath} 不存在");
                return false;
            }
            File.Copy(copySourcePath, copyTagetPath);
        }


        AssetDatabase.Refresh();
        Debug.Log("生成AOTDll并复制进文件夹");
        return true;
    }

    [MenuItem("工具/生成HotDll件并复制进文件夹")]
    public static bool GenerateHotDllListFile()
    {
        //先生成AOT文件
        CompileDllCommand.CompileDllActiveBuildTarget();

        //后拷贝文件
        string gameDllSourcePath = Path.Combine(ProjectPath, HotGameDllSourcePath, GetTargetPath());
        Debug.Log($"热更代码Dll来源目录 {gameDllSourcePath}");

        string gameDllTargetPath = Path.Combine(ProjectPath, HotGameDllTargetPath);
        Debug.Log($"热更代码Dll目标目录 {gameDllTargetPath}");


        //热更代码Dll配置文件名字
        string gameDllCfgFilePath = Path.Combine(gameDllTargetPath, HotGameDllCfgFileName);
        Debug.Log($"热更代码Dll配置文件名字 {gameDllCfgFilePath}");

        if (!Directory.Exists(gameDllTargetPath))
        {
            Debug.LogError($"AOTDll目标目录 {gameDllTargetPath} 不存在");
            return false;
        }

        if (!File.Exists(gameDllCfgFilePath))
        {
            Debug.LogError($"热更代码Dll配置文件 {gameDllCfgFilePath}不存在");
            return false;
        }

        //移除旧的GameDLL文件
        foreach (string fileNmae in Directory.GetFiles(gameDllTargetPath))
        {
            if (!fileNmae.Contains(".json"))
            {
                File.Delete(fileNmae);
            }
        }

        //根据配置列表拷贝新的GameDLL
        List<string> gameDllFileName = JsonMapper.ToObject<List<string>>(File.ReadAllText(gameDllCfgFilePath));
        for (int i = 0; i < gameDllFileName.Count; i++)
        {
            string copySourcePath = Path.Combine(gameDllSourcePath, gameDllFileName[i]);
            string copyTagetPath = Path.Combine(gameDllTargetPath, gameDllFileName[i] + ".bytes");
            if (!File.Exists(copySourcePath))
            {
                Debug.LogError($"{copySourcePath} 不存在");
                return false;
            }
            File.Copy(copySourcePath, copyTagetPath);
        }
        AssetDatabase.Refresh();

        Debug.Log("生成HotDll件并复制进文件夹");

        return true;
    }


    // 获取各个平台的目录
    public static string GetTargetPath()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            return "Android/";
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            return "iOS/";
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
        {
            return "StandaloneWindows64/";
        }
        else
        {
            return "StandaloneWindows/";
        }
    }

}
