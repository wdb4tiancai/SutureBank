﻿using Game.Scene;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
using YooAsset;
namespace Game.UI
{
    public partial class LoginUi
    {
        /// <summary>
        /// 初始化逻辑
        /// </summary>
        /// <param name="screenInfo"></param>
        protected override void OnInitScreen()
        {
            base.OnInitScreen();
            m_Button.SetOnClick(OnButton);
        }


        private void OnButton()
        {
            SceneMgr.Instance.ChangeToMainScene();
        }
    }
}
