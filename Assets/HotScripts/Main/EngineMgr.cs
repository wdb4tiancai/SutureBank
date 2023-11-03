using Cysharp.Threading.Tasks;
using Game.UI;
using UnityEngine;

namespace Game
{
    public class EngineMgr
    {
        public bool m_IsInit = false;
        public static Engine Engine;//启动器管理对象

        public async UniTask Init(Engine engine)
        {
            Engine = engine;
            await ConfigMgr.Instance.Init(engine);
            await UiMgr.Instance.Init(engine);

            m_IsInit = true;
        }
        public void Destroy()
        {
            ConfigMgr.Instance.Destroy();
            UiMgr.Instance.Destroy();
        }
        public void Update(float dt)
        {
            if (!m_IsInit)
            {
                return;
            }
            UiMgr.Instance.Update(dt);
        }

        public void ApplicationFocus(bool focus)
        {
            if (!m_IsInit)
            {
                return;
            }
        }
    }
}
