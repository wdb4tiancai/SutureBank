using Game.UI;
using SharePublic;
using UniFramework.Machine;
using UnityEngine;
namespace Game.Main
{
    //UI管理器初始化
    public class FsmUIInit : IStateNode
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
                Debug.Log("进入UI管理器初始化");
            }
            if (UiMgr.Instance.IsInit)
            {
                Debug.LogError("错误，重复初始化UI管理器");
                return;
            }
            UiMgr.Instance.Init();
        }

        public void OnExit()
        {
            if (ShareDebug.IsDebugOpen())
            {
                Debug.Log("退出UI管理器初始化");
            }
        }

        public void OnUpdate()
        {
        }
    }
}