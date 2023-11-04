using SharePublic;
using System.Collections;
using UniFramework.Event;
using UnityEngine;
using YooAsset;


public class Launcher : MonoBehaviour
{
    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    /// <summary>
    /// 资源包的名字
    /// </summary>
    public string AssetPackageName = string.Empty;

    /// <summary>
    /// 启动Ui
    /// </summary>
    /// 
    public string LauncherUiPath = string.Empty;

    void Awake()
    {
        Debug.Log($"资源系统运行模式：{PlayMode}");
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }
    void Start()
    {
        //初始化资源包
        AssetsVersion.AssetPackageName = AssetPackageName;

        // 游戏管理器
        LauncherBehaviour.Instance.Behaviour = this;

        // 初始化事件系统
        UniEvent.Initalize();

        // 初始化资源系统
        YooAssets.Initialize();

        // 开始补丁更新流程
        LauncherOperation operation = new LauncherOperation(LauncherUiPath, AssetPackageName, EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), PlayMode);
        YooAssets.StartOperation(operation);
    }
}