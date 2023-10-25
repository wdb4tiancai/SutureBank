using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;

/// <summary>
/// 流程更新完毕
/// </summary>
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
        var packageName = (string)m_Machine.GetBlackboardValue("PackageName");
        var gamePackage = YooAssets.GetPackage(packageName);
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