using System.Collections;
using UniFramework.Event;
using UnityEngine;
using YooAsset;


public class Launcher : MonoBehaviour
{
    /// <summary>
    /// ��Դϵͳ����ģʽ
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    /// <summary>
    /// ��Դ��������
    /// </summary>
    public string AssetPackageName = string.Empty;

    /// <summary>
    /// ����Ui
    /// </summary>
    /// 
    public string LauncherUiPath = string.Empty;


    void Awake()
    {
        Debug.Log($"��Դϵͳ����ģʽ��{PlayMode}");
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        // ��Ϸ������
        LauncherBehaviour.Instance.Behaviour = this;

        // ��ʼ���¼�ϵͳ
        UniEvent.Initalize();

        // ��ʼ����Դϵͳ
        YooAssets.Initialize();

        // ��ʼ������������
        LauncherOperation operation = new LauncherOperation(LauncherUiPath, AssetPackageName, "", PlayMode);//EDefaultBuildPipeline.BuiltinBuildPipeline.ToString()
        YooAssets.StartOperation(operation);
    }
}