using Cysharp.Threading.Tasks;
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
        public override async UniTask Init()
        {
            m_IsInit = true;
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override async UniTask Destroy()
        {
            if (!m_IsInit)
            {
                return;
            }
            ResourcePackage package = YooAssets.GetPackage(AssetsVersion.AssetPackageName);
            package?.UnloadUnusedAssets();
            await Resources.UnloadUnusedAssets();
            System.GC.Collect();
            await UniTask.CompletedTask;
        }

        public override async UniTask Reset()
        {
            if (!m_IsInit)
            {
                return;
            }
            await UniTask.CompletedTask;
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
            Debug.Log("UnLoadAssets");
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

        /// <summary>
        /// 资源加载
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadCallBack"></param>
        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single)
        {
            SceneHandle handle = YooAssets.LoadSceneAsync(sceneName, sceneMode);
            await handle.Task;
        }
    }
}


