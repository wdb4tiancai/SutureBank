using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Event;
using YooAsset;

public class LauncherBehaviour
{
    private static LauncherBehaviour _instance;
    public static LauncherBehaviour Instance
    {
        get
        {
            if (_instance == null)
                _instance = new LauncherBehaviour();
            return _instance;
        }
    }

    private readonly EventGroup _eventGroup = new EventGroup();

    /// <summary>
    /// 协程启动器
    /// </summary>
    public MonoBehaviour Behaviour;


    private LauncherBehaviour()
    {
        // 注册监听事件
        _eventGroup.AddListener<LauncherEventDefine.ChangeToLoginScene>(OnHandleEventMessage);
    }

    /// <summary>
    /// 开启一个协程
    /// </summary>
    public void StartCoroutine(IEnumerator enumerator)
    {
        Behaviour.StartCoroutine(enumerator);
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is LauncherEventDefine.ChangeToLoginScene)
        {
            YooAssets.LoadSceneAsync("SceneLogin");
        }
    }
}