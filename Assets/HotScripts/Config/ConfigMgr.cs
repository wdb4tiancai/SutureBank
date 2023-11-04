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
    public class ConfigMgr : SingletonBase<ConfigMgr>
    {
        private bool m_IsInit = false;
        private Engine m_Engine;
        private Game.Data.GameConfigs m_GameConfigs;
        public async UniTask Init(Engine engine)
        {
            m_Engine = engine;
            m_GameConfigs = new Game.Data.GameConfigs();
            await m_GameConfigs.LoadRes(LoadByteBuf);
        }
        public void Destroy()
        {
            if (!m_IsInit)
            {
                return;
            }
        }
        public void Update(float dt)
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

