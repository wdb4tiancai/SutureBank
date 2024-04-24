
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using LitJson;
using System.Linq;
using YooAsset.Editor;
using SharePublic;

public class BuildHelper
{

    [MenuItem("工具/打包工具/步骤/生成AOTDll并复制进文件夹")]
    public static bool GenerateAOTDllListFile()
    {
        //先生成AOT文件
        PrebuildCommand.GenerateAll();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //后拷贝文件
        string aotDllSourcePath = Path.Combine(BuildCfgHelper.ProjectPath, BuildCfgHelper.HotAotDllSourcePath, GetTargetPath());
        Debug.Log($"AOTDll来源目录 {aotDllSourcePath}");

        string aotDllTargetPath = Path.Combine(BuildCfgHelper.ProjectPath, BuildCfgHelper.HotAotDllTargetPath);
        Debug.Log($"AOTDll目标目录 {aotDllTargetPath}");

        // AOTDll配置文件名字
        string aotDllCfgFilePath = Path.Combine(aotDllTargetPath, BuildCfgHelper.HotAotDllCfgFileName);
        Debug.Log($"AOTDll配置文件名字 {aotDllCfgFilePath}");

        if (!Directory.Exists(aotDllTargetPath))
        {
            Debug.LogError($"AOTDll目标目录 {aotDllTargetPath} 不存在");
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

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //写新的配置文件
        IReadOnlyList<string> assemblies = AOTGenericReferences.PatchedAOTAssemblyList;
        File.WriteAllText(aotDllCfgFilePath, JsonMapper.ToJson(assemblies.ToList()));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //拷贝新的AOTDLL
        for (int i = 0; i < assemblies.Count; i++)
        {
            string copySourcePath = Path.Combine(aotDllSourcePath, assemblies[i]);
            string copyTagetPath = Path.Combine(aotDllTargetPath, assemblies[i] + ".bytes");
            if (!File.Exists(copySourcePath))
            {
                Debug.LogError($"{copySourcePath} 不存在");
                return false;
            }
            File.Copy(copySourcePath, copyTagetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("生成AOTDll并复制进文件夹");
        return true;
    }

    [MenuItem("工具/打包工具/步骤/生成HotDll件并复制进文件夹")]
    public static bool GenerateHotDllListFile()
    {
        //先生成AOT文件
        CompileDllCommand.CompileDllActiveBuildTarget();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //后拷贝文件
        string gameDllSourcePath = Path.Combine(BuildCfgHelper.ProjectPath, BuildCfgHelper.HotGameDllSourcePath, GetTargetPath());
        Debug.Log($"热更代码Dll来源目录 {gameDllSourcePath}");

        string gameDllTargetPath = Path.Combine(BuildCfgHelper.ProjectPath, BuildCfgHelper.HotGameDllTargetPath);
        Debug.Log($"热更代码Dll目标目录 {gameDllTargetPath}");


        //热更代码Dll配置文件名字
        string gameDllCfgFilePath = Path.Combine(gameDllTargetPath, BuildCfgHelper.HotGameDllCfgFileName);
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
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

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
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("生成HotDll件并复制进文件夹");

        return true;
    }


    //生成热更资源
    public static void BuildUpdateAsset(BuildTarget buildTarget, string packageVersion)
    {
        Debug.Log($"开始构建 平台： {buildTarget} 版本号：{packageVersion}");

        var buildoutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        var streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();

        // 构建参数
        BuiltinBuildParameters buildParameters = new BuiltinBuildParameters();
        buildParameters.BuildOutputRoot = buildoutputRoot;
        buildParameters.BuildinFileRoot = streamingAssetsRoot;
        buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline.ToString();
        buildParameters.BuildTarget = buildTarget;
        buildParameters.BuildMode = EBuildMode.ForceRebuild;
        buildParameters.PackageName = AssetsVersion.AssetPackageName;
        buildParameters.PackageVersion = packageVersion;
        buildParameters.VerifyBuildingResult = true;
        buildParameters.FileNameStyle = EFileNameStyle.HashName;
        buildParameters.BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyByTags;
        buildParameters.BuildinFileCopyParams = string.Empty;
        buildParameters.EncryptionServices = null;
        buildParameters.CompressOption = ECompressOption.LZ4;

        // 执行构建
        BuiltinBuildPipeline pipeline = new BuiltinBuildPipeline();
        var buildResult = pipeline.Run(buildParameters, true);
        if (buildResult.Success)
        {
            Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
        }
        else
        {
            Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
        {
            return "WebGL/";
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
