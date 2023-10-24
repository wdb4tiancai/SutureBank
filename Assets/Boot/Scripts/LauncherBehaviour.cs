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
    /// Э��������
    /// </summary>
    public MonoBehaviour Behaviour;


    private LauncherBehaviour()
    {
        // ע������¼�
        _eventGroup.AddListener<SceneEventDefine.ChangeToHomeScene>(OnHandleEventMessage);
        _eventGroup.AddListener<SceneEventDefine.ChangeToBattleScene>(OnHandleEventMessage);
    }

    /// <summary>
    /// ����һ��Э��
    /// </summary>
    public void StartCoroutine(IEnumerator enumerator)
    {
        Behaviour.StartCoroutine(enumerator);
    }

    /// <summary>
    /// �����¼�
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is SceneEventDefine.ChangeToHomeScene)
        {
            YooAssets.LoadSceneAsync("scene_home");
        }
        else if (message is SceneEventDefine.ChangeToBattleScene)
        {
            YooAssets.LoadSceneAsync("scene_battle");
        }
    }
}