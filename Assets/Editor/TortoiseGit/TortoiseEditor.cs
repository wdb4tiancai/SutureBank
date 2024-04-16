using UnityEditor;
using UnityEngine;

public class TortoiseEditor
{
    public static string tortoiseGitPath = @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe";


    [MenuItem("工具/TortoiseGit/拉取 _F10")]
    public static void GitAssetsPull()
    {
        TortoiseGit.GitCommand(GitType.Pull, Application.dataPath + "/../", tortoiseGitPath);
    }



    [MenuItem("工具/TortoiseGit/提交 _F11")]
    public static void GitAssetsCommit()
    {
        TortoiseGit.GitCommand(GitType.Commit, Application.dataPath + "/../", tortoiseGitPath);
    }

    [MenuItem("工具/TortoiseGit/推送 _F12")]
    public static void GitAssetPush()
    {
        TortoiseGit.GitCommand(GitType.Push, Application.dataPath + "/../", tortoiseGitPath);
    }
}