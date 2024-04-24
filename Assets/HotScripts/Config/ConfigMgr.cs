using Game.Util;
using YooAsset;
using UnityEngine;
using System.Collections.Generic;
using Luban;
using Game.Res;
using Game.Data;

namespace Game.Main
{
    public class ConfigMgr : SingletonMgrBase<ConfigMgr>
    {
        public bool IsInit { get; private set; } = false;

        public Game.Data.GameConfigs GameConfigs { get; private set; }

        private List<ConfigBase> m_WaitLoadCfgs = new List<ConfigBase>();//等待下载的配置表
        private List<ConfigBase> m_FailLoadCfgs = new List<ConfigBase>(100);//下载失败的配置表

        public override void Init()
        {
            if (IsInit)
            {
                return;
            }
            IsInit = true;
            GameConfigs = new Game.Data.GameConfigs();
            m_WaitLoadCfgs = GameConfigs.LoadRes();
            StartLoadCfgs();
        }

        public override void Destroy()
        {
            if (!IsInit)
            {
                return;
            }
        }

        public override void Reset()
        {
            if (!IsInit)
            {
                return;
            }
        }

        public override void Update(float dt)
        {
            if (!IsInit)
            {
                return;
            }
        }


        //开始下载配置表
        private void StartLoadCfgs()
        {
            List<ConfigBase> tempLoadCfgs = new List<ConfigBase>(m_WaitLoadCfgs.Count);
            tempLoadCfgs.AddRange(m_WaitLoadCfgs);
            for (int i = 0; i < tempLoadCfgs.Count; i++)
            {
                LoadCfg(tempLoadCfgs[i]);
            }
        }


        //加载配置表
        private void LoadCfg(ConfigBase config)
        {
            YooAssets.LoadAssetAsync(config.ConfigName).Completed += (handle) =>
            {
                ChangeLoadCfg(config, handle);
            };
        }

        //更改下载记录
        private void ChangeLoadCfg(ConfigBase config, AssetHandle handle)
        {
            m_WaitLoadCfgs.Remove(config);
            if (handle.IsDone && handle.AssetObject != null)
            {
                config.LoadByteBuf(new ByteBuf(((TextAsset)handle.AssetObject).bytes));
            }
            else
            {
                m_FailLoadCfgs.Add(config);
            }
            if (m_WaitLoadCfgs.Count <= 0)
            {
                EngineFsmDefine.ConfigInitializeSucceed.SendEventMessage();
            }
        }
    }
}

