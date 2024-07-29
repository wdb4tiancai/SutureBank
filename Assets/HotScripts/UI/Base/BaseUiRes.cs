using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Game.UI
{
    public partial class BaseUi
    {
        private List<AssetHandle> m_UiResAssetHandle;//ui加载的资源列表
        private void InitUiLoadRes()
        {
            m_UiResAssetHandle = new List<AssetHandle>(20);
        }

        private void DestroyedUiLoadRes()
        {
            for (int i = 0; i < m_UiResAssetHandle.Count; i++)
            {
                m_UiResAssetHandle[i]?.Release();
            }
            m_UiResAssetHandle.Clear();
        }


        //设置ui上使用的图片
        public async UniTask SetImageSprite(Image image, string resPath)
        {
            if (image == null || resPath.Equals(string.Empty))
            {
                return;
            }
            AssetHandle resource = YooAssets.LoadAssetAsync<Sprite>(resPath);
            if (resource == null)
            {
                return;
            }
            m_UiResAssetHandle.Add(resource);
            await resource.Task;
            if (resource.Status != EOperationStatus.Succeed)
            {
                return;
            }
            Sprite sprite = resource.AssetObject as Sprite;
            if (image != null && sprite != null)
            {
                image.sprite = sprite;
            }
        }
        //加载预制
        public async UniTask LoadPrefab(string prefabPath, Transform prefabParent = null)
        {
            if (prefabPath.Equals(string.Empty))
            {
                return;
            }
            if (prefabParent == null) prefabParent = transform;

            AssetHandle resource = YooAssets.LoadAssetAsync<GameObject>(prefabPath);
            if (resource == null)
            {
                return;
            }
            m_UiResAssetHandle.Add(resource);
            await resource.Task;
            if (resource.Status != EOperationStatus.Succeed)
            {
                return;
            }
            GameObject prefab = resource.InstantiateSync();
            prefab.transform.SetParent(prefabParent, false);
            prefab.transform.localPosition = Vector3.zero;
        }
    }

}
