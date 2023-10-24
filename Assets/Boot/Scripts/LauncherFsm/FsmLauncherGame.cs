using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;

/// <summary>
/// 启动游戏
/// </summary>
internal class FsmLauncherGame : IStateNode
{
    private StateMachine m_Machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
        // 切换到主页面场景
        SceneEventDefine.ChangeToHomeScene.SendEventMessage();
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }
}