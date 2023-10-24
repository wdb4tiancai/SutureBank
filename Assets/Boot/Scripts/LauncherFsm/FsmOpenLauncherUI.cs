using UniFramework.Machine;
using UnityEngine;

/// <summary>
/// 打开启动界面
/// </summary>
public class FsmOpenLauncherUI : IStateNode
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
        Debug.Log("打开启动界面");
#endif
        // 加载更新页面
        string launcherUiPath = (string)m_Machine.GetBlackboardValue("LauncherUiPath");
        var launcherUiObj = Resources.Load<GameObject>(launcherUiPath);
        GameObject.Instantiate(launcherUiObj);
        m_Machine.Run<FsmInitializePackage>();
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }
}