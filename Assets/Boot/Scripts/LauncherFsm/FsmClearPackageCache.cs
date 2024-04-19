using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using SharePublic;

/// <summary>
/// 清理未使用的缓存文件
/// </summary>
[UnityEngine.Scripting.Preserve]
internal class FsmClearPackageCache : IStateNode
{
    private StateMachine m_Machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
#if UNITY_EDITOR
        Debug.Log("清理未使用的缓存文件");
#endif
        LauncherEventDefine.LauncherStatesChange.SendEventMessage("清理未使用的缓存文件！");
        var package = YooAssets.GetPackage(AssetsVersion.AssetPackageName);
        var operation = package.ClearUnusedCacheFilesAsync();
        operation.Completed += Operation_Completed;
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private void Operation_Completed(YooAsset.AsyncOperationBase obj)
    {
        m_Machine.ChangeState<FsmUpdaterDone>();
    }
}