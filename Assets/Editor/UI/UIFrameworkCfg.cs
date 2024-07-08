using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UIFrameWork.Editor
{

    [CreateAssetMenu(fileName = "UIFrameworkCfg", menuName = "自定义配置/UI配置器")]
    public class UIFrameworkCfg : ScriptableObject
    {
        /// <summary>
        /// 代码保存目录
        /// </summary>
        public List<string> FolderList;

        /// <summary>
        /// 当前所有的NameSpace
        /// </summary>
        public List<string> NameSpaceList;

        /// <summary>
        /// 所有的基类
        /// </summary>
        public List<string> BaseClassList;

        public UIFrameworkCfg()
        {
            FolderList = new List<string>();
            NameSpaceList = new List<string>();
            BaseClassList = new List<string>();
        }
    }
}
