namespace UIFrameWork
{
    /// <summary>
    /// UI界面信息类，定义UI的一些基础数据
    /// </summary>
    public class BaseScreenInfo
    {
        /// <summary>
        /// UI界面所属LayerId
        /// </summary>
        public int LayerId { get; protected set; }
        /// <summary>
        /// UI界面名字
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// UI界面资源路径
        /// </summary>
        public string ResPath { get; protected set; }
        /// <summary>
        /// 当其他窗口占据前景时，是否应该隐藏此窗口
        /// </summary>
        public bool IsHideOnForegroundLost { get; protected set; }
        /// <summary>
        /// 是否弹出窗口
        /// </summary>
        public bool IsPopup { get; protected set; }

        public BaseScreenInfo()
        {
            IsHideOnForegroundLost = true;
            IsPopup = false;
        }

        //设置UI界面所属LayerId
        public void SetLayerId(int layerId)
        {
            LayerId = layerId;
        }

        //设置UI界面的名字
        public void SetName(string name)
        {
            Name = name;
        }

        //设置UI界面资源路径
        public void SetResPath(string resPath)
        {
            ResPath = resPath;
        }

        //设置当其他窗口占据前景时，是否应该隐藏此窗口
        public void SetIsHideOnForegroundLost(bool isHideOnForegroundLost)
        {
            IsHideOnForegroundLost = isHideOnForegroundLost;
        }

        //设置是否弹出窗口
        public void SetIsPopup(bool isPopup)
        {
            IsPopup = isPopup;
        }

    }
}
