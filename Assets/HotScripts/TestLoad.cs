using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using YooAsset;

public class TestLoad : MonoBehaviour
{
    ScreenInfo screenInfo;
    void Start()
    {
        AssetHandle handle = YooAssets.LoadAssetAsync<GameObject>("TestCube");
        handle.Completed += Handle_Completed;
    }


    void Handle_Completed(AssetHandle handle)
    {
        handle.InstantiateAsync();
    }
}
