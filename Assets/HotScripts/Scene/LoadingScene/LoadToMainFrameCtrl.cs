
using Cysharp.Threading.Tasks;
using Game.Res;
using Game.UI;
using UnityEngine;

namespace Game.Scene
{
    public class LoadToMainFrameCtrl : FrameCtrl
    {
        public LoadToMainFrameCtrl(BaseScene scene, RenderMgr renderMgr = null) : base(scene, renderMgr)
        {

        }

        public override void Init()
        {
            base.Init();
            m_IsInit = true;
            Debug.Log("LoadToMainFrameCtrl Init");
        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.Log("LoadToMainFrameCtrl Destroy");
        }


        public async UniTask ToMainScene()
        {
            MainScene mainScene = new MainScene();
            MainFrameCtrl mainFrameCtrl = new MainFrameCtrl(mainScene);
            SceneMgr.Instance.ChangeCtrl(mainFrameCtrl);
            await ResMgr.Instance.LoadSceneAsync("MainScene");
            await UiMgr.Instance.OpenUiAsync(UiCfg.MainUi);
            UiMgr.Instance.CloseUi(UiCfg.LoadingUi);
        }
    }
}
