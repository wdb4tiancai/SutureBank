﻿using Cysharp.Threading.Tasks;
using Game.UI;
using Game.Data;
using Game.Audio;
using Game.Res;
using Game.Scene;
using Game.Util;

namespace Game.Main
{
    public class EngineMgr : SingletonMgrBase<EngineMgr>
    {
        private EngineFsmMgr m_EngineFsmMgr;

        public override void Init()
        {
            m_EngineFsmMgr = new EngineFsmMgr();
        }

        public void Start()
        {
            m_EngineFsmMgr.Start();
        }

        public override void Destroy()
        {
            SceneMgr.Instance.Destroy();
            ConfigMgr.Instance.Destroy();
            UiMgr.Instance.Destroy();
            AudioMgr.Instance.Destroy();
            ResMgr.Instance.Destroy();
        }


        public override void Reset()
        {
            SceneMgr.Instance.Reset();
            ConfigMgr.Instance.Reset();
            UiMgr.Instance.Reset();
            AudioMgr.Instance.Reset();
            ResMgr.Instance.Reset();
        }

        public override void Update(float dt)
        {
            m_EngineFsmMgr.Update();
            SceneMgr.Instance.Update(dt);
            UiMgr.Instance.Update(dt);
        }

        public void ApplicationFocus(bool focus)
        {
        }

    }
}
