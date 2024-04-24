using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using HybridCLR;
using System.Reflection;
using LitJson;
using UniFramework.Machine;
using Cysharp.Threading.Tasks;


/// <summary>
/// 加载热更代码
/// </summary>
[UnityEngine.Scripting.Preserve]
internal class FsmLoadHotUpdateDll : IStateNode
{
    private StateMachine m_Machine;
    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
        LauncherEventDefine.LauncherStatesChange.SendEventMessage("加载热更代码！");
        Debug.Log("加载热更代码");
#if UNITY_EDITOR
        m_Machine.ChangeState<FsmLauncherGame>();
#else
        LoadHotUpdateDll().Forget();
#endif


    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    //加载DLL代码
    private async UniTaskVoid LoadHotUpdateDll()
    {
        await LoadMetadataForAOTAssemblies();
        await LoadHotUpdateAssemblies();
        await UniTask.CompletedTask;
        m_Machine.ChangeState<FsmLauncherGame>();
    }

    //加载Aot依赖
    private async UniTask LoadMetadataForAOTAssemblies()
    {
        Debug.Log("加载AOT依赖");
        HomologousImageMode mode = HomologousImageMode.SuperSet;

        AssetHandle cfgHandle = YooAssets.LoadAssetAsync("AotDllCfg", typeof(TextAsset));
        await cfgHandle.ToUniTask();

        List<string> aotDllFiles = JsonMapper.ToObject<List<string>>(((TextAsset)cfgHandle.AssetObject).ToString());
        foreach (var aotDllFile in aotDllFiles)
        {
            AssetHandle aotHandle = YooAssets.LoadAssetAsync(aotDllFile, typeof(TextAsset));
            await aotHandle.ToUniTask();
            if (aotHandle.Status != EOperationStatus.Succeed)
            {
                Debug.Log($"AOT资源加载 {aotDllFile} 失败");
                return;
            }
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(((TextAsset)aotHandle.AssetObject).bytes, mode);
            aotHandle.Release();
            if (err != LoadImageErrorCode.OK)
            {
                Debug.Log($"AOT资源加载 {aotDllFile} 失败");
                return;
            }
            Debug.Log($"AOT资源加载:{aotDllFile}. mode:{mode} ret:{err}");
        }
        await UniTask.CompletedTask;
    }

    //加载代码
    private async UniTask LoadHotUpdateAssemblies()
    {
        Debug.Log("加载代码");
        AssetHandle cfgHandle = YooAssets.LoadAssetAsync("HotUpdateDllCfg", typeof(TextAsset));
        await cfgHandle.ToUniTask();

        List<string> hotDllFiles = JsonMapper.ToObject<List<string>>(((TextAsset)cfgHandle.AssetObject).ToString());
        foreach (var hotDllFile in hotDllFiles)
        {
            AssetHandle hotDllHandle = YooAssets.LoadAssetAsync(hotDllFile, typeof(TextAsset));
            await hotDllHandle.ToUniTask();
            if (hotDllHandle.Status != EOperationStatus.Succeed)
            {
                Debug.Log($"加载热更新Dll: {hotDllFile} 失败");
                return;
            }
            Assembly.Load(((TextAsset)hotDllHandle.AssetObject).bytes);
            hotDllHandle.Release();
            Debug.Log($"加载热更新Dll: {hotDllFile} 成功");
        }
        await UniTask.CompletedTask;
    }
}