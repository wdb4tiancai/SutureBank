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

    public LauncherOperation()
    {
        // 注册监听事件
        m_EventGroup.AddListener<LauncherEventDefine.UserTryInitialize>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.UserBeginDownloadWebFiles>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.UserTryUpdatePackageVersion>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.UserTryUpdatePatchManifest>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.UserTryDownloadWebFiles>(OnHandleEventMessage);
        m_EventGroup.AddListener<LauncherEventDefine.ChangeToLoginScene>(OnHandleEventMessage);

        // 创建状态机
        m_Machine = new StateMachine(this);
        m_Machine.AddNode<FsmOpenLauncherUI>();//打开启动界面
        m_Machine.AddNode<FsmInitializePackage>();//初始化热更资源系统
        m_Machine.AddNode<FsmUpdatePackageVersion>();// 更新资源版本号
        m_Machine.AddNode<FsmUpdatePackageManifest>();// 更新资源清单
        m_Machine.AddNode<FsmCreatePackageDownloader>();// 创建文件下载器
        m_Machine.AddNode<FsmDownloadPackageFiles>();// 下载更新文件
        m_Machine.AddNode<FsmDownloadPackageOver>();// 下载完毕
        m_Machine.AddNode<FsmClearPackageCache>();// 清理未使用的缓存文件
        m_Machine.AddNode<FsmUpdaterDone>();// 流程更新完毕
        m_Machine.AddNode<FsmLoadHotUpdateDll>();// 加载热更代码
        m_Machine.AddNode<FsmLauncherGame>();// 启动游戏

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
        else if (message is LauncherEventDefine.ChangeToLoginScene)
        {
            m_EventGroup.RemoveAllListener();
            YooAssets.LoadSceneAsync("EngineScene");
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }
}