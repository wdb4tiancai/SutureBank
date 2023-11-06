
using Cysharp.Threading.Tasks;
using Game.Res;
using UnityEngine;

namespace Game.Scene
{
    public class LoginFrameCtrl : FrameCtrl
    {
        public LoginFrameCtrl(BaseScene scene, RenderMgr renderMgr = null) : base(scene, renderMgr)
        {

        }

        public override void Init()
        {
            base.Init();
            m_IsInit = true;
            Debug.LogError("LoginFrameCtrl Init");
        }

        public override void Destroy()
        {
            base.Destroy();
            Debug.LogError("LoginFrameCtrl Init");
        }
    }
}
