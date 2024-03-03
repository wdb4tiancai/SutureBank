using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Game.UI
{
    public class UIResImageHandle : UIResBaseHandle
    {
        //资源加载回调
        //无论成功失败都会调用
        public Action<Sprite> OnComplete;
        public Image ImageTarget { get; set; }//资源加载完需要赋值的ui目标
        protected override async UniTask OnInitAsync()
        {
            m_Resource = YooAssets.LoadAssetAsync<Sprite>(ResPath);
            await m_Resource.Task;
            Sprite sprite = m_Resource.AssetObject as Sprite;
            if (ImageTarget != null && sprite != null)
            {
                ImageTarget.sprite = sprite;
            }           
            OnComplete?.Invoke(sprite);

        }
        protected override void OnDestroy()
        {
            ImageTarget = null;
        }


        protected override void OnSetNotValid()
        {
            ImageTarget = null;
        }
    }
}
