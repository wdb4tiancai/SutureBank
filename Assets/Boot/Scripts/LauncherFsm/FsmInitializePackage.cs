using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using SharePublic;

/// <summary>
/// 初始化资源包
/// </summary>
[UnityEngine.Scripting.Preserve]
internal class FsmInitializePackage : IStateNode
{
    private StateMachine m_Machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
#if UNITY_EDITOR
        Debug.Log("初始化资源包");
#endif
        LauncherEventDefine.LauncherStatesChange.SendEventMessage("初始化资源包！");
        LauncherBehaviour.Instance.StartCoroutine(InitPackage());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private IEnumerator InitPackage()
    {

        // 创建资源包裹类
        var package = YooAssets.TryGetPackage(AssetsVersion.AssetPackageName);
        if (package == null)
            package = YooAssets.CreatePackage(AssetsVersion.AssetPackageName);

        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;

#if GAME_PLATFORM_EDITOR
        var createParameters = new EditorSimulateModeParameters();
        createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(AssetsVersion.BuildPipeline, AssetsVersion.AssetPackageName);
        initializationOperation = package.InitializeAsync(createParameters);
#elif GAME_PLATFORM_ANDROID || GAME_PLATFORM_IOS

        string defaultHostServer = GetHostServerURL();
        string fallbackHostServer = GetHostServerURL();
        var createParameters = new HostPlayModeParameters();
        createParameters.DecryptionServices = null;
        createParameters.BuildinQueryServices = new GameQueryServices();
        createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);

        initializationOperation = package.InitializeAsync(createParameters);

#elif GAME_PLATFORM_WEIXIN
        string defaultHostServer = GetHostServerURL();
        string fallbackHostServer = GetHostServerURL();
        var createParameters = new WebPlayModeParameters();
        createParameters.DecryptionServices = null;
        createParameters.BuildinQueryServices = new GameQueryServices();
        createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
        initializationOperation = package.InitializeAsync(createParameters);
#endif


        yield return initializationOperation;

        // 如果初始化失败弹出提示界面
        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning($"初始化资源包失败 {initializationOperation.Error}");
            LauncherEventDefine.InitializeFailed.SendEventMessage();
        }
        else
        {
            var version = package.GetPackageVersion();
            Debug.Log($"初始资源包版本 : {version}");
            m_Machine.ChangeState<FsmUpdatePackageVersion>();
        }
    }

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        string hostServerIP = "http://10.61.84.115:7595";
        string appVersion = "1.0.0";

        return $"{hostServerIP}/{appVersion}";
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
}
