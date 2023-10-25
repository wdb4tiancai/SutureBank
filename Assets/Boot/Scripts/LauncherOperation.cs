using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Event;
using YooAsset;

public class LauncherOperation : GameAsyncOperation
{
    //启动器状态
    private enum LauncherSteps
    {
        None,//未开始
        Update,//更新
        Done,//完成
    }
    //事件组
    private readonly EventGroup m_EventGroup = new EventGroup();
    //状态机
    private readonly StateMachine m_Machine;
    //启动器步骤
    private LauncherSteps m_Steps = LauncherSteps.None;

    public LauncherOperation(string launcherUiPath, string packageName, string buildPipeline, EPlayMode playMode)
    {
        // 注册监听事件
        m_EventGroup.AddListener<LauncherEventDefine.UserTryInitialize>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);

        // 创建状态机
        m_Machine = new StateMachine(this);
        m_Machine.AddNode<FsmOpenLauncherUI>();
        m_Machine.AddNode<FsmInitializePackage>();
        m_Machine.AddNode<FsmUpdatePackageVersion>();
        m_Machine.AddNode<FsmUpdatePackageManifest>();
        m_Machine.AddNode<FsmCreatePackageDownloader>();
        m_Machine.AddNode<FsmDownloadPackageFiles>();
        m_Machine.AddNode<FsmDownloadPackageOver>();
        m_Machine.AddNode<FsmClearPackageCache>();
        m_Machine.AddNode<FsmUpdaterDone>();
        m_Machine.AddNode<FsmLoadHotUpdateDll>();
        m_Machine.AddNode<FsmLauncherGame>();

        //启动器ui路径
        m_Machine.SetBlackboardValue("LauncherUiPath", launcherUiPath);
        //热更资源的PackageName
        m_Machine.SetBlackboardValue("PackageName", packageName);
        //资源的模式
        m_Machine.SetBlackboardValue("PlayMode", playMode);
        //构建管线类型
        m_Machine.SetBlackboardValue("BuildPipeline", buildPipeline);

    }
    protected override void OnStart()
    {
        m_Steps = LauncherSteps.Update;
        m_Machine.Run<FsmOpenLauncherUI>();
    }
    protected override void OnUpdate()
    {
        if (m_Steps == LauncherSteps.None || m_Steps == LauncherSteps.Done)
            return;

        if (m_Steps == LauncherSteps.Update)
        {
            m_Machine.Update();

        }
    }
    protected override void OnAbort()
    {
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is LauncherEventDefine.UserTryInitialize)
        {
            m_Machine.ChangeState<FsmInitializePackage>();
        }
        else if (message is LauncherEventDefine.UserBeginDownloadWebFiles)
        {
            m_Machine.ChangeState<FsmDownloadPackageFiles>();
        }
        else if (message is LauncherEventDefine.UserTryUpdatePackageVersion)
        {
            m_Machine.ChangeState<FsmUpdatePackageVersion>();
        }
        else if (message is LauncherEventDefine.UserTryUpdatePatchManifest)
        {
            m_Machine.ChangeState<FsmUpdatePackageManifest>();
        }
        else if (message is LauncherEventDefine.UserTryDownloadWebFiles)
        {
            m_Machine.ChangeState<FsmCreatePackageDownloader>();
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }
}