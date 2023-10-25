using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using HybridCLR;
using System.Reflection;

/// <summary>
/// �����ȸ�����
/// </summary>
internal class FsmLoadHotUpdateDll : IStateNode
{
    private StateMachine m_Machine;
    private bool m_IsLoadDllSucceed = true;
    void IStateNode.OnCreate(StateMachine machine)
    {
        m_Machine = machine;
    }
    void IStateNode.OnEnter()
    {
        m_IsLoadDllSucceed = true;
        LauncherEventDefine.LauncherStatesChange.SendEventMessage("�����ȸ����룡");
#if UNITY_EDITOR
        Debug.Log("�����ȸ�����");
        m_Machine.ChangeState<FsmLauncherGame>();
#else
        LauncherBehaviour.Instance.StartCoroutine(LoadHotUpdateDll());

#endif


    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    //����DLL����
    private IEnumerator LoadHotUpdateDll()
    {
        yield return LoadMetadataForAOTAssemblies();
        if (m_IsLoadDllSucceed == false)
        {
            yield break;
        }
        yield return LoadHotUpdateAssemblies();
        if (m_IsLoadDllSucceed == false)
        {
            yield break;
        }
        m_Machine.ChangeState<FsmLauncherGame>();
    }

    //����Aot����
    private IEnumerator LoadMetadataForAOTAssemblies()
    {
        Debug.Log("����Aot����");
        HomologousImageMode mode = HomologousImageMode.SuperSet;

        List<string> allDllNames = new List<string>() { "YooAsset.dll", "mscorlib.dll" };
        foreach (var name in allDllNames)
        {
            var dataHandle = YooAssets.LoadAssetSync(name, typeof(TextAsset));
            yield return dataHandle;
            if (dataHandle.Status != EOperationStatus.Succeed)
            {
                Debug.Log("��Դ����ʧ��" + name);
                m_IsLoadDllSucceed = false;
                yield break;
            }
            // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(((TextAsset)dataHandle.AssetObject).bytes, mode);
            dataHandle.Release();
            if (err != LoadImageErrorCode.OK)
            {
                Debug.Log("��Դ����ʧ��" + name);
                m_IsLoadDllSucceed = false;
                yield break;
            }
            Debug.Log($"LoadMetadataForAOTAssembly:{name}. mode:{mode} ret:{err}");
        }
        yield return true;
    }

    //���ش���
    private IEnumerator LoadHotUpdateAssemblies()
    {
        Debug.Log("���ش���");
        List<string> allDllNames = new List<string>() { "HotScripts.dll" };
        foreach (var name in allDllNames)
        {
            var dataHandle = YooAssets.LoadAssetSync(name, typeof(TextAsset));
            yield return dataHandle;
            if (dataHandle.Status != EOperationStatus.Succeed)
            {
                Debug.Log("��Դ����ʧ��" + name);
                m_IsLoadDllSucceed = false;
                yield break;
            }
            Assembly assembly = Assembly.Load(((TextAsset)dataHandle.AssetObject).bytes);
            dataHandle.Release();
            Debug.Log(assembly.GetTypes());
            Debug.Log($"�����ȸ���Dll:{name}");
        }
        yield return true;
    }
}