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
        public static string MainUi = "MainUi";
        public static string DialogueUi = "DialogueUi";


        public static Dictionary<string, ScreenInfo> UIInfo = new Dictionary<string, ScreenInfo>()
        {
            {MainUi,new ScreenInfo(){LayerId = 1,Name = "MainUi",ResPath = "MainUi" } },
        };
    }

    public class UiMgr : SingletonBase<UiMgr>
    {
        private GameObject m_UIFrameObj;
        private UIFrame m_UIFrame;
        private AssetHandle m_UIFrameHandle;
        private Engine m_Engin;
        private bool m_IsInit = false;
        public async UniTask Init(Engine engine)
        {
            m_Engin = engine;
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
            m_IsInit = true;
            await OpenUiAsync("MainUi");
        }
        public void Destroy()
        {
            if (!m_IsInit)
            {
                return;
            }
        }
        public void Update(float dt)
        {
            if (!m_IsInit)
            {
                return;
            }
        }

        //打开ui界面，异步的
        public async UniTask OpenUiAsync(string uiName)
        {
            await OpenUiAsync(uiName, null);
        }

        //打开ui界面，异步的
        public async UniTask OpenUiAsync(string uiName, BaseScreenData screenData)
        {
            ScreenInfo uiInfo;
            if (!UICfg.UIInfo.TryGetValue(uiName, out uiInfo))
            {
                Debug.LogError("error ====== ui not exit ==== " + uiName);
                return;
            }
            AssetHandle m_UIObj = YooAssets.LoadAssetAsync<GameObject>(uiInfo.ResPath);
            await m_UIObj.Task;
            GameObject uiObj = m_UIObj.InstantiateSync();
            uiObj.name = uiName;
            BaseScreen baseScreen = uiObj.GetComponent<BaseScreen>();
            m_UIFrame.OpenUi(baseScreen, uiInfo, screenData);
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
                Debug.LogError("error ====== ui not exit ==== " + uiName);
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

    }
}

