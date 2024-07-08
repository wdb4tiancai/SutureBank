using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ResPreprocessTextureEnum
{
    UI = 0,//UI资源
}

[System.Serializable]
public class ResPreprocessTextureCfgItem
{
    public string RemarkName;//备注名字
    public ResPreprocessTextureEnum TextureType;//资源类型
    public TextureImporterFormat Format = TextureImporterFormat.ASTC_8x8;//压缩比列
    public int MaxTextureSizeLimit = 0;//图片最大值限制,<=0 没有限制
    public string[] ResPaths;//资源路径
}

[CreateAssetMenu(fileName = "PreprocessTextureCfg", menuName = "自定义配置/Texture导入配置表", order = 1)]
public class ResPreprocessTextureCfg : ScriptableObject
{
    [SerializeField]
    public ResPreprocessTextureCfgItem[] Configs;


    //通过资源路径获得资源所在的组
    public ResPreprocessTextureCfgItem GetCfgByResPath(string resPath)
    {
        if (Configs == null || Configs.Length <= 0)
        {
            return null;
        }
        for (int i = 0; i < Configs.Length; i++)
        {
            ResPreprocessTextureCfgItem cfgItem = Configs[i];
            for (int j = 0; j < cfgItem.ResPaths.Length; j++)
            {
                if (resPath.Contains(cfgItem.ResPaths[j]))
                {
                    return cfgItem;
                }
            }
        }
        return null;
    }
}
