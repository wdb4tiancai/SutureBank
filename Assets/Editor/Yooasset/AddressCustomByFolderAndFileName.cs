using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset.Editor;

[DisplayName("定位地址: 自定义文件夹名+文件名")]
public class AddressCustomByFolderAndFileName : IAddressRule
{
    string IAddressRule.GetAssetAddress(AddressRuleData data)
    {
        //string fileName = Path.GetFileNameWithoutExtension(data.AssetPath);
        //FileInfo fileInfo = new FileInfo(data.AssetPath);
        return $"{data.AssetPath.Replace($"{data.CollectPath}/", "")}";
    }
}
