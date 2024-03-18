
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class ConfigHelper
{


    [MenuItem("工具/配置表/生成配置表")]
    public static bool GenExcelConfig()
    {
        Process p = new Process();
#if UNITY_EDITOR_WIN
        p.StartInfo.FileName = Application.dataPath + "/../ExcelDatas/gen.bat";
#else
        p.StartInfo.FileName = Application.dataPath + "/../ExcelDatas/gen.sh";
#endif

        p.StartInfo.Arguments = "";
        p.StartInfo.CreateNoWindow = false;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.WorkingDirectory = Application.dataPath + "/../ExcelDatas/";
        p.Start();
        p.WaitForExit();
        string output = p.StandardOutput.ReadToEnd();
        UnityEngine.Debug.Log($"配置表生成\n {output}");
        if (p.ExitCode != 0)
        {
            UnityEngine.Debug.LogError("生成配置表失败");
            p.Close();
            return false;
        }
        else
        {
            UnityEngine.Debug.LogError("生成配置表成功");
        }
        p.Close();
        AssetDatabase.Refresh();
        return true;
    }





}
