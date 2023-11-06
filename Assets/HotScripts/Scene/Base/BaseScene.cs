using UnityEngine;

namespace Game.Scene
{
    public abstract class BaseScene
    {
        protected float m_TimeScale = 1;

        public float GetDtTime()
        {
            return Time.deltaTime * m_TimeScale;
        }

        public virtual void Init()
        {
            m_TimeScale = 1;
        }

        public virtual void Update(float dt)
        {
        }
        public virtual void Destroy()
        {
            m_TimeScale = 1;
        }
    }
}
