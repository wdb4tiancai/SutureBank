
using UniFramework.Event;
namespace Game.Main
{
    //启动器状态机事件
    public class EngineFsmDefine
    {
        /// <summary>
        /// 配置表初始化成功
        /// </summary>
        public class ConfigInitializeSucceed : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new ConfigInitializeSucceed();
                UniEvent.SendMessage(msg);
            }
        }

        /// <summary>
        /// UI初始化成功
        /// </summary>
        public class UIInitializeSucceed : IEventMessage
        {
            public static void SendEventMessage()
            {
                var msg = new UIInitializeSucceed();
                UniEvent.SendMessage(msg);
            }
        }
    }
}
