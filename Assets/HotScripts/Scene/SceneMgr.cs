﻿using Cysharp.Threading.Tasks;
using Game.Res;
using Game.UI;
using Game.Util;
using UIFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scene
{
    public class SceneMgr : SingletonBase<SceneMgr>
    {
        private Engine m_Engine;
        private bool m_IsInit = false;
        private BaseScene m_CurScene;
        private FrameCtrl m_CurFrameCtrl;

        public void Init(Engine engine)
        {
            m_Engine = engine;
            m_IsInit = true;
        }
        public void Destroy()
        {
            if (!m_IsInit)
            {
                return;
            }
        }
        public void Update(float dt)
        {
            if (!m_IsInit)
            {
                return;
            }
            m_CurFrameCtrl?.Update(dt);
        }


        //切换到登录场景
        public async void ChangeToLoginScene()
        {
            //设置目标帧率
            SetTargetFrame(60);
            //打开与关闭ui
            await OpenLoadingUIAndLoadLoadingScene();
            LoadingScene loadingScene = new LoadingScene();
            LoadToLoginFrameCtrl loadingFrameCtrl = new LoadToLoginFrameCtrl(loadingScene);
            //更改设置
            ChangeCtrl(loadingFrameCtrl);
            //开始去Login场景
            await loadingFrameCtrl.ToLoginScene();
        }

        //切换到主场景
        public async void ChangeToMainScene()
        {
            //设置目标帧率
            SetTargetFrame(60);
            //打开与关闭ui
            await OpenLoadingUIAndLoadLoadingScene();
            LoadingScene loadingScene = new LoadingScene();
            LoadToMainFrameCtrl loadingFrameCtrl = new LoadToMainFrameCtrl(loadingScene);
            //更改设置
            ChangeCtrl(loadingFrameCtrl);
            //开始去Login场景
            await loadingFrameCtrl.ToMainScene();
        }


        //切换控制器
        public void ChangeCtrl(FrameCtrl frameCtrl)
        {
            if (m_CurFrameCtrl != null)
            {
                m_CurFrameCtrl.Destroy();
                m_CurFrameCtrl = null;
            }
            ResMgr.Instance.UnLoadAssets();
            m_CurFrameCtrl = frameCtrl;
            m_CurFrameCtrl?.Init();
        }


        //打开LoadUi和LoadScene
        private async UniTask OpenLoadingUIAndLoadLoadingScene()
        {
            BaseUi loadingUi = UiMgr.Instance.GetOpenUi(UICfg.LoadingUi);
            if (loadingUi == null)
            {
                loadingUi = await UiMgr.Instance.OpenUiAsync(UICfg.LoadingUi);
            }
            UiMgr.Instance.CloseAllUiExceptLoadingUi(UICfg.LoadingUi);
            await ResMgr.Instance.LoadSceneAsync("LoadingScene");
        }


        //设置帧率
        private void SetTargetFrame(int targetFrame)
        {
            Application.targetFrameRate = targetFrame;
            FrameDtTime = 1.0f / targetFrame;
        }
        public float FrameDtTime
        {
            get; private set;
        }
    }
}

