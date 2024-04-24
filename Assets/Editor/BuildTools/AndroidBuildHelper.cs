
using UnityEditor;

public class AndroidBuildHelper
{
    [MenuItem("工具/打包工具/Android/打Android整包")]
    public static void AndroidBuild()
    {
        BuildHelper.GenerateAOTDllListFile();
        BuildHelper.GenerateHotDllListFile();
        BuildHelper.BuildUpdateAsset(BuildTarget.Android, "1.0.0");
        AndroidBuildApk();

    }


    private static void AndroidBuildApk()
    {
        if (!System.IO.Directory.Exists(BuildCfgHelper.BuildOutPutPath))
        {
            System.IO.Directory.CreateDirectory(BuildCfgHelper.BuildOutPutPath);
        }
        if (!System.IO.Directory.Exists(BuildCfgHelper.AndroidBuildOutPutPath))
        {
            System.IO.Directory.CreateDirectory(BuildCfgHelper.AndroidBuildOutPutPath);
        }
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, BuildCfgHelper.AndroidBuildOutPutPath + "/test.apk", BuildTarget.Android, BuildOptions.None);
    }

}
