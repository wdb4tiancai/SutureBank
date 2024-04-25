
using Cysharp.Threading.Tasks;
using Game.Res;
using UnityEngine;

namespace Game.Scene
{
    public class MainFrameCtrl : FrameCtrl
    {
        public MainFrameCtrl(BaseScene scene, RenderMgr renderMgr = null) : base(scene, renderMgr)
        {

        }

        public override void Init()
        {
            base.Init();
            m_IsInit = true;
            Debug.Log("MainFrameCtrl Init");
        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.Log("MainFrameCtrl Destroy");
        }
    }
}
