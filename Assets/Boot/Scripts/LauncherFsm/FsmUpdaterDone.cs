using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using SharePublic;

/// <summary>
/// 流程更新完毕
/// </summary>
[UnityEngine.Scripting.Preserve]
internal class FsmUpdaterDone : IStateNode
{
    private StateMachine m_Machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
#if UNITY_EDITOR
        Debug.Log("流程更新完毕");
#endif
        // 设置默认的资源包
        var gamePackage = YooAssets.GetPackage(AssetsVersion.AssetPackageName);
        YooAssets.SetDefaultPackage(gamePackage);
        m_Machine.ChangeState<FsmLoadHotUpdateDll>();
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }
}