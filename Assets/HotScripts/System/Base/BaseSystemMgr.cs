namespace Game.Mgr
{
    public abstract class DYBaseDataMgr<T> : IBaseSystemMgr where T : BaseSystemData, new()
    {
        protected T m_GameData;

        //初始化
        public void Init()
        {
            m_GameData = new T();
            InitImp();
        }
        /// <summary>
        // 初始化
        /// </summary>
        protected abstract void InitImp();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            ResetImp();
        }

        protected abstract void ResetImp();

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            UpdateImp();
        }
        protected abstract void UpdateImp();

        //获取数据
        public T GetLocalData()
        {
            return m_GameData;
        }
    }
}
