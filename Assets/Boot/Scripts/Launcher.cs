using SharePublic;
using UniFramework.Event;
using UnityEngine;
using YooAsset;


public class Launcher : MonoBehaviour
{

    void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }
    void Start()
    {
        AssetsVersion.BuildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline.ToString();

        // 游戏管理器
        LauncherBehaviour.Instance.Behaviour = this;

        // 初始化事件系统
        UniEvent.Initalize();

        // 初始化资源系统
        YooAssets.Initialize();

        // 开始补丁更新流程
        LauncherOperation operation = new LauncherOperation();

        YooAssets.StartOperation(operation);
    }
}