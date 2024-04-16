using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DeletCompentTools : EditorWindow
{
    [MenuItem("工具/ResTools/预制删除工具集")]
    static void Win()
    {
        EditorWindow.GetWindow<DeletCompentTools>(false, "小工具", true).Show();
    }
    // Use this for initialization
    void OnGUI()
    {

        if (GUILayout.Button("删除丢失脚本"))
        {
            DeletMissingScript();
        }


    }
    /// <summary>
    /// 删除丢失脚本
    /// </summary>
    void DeletMissingScript()
    {

        GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

        for (int i = 0; i < pAllObjects.Length; i++)
        {
            if (pAllObjects[i].hideFlags == HideFlags.None)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(pAllObjects[i]);
            }
        }
    }
}


