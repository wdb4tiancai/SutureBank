using Cysharp.Threading.Tasks;
using Game.Scene;
using SharePublic;
using UniFramework.Machine;
using UnityEngine;
namespace Game.Main
{
    //游戏状态
    public class FsmGame : IStateNode
    {
        private StateMachine m_Machine;
        public void OnCreate(StateMachine machine)
        {
            m_Machine = machine;
        }

        public void OnEnter()
        {
            SceneMgr.Instance.ChangeToLoginScene().Forget();
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