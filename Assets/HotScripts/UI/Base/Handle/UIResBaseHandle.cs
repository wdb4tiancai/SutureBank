using Cysharp.Threading.Tasks;
using YooAsset;

namespace Game.UI
{
    public abstract class UIResBaseHandle
    {
        public string ResPath { get; private set; }//资源名字
        protected bool IsLoadEnd { get; set; }//是否加载结束
        protected bool IsDestroy { get; set; }//是否删除
        protected bool IsValid { get; set; }//是否有效
        protected AssetHandle m_Resource;//资源加载的对象

        //是否等待完成
        public bool IsWaitComplete => IsLoadEnd == false && IsValid == true && IsDestroy == false;

        /// <summary>
        /// 资源初始化
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="uiTarget"></param>
        public void Init(string resPath)
        {
            ResPath = resPath;
            IsLoadEnd = false;
            IsValid = true;
            IsDestroy = false;
            m_Resource = null;
            OnInitAsync();
        }


        /// <summary>
        /// 资源删除
        /// </summary>
        public void Destroy()
        {
            if (IsDestroy == true)
            {
                return;
            }
            IsDestroy = true;
            IsValid = false;
            OnDestroy();

            m_Resource?.Release();
            m_Resource = null;
        }

        /// <summary>
        /// 标记目标为无效
        /// </summary>
        public void SetNotValid()
        {
            if (IsWaitComplete == false)
            {
                return;
            }
            IsValid = false;
            OnSetNotValid();
        }

     
        protected abstract UniTask OnInitAsync();
        protected abstract void OnDestroy();
        protected abstract void OnSetNotValid();
    }
}
