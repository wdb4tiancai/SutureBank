using Game.Scene;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using YooAsset;
namespace Game.UI
{
    public partial class SelectServerUI
    {
        /// <summary>
        /// 初始化逻辑
        /// </summary>
        /// <param name="screenInfo"></param>
        protected override void OnInitScreen()
        {
            base.OnInitScreen();
            m_EnterGame.SetOnClick(OnButton);
        }


        private void OnButton()
        {
            SceneMgr.Instance.ChangeToMainScene();
        }
    }
}
