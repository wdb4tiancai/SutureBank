
using Cysharp.Threading.Tasks;
using Game.Res;
using Game.UI;
using UnityEngine;
using YooAsset;

namespace Game.Scene
{
    public class LoadToLoginFrameCtrl : FrameCtrl
    {
        public LoadToLoginFrameCtrl(BaseScene scene, RenderMgr renderMgr = null) : base(scene, renderMgr)
        {

        }

        public override void Init()
        {
            base.Init();
            m_IsInit = true;
            Debug.Log("LoadToLoginFrameCtrl Init");
        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.Log("LoadToLoginFrameCtrl Destroy");
        }


        public async UniTask ToLoginScene()
        {
            LoginScene loadingScene = new LoginScene();
            LoginFrameCtrl loginFrameCtrl = new LoginFrameCtrl(loadingScene);
            SceneMgr.Instance.ChangeCtrl(loginFrameCtrl);
            SceneHandle handle = YooAssets.LoadSceneAsync("LoginScene");
            await handle.ToUniTask();
            await UiMgr.Instance.OpenUiAsync(UiCfg.LoginUi);
            UiMgr.Instance.CloseUi(UiCfg.LoadingUi);
        }
    }
}
