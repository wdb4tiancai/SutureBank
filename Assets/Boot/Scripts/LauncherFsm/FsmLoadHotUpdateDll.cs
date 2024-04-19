using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using HybridCLR;
using System.Reflection;
using LitJson;
using UniFramework.Machine;

/// <summary>
/// 加载热更代码
/// </summary>
[UnityEngine.Scripting.Preserve]
internal class FsmLoadHotUpdateDll : IStateNode
{
    private StateMachine m_Machine;
    private bool m_IsLoadDllSucceed = true;
    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
        m_IsLoadDllSucceed = true;
        LauncherEventDefine.LauncherStatesChange.SendEventMessage("加载热更代码！");
        Debug.Log("加载热更代码");
#if UNITY_EDITOR
        m_Machine.ChangeState<FsmLauncherGame>();
#else
        LauncherBehaviour.Instance.StartCoroutine(LoadHotUpdateDll());
#endif


    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    //加载DLL代码
    private IEnumerator LoadHotUpdateDll()
    {
        yield return LoadMetadataForAOTAssemblies();
        if (m_IsLoadDllSucceed == false)
        {
            yield break;
        }
        yield return LoadHotUpdateAssemblies();
        if (m_IsLoadDllSucceed == false)
        {
            yield break;
        }
        m_Machine.ChangeState<FsmLauncherGame>();
    }

    //加载Aot依赖
    private IEnumerator LoadMetadataForAOTAssemblies()
    {
        Debug.Log("加载AOT依赖");
        HomologousImageMode mode = HomologousImageMode.SuperSet;

        var cfgHandle = YooAssets.LoadAssetSync("AotDllCfg", typeof(TextAsset));
        yield return cfgHandle;

        List<string> allDllNames = JsonMapper.ToObject<List<string>>(((TextAsset)cfgHandle.AssetObject).ToString());
        foreach (var name in allDllNames)
        {
            var dataHandle = YooAssets.LoadAssetSync(name, typeof(TextAsset));
            yield return dataHandle;
            if (dataHandle.Status != EOperationStatus.Succeed)
            {
                Debug.Log("AOT资源加载失败" + name);
                m_IsLoadDllSucceed = false;
                yield break;
            }
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(((TextAsset)dataHandle.AssetObject).bytes, mode);
            dataHandle.Release();
            if (err != LoadImageErrorCode.OK)
            {
                Debug.Log("AOT加载失败" + name);
                m_IsLoadDllSucceed = false;
                yield break;
            }
            Debug.Log($"LoadMetadataForAOTAssembly:{name}. mode:{mode} ret:{err}");
        }
        yield return true;
    }

    //加载代码
    private IEnumerator LoadHotUpdateAssemblies()
    {
        Debug.Log("加载代码");
        var cfgHandle = YooAssets.LoadAssetSync("HotUpdateDllCfg", typeof(TextAsset));
        yield return cfgHandle;

        List<string> allDllNames = JsonMapper.ToObject<List<string>>(((TextAsset)cfgHandle.AssetObject).ToString());
        foreach (var name in allDllNames)
        {
            var dataHandle = YooAssets.LoadAssetSync(name, typeof(TextAsset));
            yield return dataHandle;
            if (dataHandle.Status != EOperationStatus.Succeed)
            {
                Debug.Log("资源加载失败" + name);
                m_IsLoadDllSucceed = false;
                yield break;
            }
            Assembly assembly = Assembly.Load(((TextAsset)dataHandle.AssetObject).bytes);
            dataHandle.Release();
            Debug.Log(assembly.GetTypes());
            Debug.Log($"加载热更新Dll:{name}");
        }
        yield return true;
    }
}