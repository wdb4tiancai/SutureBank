using System;
using UnityEngine;
using YooAsset;

namespace Game.Audio
{
    public class AudioProxy
    {
        //音效的名字
        public string AudioName { get; private set; }

        //音乐的类型
        public AudioType AudioType { get; private set; }

        //音乐的播放模式
        private PlayMode Mode;


        private AudioSource m_Source { get; }

        private Transform m_Agent { get; }

        //音效的状态
        private AudioState m_AudioState { get; set; }

        //音效资源的状态
        public AudioResState AudioResState { get; private set; }

        //音效资源
        private AudioClip m_Clip;

        //音效资源句柄
        private AssetHandle m_AssetHandle;

        //当前音效是否在加载
        public bool IsLoad => AudioResState == AudioResState.Loading;

        //当前音效是否空闲
        public bool IsIdle => AudioResState == AudioResState.Done && (!m_Source.isPlaying || m_AudioState == AudioState.Cancel);

        //当前音效是否在播放
        public bool IsPlay => AudioResState == AudioResState.Done && m_Source.isPlaying;

        //资源加载
        private Action<string, Action<AssetHandle>> m_LoadResAction;
        //资源卸载
        private Action<AssetHandle> m_UnLoadResAction;


        //初始化
        public AudioProxy(AudioType audioType, Transform agent, AudioSource source, Action<string, Action<AssetHandle>> loadResAction, Action<AssetHandle> unLoadResAction)
        {
            m_Clip = null;
            m_AssetHandle = null;
            m_LoadResAction = loadResAction;
            m_UnLoadResAction = unLoadResAction;
            AudioType = audioType;
            m_Agent = agent;
            m_Source = source;
            m_AudioState = AudioState.None;
        }

        //播放音乐
        public void Play(PlayMode playMode, string audioName)
        {
            Play(playMode, audioName, Vector3.zero);
        }

        //播放音乐
        public void Play(PlayMode playMode, string audioName, Vector3 pos)
        {
            Mode = playMode;
            m_AudioState = AudioState.Play;
            m_Agent.localPosition = pos;
            AudioName = audioName;
            LoadAudioClip(audioName);
        }

        //暂停
        public void Pause()
        {
            m_AudioState = AudioState.Pause;
            if (m_Source != null && m_Source.isPlaying)
                m_Source.Pause();
        }

        //恢复
        public void Resume()
        {
            if (m_AudioState != AudioState.Pause)
                return;

            m_AudioState = AudioState.Play;
            if (m_Clip != null && !m_Source.isPlaying)
                m_Source.UnPause();
        }

        //停止
        public void Stop()
        {
            m_AudioState = AudioState.Stop;
            if (m_Source != null && m_Source.isPlaying)
                m_Source.Stop();
        }

        //移除
        public void Destroy()
        {
            if (AudioResState != AudioResState.Done)
                return;
            if (AudioName.Equals(string.Empty))
            {
                return;
            }
            Stop();
            m_Clip = null;
            m_Source.clip = null;
            m_UnLoadResAction(m_AssetHandle);
            m_AssetHandle = null;
            AudioName = string.Empty;
        }


        //归还资源
        public void BackClipRes()
        {
            if (AudioResState != AudioResState.Done)
                return;
            if (AudioName.Equals(string.Empty))
            {
                return;
            }
            Stop();
            m_Clip = null;
            m_Source.clip = null;
            m_UnLoadResAction(m_AssetHandle);
            m_AssetHandle = null;
            AudioName = string.Empty;
        }

        //取消
        public void Cancel()
        {
            m_AudioState = AudioState.Cancel;
        }

        //播放音乐
        private void PlayInternal()
        {
            if (!m_Clip || !m_Source || m_AudioState != AudioState.Play)
                return;

            if (m_Source.isPlaying)
                m_Source.Stop();

            m_Source.clip = m_Clip;
            m_Source.volume = 1;// AudioManager.Instance.GetVolume(AudioType);
            m_Source.loop = Mode == PlayMode.Repeat;
            m_Source.pitch = Time.timeScale;
            m_Source.Play();
        }

        //加载音乐资源
        private void LoadAudioClip(string audioName)
        {
            AudioResState = AudioResState.Loading;

            m_LoadResAction(audioName, (handle) =>
            {
                AudioResState = AudioResState.Done;
                m_AssetHandle = handle;
                m_Clip = m_AssetHandle == null || m_AssetHandle.Status != EOperationStatus.Succeed ? null : m_AssetHandle.AssetObject as AudioClip;
                if (m_AudioState == AudioState.Cancel)
                {
                    return;
                }
                PlayInternal();
            }
            );
        }
    }
}
