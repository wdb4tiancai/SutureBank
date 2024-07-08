using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFrameWork
{
    /// <summary>
    ///这是所有UI的集中访问点。
    ///所有的调用都应该指向这个。
    /// </summary>
    public class UIFrame : MonoBehaviour
    {
        /// <summary>
        /// 所有的Layer层信息
        /// </summary>
        [SerializeField]
        private LayerProperties[] m_LayerProperties;

        /// <summary>
        /// 所有的Layer层信息Map
        /// </summary>
        private Dictionary<int, LayerProperties> m_LayerPropertiesMap;

        /// <summary>
        /// 不同的Layer层，不同的处理逻辑
        /// </summary>
        private Dictionary<LayerType, BaseLayer> m_LayerMap;

        /// <summary>
        /// Panel层对象
        /// </summary>
        private PanelLayer m_PanelLayer;

        /// <summary>
        /// Window层对象
        /// </summary>
        private WindowLayer m_WindowLayer;

        /// <summary>
        /// 主相机
        /// </summary>
        private Canvas m_MainCanvas;

        /// <summary>
        /// UI交互对象
        /// </summary>
        private GraphicRaycaster m_GraphicRaycaster;

        /// <summary>
        /// 遮罩计数
        /// </summary>
        private int m_ScreenBlockCount;

        /// <summary>
        /// UI的主画布
        /// </summary>
        public Canvas MainCanvas
        {
            get
            {
                if (m_MainCanvas == null)
                {
                    m_MainCanvas = GetComponent<Canvas>();
                }

                return m_MainCanvas;
            }
        }

        /// <summary>
        /// 主UI画布使用的相机
        /// </summary>
        public Camera UICamera
        {
            get { return MainCanvas.worldCamera; }
        }

        /// <summary>
        ///初始化UI框架。初始化包括初始化面板层和窗口层。

        ///虽然到目前为止我处理过的所有案例都采用了“窗口和面板”方法，

        ///我把它设置为虚拟的，以防你需要额外的层或其他特殊的初始化。
        /// </summary>
        public virtual void Initialize()
        {
            //Layer控制器
            m_LayerMap = new Dictionary<LayerType, BaseLayer>();

            //创建PanelLayer控制器
            m_PanelLayer = new PanelLayer();
            m_PanelLayer.Initialize();
            m_PanelLayer.AddScreenDestroyed(ScreenDestroyed);
            m_LayerMap.Add(LayerType.Panel, m_PanelLayer);

            //创建WindowLayer控制器
            m_WindowLayer = new WindowLayer();
            m_WindowLayer.Initialize();
            m_WindowLayer.AddRequestScreenBlock(OnRequestScreenBlock);
            m_WindowLayer.AddRequestScreenUnblock(OnRequestScreenUnblock);
            m_WindowLayer.AddScreenDestroyed(ScreenDestroyed);
            m_LayerMap.Add(LayerType.Window, m_WindowLayer);

            //注册Layer注册信息
            m_LayerPropertiesMap = new Dictionary<int, LayerProperties>(m_LayerProperties.Length);
            for (int i = 0; i < m_LayerProperties.Length; i++)
            {
                m_LayerPropertiesMap.Add(m_LayerProperties[i].LayerId, m_LayerProperties[i]);
            }
            //获得UI交互
            m_GraphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        }


        /// <summary>
        /// 打开ui界面
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="screenInfo"></param>
        /// <param name="screenData"></param>
        public void OpenUi(BaseScreen screen, BaseScreenInfo screenInfo, BaseScreenData screenData)
        {
            if (!m_LayerPropertiesMap.ContainsKey(screenInfo.LayerId))
            {
                Debug.LogError(string.Format("OpenUi 打开的 {0}，LayerId不对", screenInfo.Name));
                return;
            }

            //注册屏幕遮挡
            OnRequestScreenBlock();

            //屏幕初始化
            screen.InitScreen(screenInfo);

            //根据LayerId注册到对应的Transform
            LayerProperties layerProperties;
            m_LayerPropertiesMap.TryGetValue(screenInfo.LayerId, out layerProperties);
            screen.transform.SetParent(layerProperties.LayerTransform, false);

            //根据Layer类型获得不同的出里
            BaseLayer layer;
            m_LayerMap.TryGetValue(layerProperties.LayerType, out layer);
            layer.ShowScreen(screen, screenData, true);

            //注销屏幕遮挡
            OnRequestScreenUnblock();
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="screen"></param>
        public void CloseUi(BaseScreen screen)
        {
            if (m_PanelLayer.IsExistScreens(screen))
            {
                m_PanelLayer.CloseScreen(screen, true);
            }
            else if (m_WindowLayer.IsExistScreens(screen))
            {
                m_WindowLayer.CloseScreen(screen, true);
            }
        }


        /// <summary>
        /// 检查给定面板是否打开。
        /// </summary>
        /// <param name="panel name">Panel 名字.</param>
        public BaseScreen FindScreenByScreenInfo(BaseScreenInfo screenInfo)
        {
            LayerProperties layerProperties;
            m_LayerPropertiesMap.TryGetValue(screenInfo.LayerId, out layerProperties);
            if (layerProperties == null)
            {
                return null;
            }
            BaseLayer layer;
            m_LayerMap.TryGetValue(layerProperties.LayerType, out layer);
            return layer.FindScreenByName(screenInfo.Name);
        }

        /// <summary>
        /// 检查给定面板是否打开。
        /// </summary>
        /// <param name="panel name">Panel 名字.</param>
        public bool IsPanelOpen(string panelName)
        {
            return m_PanelLayer.IsPanelVisible(panelName);
        }

        /// <summary>
        /// 关闭所有屏幕
        /// </summary>
        public void CloseAll()
        {
            CloseAllWindows();
            CloseAllPanels();
        }

        /// <summary>
        /// 关闭面板层上的所有屏幕,除了指定屏幕
        /// </summary>
        /// 
        public void CloseAllPanelsExceptLoadingUi(string exceptUiName)
        {
            m_PanelLayer.CloseAllPanelsExceptLoadingUi(exceptUiName);
        }

        /// <summary>
        /// 关闭面板层上的所有屏幕
        /// </summary>
        public void CloseAllPanels()
        {
            m_PanelLayer.CloseAll();
        }

        /// <summary>
        /// 关闭窗口层中的所有屏幕
        /// </summary>
        public void CloseAllWindows()
        {
            m_WindowLayer.CloseAll();
        }

        /// <summary>
        /// 界面被卸载
        /// </summary>
        /// <param name="screen"></param>
        private void ScreenDestroyed(BaseScreen screen)
        {
            if (screen == null || screen.ScreenInfo == null)
            {
                return;
            }
        }

        /// <summary>
        /// 增加遮挡
        /// </summary>
        private void OnRequestScreenBlock()
        {
            m_ScreenBlockCount++;
            if (m_ScreenBlockCount > 0)
            {
                if (m_GraphicRaycaster != null)
                {
                    m_GraphicRaycaster.enabled = false;
                }
            }
        }

        /// <summary>
        /// 减少遮挡
        /// </summary>
        private void OnRequestScreenUnblock()
        {
            m_ScreenBlockCount--;
            if (m_ScreenBlockCount <= 0)
            {
                if (m_GraphicRaycaster != null)
                {
                    m_GraphicRaycaster.enabled = true;
                }
            }
        }
    }
}
