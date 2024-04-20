using Game.Data;
using SharePublic;
using UniFramework.Machine;
using UnityEngine;
namespace Game.Main
{
    //配置表初始化
    public class FsmConfigInit : IStateNode
    {
        private StateMachine m_Machine;
        public void OnCreate(StateMachine machine)
        {
            m_Machine = machine;
        }

        public void OnEnter()
        {
            if (ShareDebug.IsDebugOpen())
            {
                Debug.Log("进入配置表初始化");
            }
            if (ConfigMgr.Instance.IsInit)
            {
                Debug.LogError("错误，重复初始化配置表");
                return;
            }
            ConfigMgr.Instance.Init();
        }

        public void OnExit()
        {
            if (ShareDebug.IsDebugOpen())
            {
                Debug.Log("进入配置表初始化");
            }
        }

        public void OnUpdate()
        {
        }
    }
}