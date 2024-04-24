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
            AssetHandle assetHandle = YooAssets.LoadAssetAsync<Sprite>(resPath);
            m_UiResAssetHandle.Add(assetHandle);
            await assetHandle.ToUniTask();
            if (assetHandle.AssetObject == null)
            {
                return;
            }
            Sprite sprite = assetHandle.AssetObject as Sprite;
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

            AssetHandle assetHandle = YooAssets.LoadAssetAsync<GameObject>(prefabPath);
            m_UiResAssetHandle.Add(assetHandle);
            await assetHandle.ToUniTask();
            if (assetHandle.AssetObject == null)
            {
                return;
            }
            GameObject prefab = assetHandle.InstantiateSync();
            prefab.transform.SetParent(prefabParent, false);
            prefab.transform.localPosition = Vector3.zero;
        }
    }

}
