
using System;
using UnityEditor;
using UnityEngine;

public class ResPreprocessTexture : AssetPostprocessor
{
    private static string Android = "Android";
    private static string IOS = "iPhone";
    private static string WebGL = "WebGL";

    public void OnPreprocessTexture()
    {
        //将要预处理的资源
        TextureImporter importer = (TextureImporter)assetImporter;

        //资源预处理配置对象
        ResPreprocessTextureCfg textureCfg = AssetDatabase.LoadAssetAtPath<ResPreprocessTextureCfg>("Assets/Editor/ResPostprocessor/PreprocessTextureCfg.asset");

        //找到资源预处理的逻辑
        ResPreprocessTextureCfgItem textureCfgItem = textureCfg.GetCfgByResPath(importer.assetPath);
        if (textureCfgItem == null)
        {
            SetTextureImporterCommon(importer);
            Debug.LogError($"{importer.assetPath} Texture资源没有指定预处理，通用处理");
            textureCfg = null;
            return;
        }

        if (textureCfgItem.TextureType == ResPreprocessTextureEnum.UI)
        {
            //UI资源的处理逻辑
            SetTextureImporterUI(importer, textureCfgItem);
        }
        textureCfg = null;

    }

    //设置Texture资源导入UI
    private static void SetTextureImporterUI(TextureImporter importer, ResPreprocessTextureCfgItem textureCfgItem)
    {
        //设置UI纹理不可读写
        importer.isReadable = false;
        //设置UI纹理Generate Mipmaps
        importer.mipmapEnabled = false;
        //设置UI纹理纹理格式
        importer.textureType = TextureImporterType.Sprite;
        //设置UI纹理WrapMode
        importer.wrapMode = TextureWrapMode.Clamp;

        FormatTexture(ref importer, true, textureCfgItem.Format, textureCfgItem.MaxTextureSizeLimit);
    }

    //设置Texture资源导入通用
    private static void SetTextureImporterCommon(TextureImporter importer)
    {
        FormatTexture(ref importer, false, TextureImporterFormat.ASTC_6x6, 0);
    }



    //设置图片的格式
    public static void FormatTexture(ref TextureImporter importer, bool isAlphaSource, TextureImporterFormat format = TextureImporterFormat.ASTC_8x8, int maxTextureSizeLimit = 0)
    {

        //设置尺寸
        // 获取当前纹理的宽度和高度
        int width, height;
        GetTextureSize(importer.assetPath, out width, out height);
        // 计算合适的 maxTextureSize
        int maxSize = GetMaxTextureSize(width, height);

        //根据限定值设置尺寸
        if (maxTextureSizeLimit > 0)
        {
            maxSize = Math.Min(maxSize, maxTextureSizeLimit);
        }


        //设置透明度
        if (isAlphaSource)
        {
            if (importer.DoesSourceTextureHaveAlpha())
            {
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
            }
            else
            {
                importer.alphaSource = TextureImporterAlphaSource.None;
            }
        }


        //设置Android平台
        TextureImporterPlatformSettings settingAndroid = importer.GetPlatformTextureSettings(Android);
        settingAndroid.maxTextureSize = Math.Min(maxSize, settingAndroid.maxTextureSize);
        settingAndroid.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
        settingAndroid.format = format;
        settingAndroid.compressionQuality = 100;
        settingAndroid.overridden = true;
        importer.SetPlatformTextureSettings(settingAndroid);

        //设置Ios平台
        TextureImporterPlatformSettings settingIos = importer.GetPlatformTextureSettings(IOS);
        settingIos.maxTextureSize = Math.Min(maxSize, settingIos.maxTextureSize);
        settingIos.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
        settingIos.format = format;
        settingIos.compressionQuality = 100;
        settingIos.overridden = true;
        importer.SetPlatformTextureSettings(settingIos);

        //设置Webgl
        TextureImporterPlatformSettings settingWebGL = importer.GetPlatformTextureSettings(WebGL);
        settingWebGL.maxTextureSize = Math.Min(maxSize, settingWebGL.maxTextureSize);
        settingWebGL.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
        settingWebGL.format = format;
        settingWebGL.compressionQuality = 100;
        settingWebGL.overridden = true;
        importer.SetPlatformTextureSettings(settingWebGL);

    }



    //计算填充率
    private static float CalculateTextureFillRate(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            return 0;
        }
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = System.IO.File.ReadAllBytes(path);
        texture.LoadImage(fileData); // 加载图像数据
        Color[] pixels = texture.GetPixels();
        int filledPixelCount = 0;

        foreach (Color pixel in pixels)
        {
            if (pixel.a > 0.01f) // Assuming a pixel with alpha > 0.01 is considered "filled"
            {
                filledPixelCount++;
            }
        }

        int totalPixelCount = texture.width * texture.height;
        return (float)filledPixelCount / totalPixelCount;
    }


    //获取纹理的宽度和高度
    private static void GetTextureSize(string path, out int width, out int height)
    {
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = System.IO.File.ReadAllBytes(path);
        texture.LoadImage(fileData); // 加载图像数据
        width = texture.width;
        height = texture.height;
    }

    //根据宽度和高度计算合适的 maxTextureSize
    private static int GetMaxTextureSize(int width, int height)
    {
        int maxDimension = Mathf.Max(width, height);

        if (maxDimension <= 32)
            return 32;
        else if (maxDimension <= 64)
            return 64;
        else if (maxDimension <= 128)
            return 128;
        else if (maxDimension <= 256)
            return 256;
        else if (maxDimension <= 512)
            return 512;
        else if (maxDimension <= 1024)
            return 1024;
        else if (maxDimension <= 2048)
            return 2048;
        else
            return 4096; // 最大尺寸
    }


}
