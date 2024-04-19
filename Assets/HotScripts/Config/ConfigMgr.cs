using Game.Util;
using Cysharp.Threading.Tasks;
using UIFramework;
using YooAsset;
using UnityEngine;
using System;
using System.Collections.Generic;
using Luban;

namespace Game.Config
{
    public class ConfigMgr : SingletonMgrBase<ConfigMgr>
    {
        private bool m_IsInit = false;
        public Game.Data.GameConfigs GameConfigs { get; private set; }

        public override async UniTask Init()
        {
            m_IsInit = false;
            GameConfigs = new Game.Data.GameConfigs();
            await GameConfigs.LoadRes(LoadByteBuf);
            m_IsInit = true;
        }

        public override async UniTask Destroy()
        {
            if (!m_IsInit)
            {
                return;
            }
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

        private static async UniTask<ByteBuf> LoadByteBuf(string file)
        {
            AssetHandle byteBufData = YooAssets.LoadAssetAsync<TextAsset>(file);
            await byteBufData.Task;
            return new ByteBuf(((TextAsset)byteBufData.AssetObject).bytes);
        }

    }
}

