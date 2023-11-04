using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    //音乐的类型
    public enum AudioType
    {
        Music = 0, //音乐
        Sound = 1, //音效
        Skill = 2, //技能音效
        UI = 3,    //UI音效
        Voice = 4  //语音
    }

    //音乐的播放模式
    public enum PlayMode
    {
        Single,//单次
        Repeat,//重复
    }

    //音效资源的状态
    public enum AudioResState
    {
        Loading = 1,//加载
        Done = 2,//完成
        Destroy = 2,//销毁
    }

    //音效的状态
    public enum AudioState
    {
        None = 0,//无
        Play,//播放
        Pause,//暂停
        Stop,//停止
        Cancel,//取消
    }
}
