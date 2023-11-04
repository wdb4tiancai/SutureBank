using Game.Util;
using SharePublic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
namespace Game.Res
{
    public class ResMgr : SingletonBase<ResMgr>
    {
        private Engine m_Engine;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="global"></param>
        public void Init(Engine engine)
        {
            m_Engine = engine;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Destroy()
        {
            ResourcePackage package = YooAssets.GetPackage(AssetsVersion.AssetPackageName);
            package?.UnloadUnusedAssets();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }


        /// <summary>
        /// 资源加载
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="loadCallBack"></param>
        public void LoadRes(string resName, Action<AssetHandle> loadCallBack)
        {
            AssetHandle handle = YooAssets.LoadAssetAsync(resName);
            handle.Completed += loadCallBack;
        }
    }
}


