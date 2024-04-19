using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using SharePublic;

/// <summary>
/// 更新资源清单
/// </summary>
[UnityEngine.Scripting.Preserve]
public class FsmUpdatePackageManifest : IStateNode
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
        Debug.Log("更新资源清单");
#endif
        LauncherEventDefine.LauncherStatesChange.SendEventMessage("更新资源清单！");
        LauncherBehaviour.Instance.StartCoroutine(UpdateManifest());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private IEnumerator UpdateManifest()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var packageVersion = (string)m_Machine.GetBlackboardValue("PackageVersion");
        var package = YooAssets.GetPackage(AssetsVersion.AssetPackageName);
        bool savePackageVersion = true;
        var operation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            LauncherEventDefine.PatchManifestUpdateFailed.SendEventMessage();
            yield break;
        }
        else
        {
            m_Machine.ChangeState<FsmClearPackageCache>();
        }
    }
}