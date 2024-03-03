using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Game.UI
{
    public partial class BaseUi
    {
        private List<UIResBaseHandle> m_UiLoadResHandles;//ui加载的资源列表
        private void InitUiLoadRes()
        {
            m_UiLoadResHandles = new List<UIResBaseHandle>(20);
        }

        private void DestroyedUiLoadRes()
        {
            for (int i = 0; i < m_UiLoadResHandles.Count; i++)
            {
                m_UiLoadResHandles[i]?.Destroy();
            }
            m_UiLoadResHandles.Clear();
            m_UiLoadResHandles = null;
        }


        //设置ui上使用的图片
        public void SetImageSprite(Image image, string path, Action<Sprite> onComplete = null)
        {
            if (image == null || path.Equals(string.Empty))
            {
                return;
            }
            for (int i = 0; i < m_UiLoadResHandles.Count; i++)
            {
                UIResImageHandle curHandle = m_UiLoadResHandles[i] as UIResImageHandle;
                if (curHandle == null || curHandle.IsWaitComplete == false)
                {
                    continue;
                }
                if (image == curHandle.ImageTarget)
                {
                    if (path == curHandle.ResPath)
                    {
                        return;
                    }
                    curHandle.SetNotValid();
                }
            }
            UIResImageHandle newHandle = new UIResImageHandle();
            newHandle.Init(path);
            newHandle.ImageTarget = image;
            newHandle.OnComplete = onComplete;
            m_UiLoadResHandles.Add(newHandle);
        }
        //加载预制
        public void LoadPrefab(string prefabPath, Transform prefabParent = null, Action<GameObject> onComplete = null)
        {
            if (prefabParent == null) prefabParent = transform;

            for (int i = 0; i < m_UiLoadResHandles.Count; i++)
            {
                UIResPrefabHandle curHandle = m_UiLoadResHandles[i] as UIResPrefabHandle;
                if (curHandle == null || curHandle.IsWaitComplete == false)
                {
                    continue;
                }
                if (prefabParent.GetInstanceID() == curHandle.PrefabParent.GetInstanceID())
                {
                    if (prefabPath == curHandle.ResPath)
                    {
                        return;
                    }
                    curHandle.SetNotValid();
                }
            }
            UIResPrefabHandle newHandle = new UIResPrefabHandle();
            newHandle.Init(prefabPath);
            newHandle.PrefabParent = prefabParent;
            newHandle.OnComplete = onComplete;
            m_UiLoadResHandles.Add(newHandle);
        }
    }

}
