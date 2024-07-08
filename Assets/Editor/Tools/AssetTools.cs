using UnityEditor;
using UnityEngine;

public class AssetTools : MonoBehaviour
{
    [MenuItem("Assets/拷贝资源路径")]
    static void CopyAssetPath()
    {
        string path = "";
        if (Selection.assetGUIDs != null && Selection.assetGUIDs.Length == 1)
        {
            path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        }

        int dotIndex = path.IndexOf(".");
        if (dotIndex >= 0)
        {
            path = path.Substring(0, dotIndex);
        }

        GUIUtility.systemCopyBuffer = path;
        Debug.LogError(path);
    }


}