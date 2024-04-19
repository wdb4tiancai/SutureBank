using Cysharp.Threading.Tasks;
using Game.UI;
using Game.Config;
using Game.Audio;
using Game.Res;
using Game.Scene;
using Game.Util;

namespace Game
{
    public class EngineMgr : SingletonMgrBase<EngineMgr>
    {
        public bool m_IsInit = false;

        public override async UniTask Init()
        {
            await SceneMgr.Instance.Init();
            await ResMgr.Instance.Init();
            await ConfigMgr.Instance.Init();
            await AudioMgr.Instance.Init();
            await UiMgr.Instance.Init();
            m_IsInit = true;
            await SceneMgr.Instance.ChangeToLoginScene();
        }
        public override async UniTask Destroy()
        {
            await SceneMgr.Instance.Destroy();
            await ConfigMgr.Instance.Destroy();
            await UiMgr.Instance.Destroy();
            await AudioMgr.Instance.Destroy();
            await ResMgr.Instance.Destroy();
        }


        public override async UniTask Reset()
        {
            if (!m_IsInit)
            {
                return;
            }
            await SceneMgr.Instance.Reset();
            await ConfigMgr.Instance.Reset();
            await UiMgr.Instance.Reset();
            await AudioMgr.Instance.Reset();
            await ResMgr.Instance.Reset();
        }

        public override void Update(float dt)
        {
            if (!m_IsInit)
            {
                return;
            }
            SceneMgr.Instance.Update(dt);
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
