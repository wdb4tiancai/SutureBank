using Cysharp.Threading.Tasks;
using YooAsset;

namespace Game.UI
{
    public abstract class UIResBaseHandle
    {
        public string ResPath { get; private set; }//��Դ����
        protected bool IsLoadEnd { get; set; }//�Ƿ���ؽ���
        protected bool IsDestroy { get; set; }//�Ƿ�ɾ��
        protected bool IsValid { get; set; }//�Ƿ���Ч
        protected AssetHandle m_Resource;//��Դ���صĶ���

        //�Ƿ�ȴ����
        public bool IsWaitComplete => IsLoadEnd == false && IsValid == true && IsDestroy == false;

        /// <summary>
        /// ��Դ��ʼ��
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
        /// ��Դɾ��
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
        /// ���Ŀ��Ϊ��Ч
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
