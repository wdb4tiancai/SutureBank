using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UIFramework
{
    /// <summary>
    /// 场景信息类
    /// </summary>
    public class ScreenInfo
    {
        /// <summary>
        /// 所属LayerId
        /// </summary>
        public int LayerId;
        /// <summary>
        /// 名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 资源路径
        /// </summary>
        public string ResPath;
        /// <summary>
        /// 当其他窗口占据前景时，是否应该隐藏此窗口？
        /// </summary>
        public bool HideOnForegroundLost;
        /// <summary>
        /// 是否弹出窗口
        /// </summary>
        public bool IsPopup;

        public ScreenInfo()
        {
            HideOnForegroundLost = true;
            IsPopup = false;
        }
    }
}
