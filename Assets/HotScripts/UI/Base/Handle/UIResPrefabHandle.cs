using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using YooAsset;

namespace Game.UI
{
    public class UIResPrefabHandle : UIResBaseHandle
    {
        public Transform PrefabParent { get; set; }//预制加载完成后的父级
        public Action<GameObject> OnComplete { get; set; }//预制加载完成后事件

        protected override async UniTask OnInitAsync()
        {
            m_Resource = YooAssets.LoadAssetAsync<GameObject>(ResPath);
            await m_Resource.Task;
            if (PrefabParent == null)
            {
                return;
            }
            GameObject prefab = m_Resource.InstantiateSync();
            prefab.transform.SetParent(PrefabParent, false);
            prefab.transform.localPosition = Vector3.zero;
            OnComplete?.Invoke(prefab);
        }
        protected override void OnDestroy()
        {
            PrefabParent = null;
            OnComplete = null;
        }


        protected override void OnSetNotValid()
        {
            PrefabParent = null;
            OnComplete = null;
        }

    }
}
