using Cysharp.Threading.Tasks;
using Game.Res;
using Game.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace Game.Audio
{
    public class AudioMgr : SingletonMgrBase<AudioMgr>
    {
        private bool m_IsInit = false;
        //音效管理器的obj对象
        private GameObject m_AudioObject;

        //声音记录
        private readonly List<AudioProxy> m_Proxies = new List<AudioProxy>(50);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="global"></param>
        public override async UniTask Init()
        {
            m_AudioObject = Engine.Instance.transform.Find("AudioMgr")?.gameObject;
            if (m_AudioObject == null)
            {
                m_AudioObject = new GameObject("AudioMgr");
                m_AudioObject.transform.SetParent(Engine.Instance.transform, false);
            }
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

        /// <summary>
        /// 停止所有的声音
        /// </summary>
        public void StopAllSound()
        {
            foreach (AudioType audioType in System.Enum.GetValues(typeof(AudioType)))
            {
                StopProxyByType(audioType);
            }
        }


        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="audioName"></param>
        public void PlayMusic(string audioName)
        {
            StopProxyByType(AudioType.Music);
            AudioProxy proxy = null;
            proxy = SpawnAudioProxy(AudioType.Music);
            proxy.BackClipRes();
            proxy.Play(PlayMode.Repeat, audioName);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="pos"></param>
        /// <param name="loop"></param>
        public void PlaySound(string audioName, Vector3 pos, bool loop = false)
        {
            var proxy = SpawnAudioProxy(AudioType.Sound);
            proxy.BackClipRes();
            proxy.Play(loop ? PlayMode.Repeat : PlayMode.Single, audioName, pos);
        }

        /// <summary>
        /// 在缓存里找到一个新的AudioProxy
        /// </summary>
        /// <param name="audioType">AudioType</param>
        /// <returns></returns>
        private AudioProxy SpawnAudioProxy(AudioType audioType)
        {
            AudioProxy proxy = FindProxyByIdleType(audioType);
            if (proxy != null)
            {
                return proxy;
            }
            proxy = CreateAudioProxy(audioType);
            m_Proxies.Add(proxy);
            return proxy;
        }

        /// <summary>
        /// 创建一个新的AudioProxy
        /// </summary>
        /// <param name="audioType">AudioType</param>
        /// <returns></returns>
        private AudioProxy CreateAudioProxy(AudioType audioType)
        {
            var go = new GameObject("o");
            go.transform.SetParent(m_AudioObject.transform, false);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            switch (audioType)
            {
                case AudioType.Music:
                case AudioType.UI:
                    source.spatialBlend = 0f;
                    break;
            }

            return new AudioProxy(audioType, go.transform, source, LoadResFunc, UnLoadResAction);
        }

        /// <summary>
        /// 获得指定类型空闲的AudioProxy
        /// </summary>
        /// <param name="audioType"></param>
        /// <returns></returns>
        private AudioProxy FindProxyByIdleType(AudioType audioType)
        {
            AudioProxy proxy = null;
            for (var i = 0; i < m_Proxies.Count; ++i)
            {
                proxy = m_Proxies[i];

                if (proxy == null)
                {
                    m_Proxies.RemoveAt(i);
                    --i;
                    continue;
                }
                if (audioType != proxy.AudioType || !proxy.IsIdle)
                    continue;
                return proxy;
            }
            return null;
        }

        /// <summary>
        /// 停止指定类型的声音播放
        /// </summary>
        /// <param name="audioType"></param>
        private void StopProxyByType(AudioType audioType)
        {
            AudioProxy proxy = null;
            for (var i = 0; i < m_Proxies.Count; ++i)
            {
                proxy = m_Proxies[i];
                if (proxy == null)
                {
                    m_Proxies.RemoveAt(i);
                    --i;
                    continue;
                }
                if (audioType != proxy.AudioType)
                    continue;
                if (proxy.IsLoad)
                {
                    proxy.Cancel();
                    continue;
                }
                else if (proxy.IsIdle)
                {
                    continue;
                }
                else if (proxy.IsPlay)
                {
                    proxy.BackClipRes();
                }
            }
        }

        //资源加载
        private void LoadResFunc(string audioName, Action<AssetHandle> loadCallBack)
        {
            ResMgr.Instance.LoadRes(audioName, loadCallBack);
        }

        //资源释放
        private void UnLoadResAction(AssetHandle assetHandle)
        {
            assetHandle?.Release();
        }
    }
}

