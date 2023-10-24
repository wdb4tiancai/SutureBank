using UniFramework.Machine;
using UnityEngine;

/// <summary>
/// ����������
/// </summary>
public class FsmOpenLauncherUI : IStateNode
{
    //״̬��
    private StateMachine m_Machine;
    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
#if UNITY_EDITOR
        Debug.Log("����������");
#endif
        // ���ظ���ҳ��
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