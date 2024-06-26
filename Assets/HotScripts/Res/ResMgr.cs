﻿using Cysharp.Threading.Tasks;
using Game.Util;
using SharePublic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
namespace Game.Res
{
    public class ResMgr : SingletonMgrBase<ResMgr>
    {
        private bool m_IsInit = false;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="global"></param>
        public override void Init()
        {
            m_IsInit = true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void Destroy()
        {
            if (!m_IsInit)
            {
                return;
            }
            ResourcePackage package = YooAssets.GetPackage(AssetsVersion.AssetPackageName);
            package?.UnloadUnusedAssets();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public override void Reset()
        {
            if (!m_IsInit)
            {
                return;
            }
        }

        public override void Update(float dt)
        {
            if (!m_IsInit)
            {
                return;
            }
        }

        //卸载资源
        public void UnLoadAssets()
        {
            ResourcePackage package = YooAssets.GetPackage(AssetsVersion.AssetPackageName);
            package?.UnloadUnusedAssets();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            Debug.Log("是否所有资源");
        }
    }
}


