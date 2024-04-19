using SharePublic;
using UniFramework.Machine;
using UnityEngine;

/// <summary>
/// 打开启动界面
/// </summary>
[UnityEngine.Scripting.Preserve]
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
        GameObject launcherUiObj = Resources.Load<GameObject>(AssetsVersion.LauncherUiPath);
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