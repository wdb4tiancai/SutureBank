using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;

/// <summary>
/// 下载更新文件
/// </summary>
public class FsmDownloadPackageFiles : IStateNode
{
    //状态机
    private StateMachine m_Machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
#if UNITY_EDITOR
        Debug.Log("开始下载补丁文件");
#endif
        LauncherEventDefine.LauncherStatesChange.SendEventMessage("开始下载补丁文件！");
        LauncherBehaviour.Instance.StartCoroutine(BeginDownload());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private IEnumerator BeginDownload()
    {
        var downloader = (ResourceDownloaderOperation)m_Machine.GetBlackboardValue("Downloader");
        downloader.OnDownloadErrorCallback = LauncherEventDefine.WebFileDownloadFailed.SendEventMessage;
        downloader.OnDownloadProgressCallback = LauncherEventDefine.DownloadProgressUpdate.SendEventMessage;
        downloader.BeginDownload();
        yield return downloader;

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
            yield break;

        m_Machine.ChangeState<FsmDownloadPackageOver>();
    }
}