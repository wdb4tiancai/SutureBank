using Cysharp.Threading.Tasks;
using Game.Scene;
using SharePublic;
using UniFramework.Event;
using UniFramework.Machine;
using UnityEngine;

namespace Game.Main
{
    public class EngineFsmMgr
    {
        //事件组
        private readonly EventGroup m_EventGroup;
        //状态机
        private readonly StateMachine m_Machine;
        public EngineFsmMgr()
        {
            //状态
            m_Machine = new StateMachine(this);
            InitFsmStateMachine();

            // 注册监听事件
            m_EventGroup = new EventGroup();
            InitEventGroup();

        }


        public void Start()
        {
            m_Machine.Run<FsmMgrInit>();
        }

        public void Update()
        {
            m_Machine.Update();
        }

        #region 状态机
        /// <summary>
        /// 初始化状态机
        /// </summary>
        private void InitFsmStateMachine()
        {
            m_Machine.AddNode<FsmMgrInit>();//管理器初始化
            m_Machine.AddNode<FsmConfigInit>();//配置表初始化
            m_Machine.AddNode<FsmUIInit>();//UI管理器初始化
            m_Machine.AddNode<FsmGame>();//游戏状态

        }

        #endregion

        #region 注册事件

        /// <summary>
        /// 初始化注册事件
        /// </summary>
        private void InitEventGroup()
        {
            //加载配置表完成
            m_EventGroup.AddListener<EngineFsmDefine.ConfigInitializeSucceed>(OnHandleEventMessage);
            //UI管理器初始化完成
            m_EventGroup.AddListener<EngineFsmDefine.UIInitializeSucceed>(OnHandleEventMessage);
        }

        /// <summary>
        /// 接收事件
        /// </summary>
        private void OnHandleEventMessage(IEventMessage message)
        {
            if (message is EngineFsmDefine.ConfigInitializeSucceed)
            {
                if (ShareDebug.IsDebugOpen())
                {
                    Debug.Log("加载配置表完成");
                }
                m_Machine.ChangeState<FsmUIInit>();
            }
            else if (message is EngineFsmDefine.UIInitializeSucceed)
            {
                if (ShareDebug.IsDebugOpen())
                {
                    Debug.Log("UI管理器初始化完成");
                }
                m_Machine.ChangeState<FsmGame>();
            }
            else
            {
                throw new System.NotImplementedException($"{message.GetType()}");
            }
        }

        #endregion
    }
}
