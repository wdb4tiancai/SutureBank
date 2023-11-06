using System;
namespace Game.Scene
{
    public abstract class FrameCtrl
    {
        protected BaseScene m_Scene;
        protected RenderMgr m_RenderMgr;
        public float m_WorldTime = 0;
        protected bool m_IsInit;

        public FrameCtrl(BaseScene scene, RenderMgr renderMgr = null)
        {
            m_IsInit = false;
            m_Scene = scene;
            m_RenderMgr = renderMgr;
        }

        public virtual void Init()
        {
            m_Scene.Init();
            if (m_RenderMgr != null)
            {
                m_RenderMgr.Init();
            }
        }

        public BaseScene GetScene()
        {
            return m_Scene;
        }
        public RenderMgr GetRenderMgr()
        {
            return m_RenderMgr;
        }

        public float GetWroldTime()
        {
            return m_WorldTime;
        }

        public virtual void Destroy()
        {
            m_Scene.Destroy();
            if (m_RenderMgr != null)
            {
                m_RenderMgr.Destroy();
            }
        }
        public virtual void Update(float dt)
        {
            if (!m_IsInit)
            {
                return;
            }
            UpdateScene(dt);
        }

        //场景初始化后日常更新
        private void UpdateScene(float dt)
        {
            if (m_WorldTime > 0)
            {
                m_WorldTime = m_WorldTime + dt;
                m_Scene.Update(dt);
            }
            else
            {
                m_WorldTime = m_WorldTime + dt;
            }
        }
    }
}
