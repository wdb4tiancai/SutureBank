using UIFramework;
using YooAsset;

namespace Game.UI
{
    public class BaseUi : BaseScreen
    {

        public AssetHandle AssetHandle { private get; set; }

        protected override void OnInitScreen()
        {
        }

        protected override void OnDestroyedScreen()
        {
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
