﻿using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
using YooAsset;
namespace Game.UI
{
    public partial class MainUi
    {
        /// <summary>
        /// 初始化逻辑
        /// </summary>
        /// <param name="screenInfo"></param>
        protected override void OnInitScreen()
        {
            base.OnInitScreen();
            m_Button1.SetOnClick(OnButton1);
            m_Button2.SetOnClick(OnButton2);
        }


        private void OnButton1()
        {

        }
        private void OnButton2()
        {
        }
    }
}
