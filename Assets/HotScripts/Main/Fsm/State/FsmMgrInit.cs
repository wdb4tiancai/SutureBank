using Game.Audio;
using Game.Res;
using Game.Scene;
using SharePublic;
using UniFramework.Machine;
using UnityEngine;
namespace Game.Main
{
    //管理器初始化
    public class FsmMgrInit : IStateNode
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
                Debug.Log("进入管理器初始化");
            }
            ResMgr.Instance.Init();
            AudioMgr.Instance.Init();
            SceneMgr.Instance.Init();

            m_Machine.ChangeState<FsmConfigInit>();
        }

        public void OnExit()
        {
            if (ShareDebug.IsDebugOpen())
            {
                Debug.Log("退出管理器初始化");
            }
        }

        public void OnUpdate()
        {
        }
    }
}