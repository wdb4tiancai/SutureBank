using UIFramework;
using YooAsset;

namespace Game.UI
{
    public partial class BaseUi : BaseScreen
    {

        public bool IsDestroyed = false;//ui界面是否删除
        public AssetHandle AssetHandle { private get; set; }//ui界面自己的资源对象

        protected override void OnInitScreen()
        {
            IsDestroyed = false;
            InitUiLoadRes();
        }

        protected override void OnDestroyedScreen()
        {
            if (IsDestroyed)
            {
                return;
            }
            IsDestroyed = true;
            DestroyedUiLoadRes();
            AssetHandle?.Release();
            AssetHandle = null;
        }

        /// <summary>
        /// 显示逻辑
        /// </summary>
        /// <param name="screenData"></param>
        protected override void OnShowScreen(BaseScreenData screenData)
        {
            OnTransitionInFinished();
        }

        /// <summary>
        /// 关闭逻辑
        /// </summary>
        protected override void OnCloseScreen()
        {
            OnTransitionOutFinished();
        }

        protected override void OnHideScreen()
        {
        }
    }

}
