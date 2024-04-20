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

    /// <summary>
    /// 协程启动器
    /// </summary>
    public MonoBehaviour Behaviour;


    /// <summary>
    /// 开启一个协程
    /// </summary>
    public void StartCoroutine(IEnumerator enumerator)
    {
        Behaviour.StartCoroutine(enumerator);
    }

}