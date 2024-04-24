using Game.Util;
using Cysharp.Threading.Tasks;
using UIFramework;
using YooAsset;
using UnityEngine;
using Game.Data;
using Game.Res;
using Game.Main;

namespace Game.UI
{
    public class UiMgr : SingletonMgrBase<UiMgr>
    {
        public bool IsInit { get; private set; } = false;
        private GameObject m_UIFrameObj;
        private UIFrame m_UIFrame;
        private AssetHandle m_UIFrameHandle;
        public override void Init()
        {
            if (IsInit)
            {
                return;
            }
            IsInit = true;
            System.Collections.Generic.List<UiCfgItem> uiCfgItems = ConfigMgr.Instance.GameConfigs.UiCfg.DataList;
            for (int i = 0; i < uiCfgItems.Count; i++)
            {
                UiCfgItem uiCfgItem = uiCfgItems[i];
                UiCfg.AddUIInfo(uiCfgItem.LayerId, uiCfgItem.Id, uiCfgItem.ResPath);
            }

            //初始化资源对象
            YooAssets.LoadAssetAsync<GameObject>("Base_UIFrame").Completed += (handle) =>
            {
                m_UIFrameObj = handle.InstantiateSync();
                m_UIFrameObj.transform.SetParent(Engine.Instance.transform);
                m_UIFrameObj.transform.SetAsLastSibling();
                m_UIFrameObj.transform.localPosition = Vector3.zero;
                m_UIFrameObj.transform.localScale = Vector3.one;
                m_UIFrameObj.name = "UIFrame";
                m_UIFrame = m_UIFrameObj.GetComponent<UIFrame>();
                m_UIFrame.Initialize();
                EngineFsmDefine.UIInitializeSucceed.SendEventMessage();
            };

        }
        public override void Destroy()
        {
            if (!IsInit)
            {
                return;
            }
            m_UIFrameHandle?.Release();
            m_UIFrameHandle = null;
        }
        public override void Reset()
        {
            if (!IsInit)
            {
                return;
            }
        }
        public override void Update(float dt)
        {
            if (!IsInit)
            {
                return;
            }
        }

        //打开ui界面，异步的
        public async UniTask<BaseUi> OpenUiAsync(string uiName)
        {
            return await OpenUiAsync(uiName, null);
        }

        //打开ui界面，异步的
        public async UniTask<BaseUi> OpenUiAsync(string uiName, BaseScreenData screenData)
        {
            ScreenInfo uiInfo;
            if (!UiCfg.UIInfo.TryGetValue(uiName, out uiInfo))
            {
                Debug.LogError($" {uiName} 不存在 ");
                return null;
            }
            AssetHandle assetHandle = YooAssets.LoadAssetAsync<GameObject>(uiInfo.ResPath);
            await assetHandle.ToUniTask();
            GameObject uiObj = assetHandle.InstantiateSync();
            uiObj.name = uiName;
            BaseUi baseUi = uiObj.GetComponent<BaseUi>();
            if (baseUi == null)
            {
                Debug.LogError($" {uiName} BaseScreen 不存在 ");
                return null;
            }
            baseUi.AssetHandle = assetHandle;
            m_UIFrame.OpenUi(baseUi, uiInfo, screenData);
            return baseUi;
        }

        /// <summary>
        /// 关闭ui
        /// </summary>
        /// <param name="uiName"></param>
        public void CloseUi(string uiName)
        {
            ScreenInfo uiInfo;
            if (!UiCfg.UIInfo.TryGetValue(uiName, out uiInfo))
            {
                Debug.LogError($" {uiName} 不存在 ");
                return;
            }
            CloseUi(m_UIFrame.FindScreenByScreenInfo(uiInfo));
        }

        /// <summary>
        /// 关闭ui界面
        /// </summary>
        /// <param name="screen"></param>
        public void CloseUi(BaseScreen screen)
        {
            if (screen == null)
            {
                return;
            }
            m_UIFrame.CloseUi(screen);
        }

        /// <summary>
        /// 关闭所有UI,除了指定UI
        /// </summary>
        /// 
        public void CloseAllUiExceptLoadingUi(string exceptUiName)
        {
            m_UIFrame.CloseAllPanelsExceptLoadingUi(exceptUiName);
            m_UIFrame.CloseAllWindows();
        }

        //获得打开的ui
        public BaseUi GetOpenUi(string uiName)
        {
            ScreenInfo uiInfo;
            if (!UiCfg.UIInfo.TryGetValue(uiName, out uiInfo))
            {
                Debug.LogError($" {uiName} 不存在 ");
                return null;
            }
            return m_UIFrame.FindScreenByScreenInfo(uiInfo) as BaseUi;
        }
    }
}

