using Game.Util;
using Cysharp.Threading.Tasks;
using UIFramework;
using YooAsset;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Game.UI
{
    public class UICfg
    {
        public const string MainUi = "MainUi";
        public const string LoadingUi = "LoadingUi";
        public const string LoginUi = "LoginUi";


        public static Dictionary<string, ScreenInfo> UIInfo = new Dictionary<string, ScreenInfo>()
        {
            {MainUi,new ScreenInfo(){LayerId = 1,Name = "MainUi",ResPath = "MainUi" } },
            {LoadingUi,new ScreenInfo(){LayerId = 5,Name = "LoadingUi",ResPath = "LoadingUi" } },
            {LoginUi,new ScreenInfo(){LayerId = 1,Name = "LoginUi",ResPath = "LoginUi" } },
        };
    }

    public class UiMgr : SingletonBase<UiMgr>
    {
        private GameObject m_UIFrameObj;
        private UIFrame m_UIFrame;
        private AssetHandle m_UIFrameHandle;
        private Engine m_Engine;
        private bool m_IsInit = false;
        public async UniTask Init(Engine engine)
        {
            m_Engine = engine;
            if (m_UIFrame == null)
            {
                AssetHandle m_UIFrameHandle = YooAssets.LoadAssetAsync<GameObject>("UIFrame");
                await m_UIFrameHandle.Task;
                m_UIFrameObj = m_UIFrameHandle.InstantiateSync();
                m_UIFrameObj.transform.SetParent(engine.transform);
                m_UIFrameObj.transform.SetAsLastSibling();
                m_UIFrameObj.transform.localPosition = Vector3.zero;
                m_UIFrameObj.transform.localScale = Vector3.one;
                m_UIFrameObj.name = "UIFrame";
                m_UIFrame = m_UIFrameObj.GetComponent<UIFrame>();
                m_UIFrame.Initialize();
            }
            m_IsInit = true;
        }
        public void Destroy()
        {
            if (!m_IsInit)
            {
                return;
            }
            m_UIFrameHandle?.Release();
            m_UIFrameHandle = null;
        }
        public void Update(float dt)
        {
            if (!m_IsInit)
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
            if (!UICfg.UIInfo.TryGetValue(uiName, out uiInfo))
            {
                Debug.LogError($" {uiName} 不存在 ");
                return null;
            }
            AssetHandle assetHandle = YooAssets.LoadAssetAsync<GameObject>(uiInfo.ResPath);
            await assetHandle.Task;
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
            if (!UICfg.UIInfo.TryGetValue(uiName, out uiInfo))
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
            if (!UICfg.UIInfo.TryGetValue(uiName, out uiInfo))
            {
                Debug.LogError($" {uiName} 不存在 ");
                return null;
            }
            return m_UIFrame.FindScreenByScreenInfo(uiInfo) as BaseUi;
        }
    }
}

