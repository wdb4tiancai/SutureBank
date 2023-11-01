using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UIFramework.Editor
{

    public class UIEditor : EditorWindow
    {

        #region 菜单项

        [MenuItem("Assets/UI工具/UI序列化工具", false)]
        public static void OpenSerializedComponentWindow()
        {
            UIEditor win = GetWindow<UIEditor>("Component Viewer");
            win.minSize = new Vector2(480f, 300f);
            win.m_CurObj = Selection.activeGameObject;
            win.Show();
        }


        #endregion


        /// <summary>
        /// 当前预制对象
        /// </summary>
        private GameObject m_CurObj;

        /// <summary>
        /// 上一个预制对象
        /// </summary>
        private GameObject m_PrevObj;

        /// <summary>
        /// ui的根节点
        /// </summary>
        private ObjectComponents m_RootComponents = null;
        /// <summary>
        /// 绘制的组件对象
        /// </summary>
        private List<ObjectComponentsWithIndent> m_DrawingComponents = new List<ObjectComponentsWithIndent>();


        /// <summary>
        /// 代码保存目录
        /// </summary>
        private string[] m_FolderList;
        /// <summary>
        /// 选择的目录Index
        /// </summary>
        private int m_FolderIndex;


        /// <summary>
        /// 当前所有的NameSpace
        /// </summary>
        private string[] m_NameSpaceList;

        /// <summary>
        /// 当前选择的NameSpace
        /// </summary>
        private int m_NameSpaceIndex;

        /// <summary>
        /// 所有的基类
        /// </summary>
        private string[] m_BaseClassList;

        /// <summary>
        /// 默认使用partial类
        /// </summary>
        private bool m_DefaultPartialClass;
        /// <summary>
        /// 默认使用public属性
        /// </summary>
        private bool m_DefaultPublicProperty;
        /// <summary>
        /// 默认Item使用partial类
        /// </summary>
        private bool m_DefaultPartialItemClass;
        /// <summary>
        /// 默认Item使用public属性
        /// </summary>
        private bool m_DefaultPublicItemProperty;

        /// <summary>
        /// GUIStyle是否初始化
        /// </summary>
        private bool m_GUIStyleInited = false;
        //GUI 元素外观
        private GUIStyle m_StyleBoldLabel;
        //GUI 元素外观
        private GUIStyle m_StyleBox;

        //是否去序列号对象
        private bool m_ToSetSerializedObjects = false;
        //滑动区域的位置
        private Vector2 m_Scroll;


        void OnEnable()
        {


            string readCfgPath = Path.Combine(Application.dataPath, "ThirdParty/UIFramework/Editor/UIFrameworkCfg.json");
            UIFrameworkCfg frameworkCfg = JsonMapper.ToObject<UIFrameworkCfg>(File.ReadAllText(readCfgPath));
            m_FolderList = frameworkCfg.FolderList.ToArray();
            m_NameSpaceList = frameworkCfg.NameSpaceList.ToArray();
            m_BaseClassList = frameworkCfg.BaseClassList.ToArray();
            //读取配置文件
            m_DefaultPartialClass = EditorPrefs.GetBool(UiTool.GetMD5Key("partial_class"), false);
            m_DefaultPublicProperty = EditorPrefs.GetBool(UiTool.GetMD5Key("public_property"), true);
            m_DefaultPartialItemClass = EditorPrefs.GetBool(UiTool.GetMD5Key("partial_item_class"), false);
            m_DefaultPublicItemProperty = EditorPrefs.GetBool(UiTool.GetMD5Key("public_item_property"), true);
            m_PrevObj = null;
            if (m_CurObj != null)
            {
                string key = UiTool.GetMD5Key("set_serialized_objects");
                if (EditorPrefs.GetString(key, null) == m_CurObj.name)
                {
                    m_ToSetSerializedObjects = true;
                }
                EditorPrefs.DeleteKey(key);
            }
        }



        void OnGUI()
        {
            if (!m_GUIStyleInited)
            {
                m_GUIStyleInited = true;
                m_StyleBoldLabel = "BoldLabel";
                m_StyleBox = "CN Box";
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
            m_CurObj = EditorGUILayout.ObjectField("Game Object", m_CurObj, typeof(GameObject), true) as GameObject;
            if (m_CurObj != m_PrevObj)
            {
                m_PrevObj = m_CurObj;
                m_DrawingComponents.Clear();
                if (m_CurObj != null)
                {
                    m_RootComponents = ObjectComponentsTools.CollectComponents(m_CurObj.transform, m_CurObj.name, m_CurObj.name, null);
                    if (m_RootComponents != null)
                    {

                        //查找有没有本地文件
                        string[] scripts = AssetDatabase.FindAssets("t:MonoScript");
                        string findPath = null;
                        for (int i = 0, imax = scripts.Length; i < imax; i++)
                        {
                            string scriptPath = AssetDatabase.GUIDToAssetPath(scripts[i]);
                            if (!scriptPath.EndsWith("/" + m_RootComponents.ClassName + ".cs"))
                            {
                                continue;
                            }
                            findPath = scriptPath.Substring(0, scriptPath.Length - m_RootComponents.ClassName.Length - 4);
                        }
                        if (!string.IsNullOrEmpty(findPath))
                        {
                            //设置默认导出文件
                            for (int i = 0; i < m_FolderList.Length; i++)
                            {
                                if (m_FolderList[i].Equals(findPath))
                                {
                                    m_FolderIndex = i;
                                    break;
                                }
                            }
                            CodeProperties cp = CodePropertiesMgr.CheckCodeAtPath(string.Concat(Path.Combine(findPath, m_RootComponents.ClassName), ".cs"));
                            if (cp != null && cp.ClassName == m_RootComponents.ClassName)
                            {
                                for (int i = 0; i < m_NameSpaceList.Length; i++)
                                {
                                    if (m_NameSpaceList[i].Equals(cp.NameSpace))
                                    {
                                        m_NameSpaceIndex = i;
                                        break;
                                    }
                                }
                            }
                        }

                        Stack<ObjectComponentsWithIndent> componentsStack = new Stack<ObjectComponentsWithIndent>();
                        m_DrawingComponents.Clear();
                        ObjectComponentsWithIndent ocwi = new ObjectComponentsWithIndent();
                        ocwi.Indent = 0;
                        ocwi.Components = m_RootComponents;
                        ocwi.FieldKey = ocwi.Components.Name + "___" + "_" + ocwi.Indent;
                        componentsStack.Push(ocwi);
                        while (componentsStack.Count > 0)
                        {
                            ocwi = componentsStack.Pop();
                            m_DrawingComponents.Add(ocwi);
                            List<ObjectComponents> ocsList = ocwi.Components.ChildComponents;
                            if (ocsList != null)
                            {
                                for (int i = ocsList.Count - 1; i >= 0; i--)
                                {
                                    ObjectComponentsWithIndent nocwi = new ObjectComponentsWithIndent();
                                    nocwi.Indent = ocwi.Indent + 1;
                                    nocwi.Components = ocsList[i];
                                    nocwi.FieldKey = ocwi.Components.Obj.name + "___" + nocwi.Components.Name + "_" + nocwi.Indent;
                                    componentsStack.Push(nocwi);
                                }
                            }
                            if (ocwi.Indent <= 0)
                            {
                                ocwi.Components.IsPartialClass = m_DefaultPartialClass;
                                ocwi.Components.IsPublicProperty = m_DefaultPublicProperty;
                            }
                            else
                            {
                                ocwi.Components.IsPartialClass = m_DefaultPartialItemClass;
                                ocwi.Components.IsPublicProperty = m_DefaultPublicItemProperty;
                            }
                            CodeProperties cp = null;
                            if (!string.IsNullOrEmpty(findPath) && !string.IsNullOrEmpty(ocwi.Components.ClassName))
                            {
                                cp = CodePropertiesMgr.CheckCodeAtPath(string.Concat(Path.Combine(findPath, ocwi.Components.ClassName), ".cs"));
                            }
                            if (cp != null && cp.ClassName == ocwi.Components.ClassName)
                            {
                                ocwi.Components.IsPartialClass = cp.IsPartialClass;
                                ocwi.Components.IsPublicProperty = cp.IsPublicProperty;

                                for (int i = 0; i < m_BaseClassList.Length; i++)
                                {
                                    if (m_BaseClassList[i].Equals(cp.BaseClass))
                                    {
                                        ocwi.Components.BaseClass = cp.BaseClass;
                                        ocwi.Components.BaseClassIndex = i;
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            if (m_CurObj == null && m_DrawingComponents.Count > 0)
            {
                m_DrawingComponents.Clear();
            }
            GUILayout.Space(8f);
            #region code folder
            EditorGUILayout.LabelField("生成代码目录");
            EditorGUILayout.BeginHorizontal();
            m_FolderIndex = EditorGUILayout.Popup(m_FolderIndex, m_FolderList);
            EditorGUILayout.EndHorizontal();
            #endregion
            GUILayout.Space(8f);
            #region name space
            EditorGUILayout.BeginHorizontal();
            m_NameSpaceIndex = EditorGUILayout.Popup("代码命名空间", m_NameSpaceIndex, m_NameSpaceList);
            EditorGUILayout.EndHorizontal();
            #endregion
            EditorGUI.BeginChangeCheck();
            m_DefaultPartialClass = EditorGUILayout.Toggle("默认使用partial类", m_DefaultPartialClass);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(UiTool.GetMD5Key("partial_class"), m_DefaultPartialClass);
            }
            EditorGUI.BeginChangeCheck();
            m_DefaultPublicProperty = EditorGUILayout.Toggle("默认使用public属性", m_DefaultPublicProperty);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(UiTool.GetMD5Key("public_property"), m_DefaultPublicProperty);
            }
            EditorGUI.BeginChangeCheck();
            m_DefaultPartialItemClass = EditorGUILayout.Toggle("默认Item使用partial类", m_DefaultPartialItemClass);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(UiTool.GetMD5Key("partial_item_class"), m_DefaultPartialItemClass);
            }
            EditorGUI.BeginChangeCheck();
            m_DefaultPublicItemProperty = EditorGUILayout.Toggle("默认Item使用public属性", m_DefaultPublicItemProperty);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(UiTool.GetMD5Key("public_item_property"), m_DefaultPublicItemProperty);
            }
            GUILayout.Space(8f);
            EditorGUILayout.LabelField("序列化节点组件列表");
            m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll, false, false);
            int count = m_DrawingComponents.Count;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < count; i++)
            {
                Color cachedBgColor = GUI.backgroundColor;
                if ((i & 1) == 0)
                {
                    GUI.backgroundColor = cachedBgColor * 0.8f;
                }
                int indent = m_DrawingComponents[i].Indent;
                if (indent > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(12f * indent);
                }
                EditorGUILayout.BeginVertical(m_StyleBox, GUILayout.MinHeight(10f));
                GUI.backgroundColor = cachedBgColor;
                ObjectComponents ocs = m_DrawingComponents[i].Components;
                int cCount = ocs.Count;
                EditorGUILayout.LabelField(ocs.Name, m_StyleBoldLabel);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(12f);
                EditorGUILayout.BeginVertical();
                for (int j = 0; j < cCount; j++)
                {
                    ComponentData cd = ocs[j];
                    EditorGUILayout.ObjectField(cd.Type.showName, cd.Component, cd.Type.type, true);
                }
                if (ocs.ChildComponents != null)
                {
                    ocs.IsPartialClass = EditorGUILayout.Toggle("使用partial类", ocs.IsPartialClass);
                    ocs.IsPublicProperty = EditorGUILayout.Toggle("使用public属性", ocs.IsPublicProperty);
                    EditorGUILayout.BeginHorizontal();
                    if (ocs.BaseClassIndex < 0)
                    {
                        ocs.BaseClass = EditorGUILayout.DelayedTextField("继承自基类", ocs.BaseClass);
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        ocs.BaseClassIndex = EditorGUILayout.Popup("继承自基类", ocs.BaseClassIndex, m_BaseClassList);
                        if (EditorGUI.EndChangeCheck())
                        {
                            ocs.BaseClass = m_BaseClassList[ocs.BaseClassIndex];
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                if (indent > 0)
                {
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(4f);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(m_CurObj == null || m_RootComponents == null);
            if (GUILayout.Button("生成并挂载", GUILayout.Width(80f)))
            {
                if (ErrorCheck())
                {
                    EditorGUILayout.EndHorizontal();
                    return;
                }

                string folder = m_FolderList[m_FolderIndex];
                string ns = m_NameSpaceList[m_NameSpaceIndex];


                if (!folder.EndsWith("/")) { folder = folder + "/"; }
                List<CodeObject> codes = ObjectCodeGen.GetCodes(m_RootComponents, ns);
                bool flag = false;
                for (int i = 0, imax = codes.Count; i < imax; i++)
                {
                    CodeObject code = codes[i];
                    string path = string.Concat(folder, code.filename);
                    string fileMd5 = null;
                    if (File.Exists(path))
                    {
                        try
                        {
                            using (FileStream fs = File.OpenRead(path))
                            {
                                fileMd5 = BitConverter.ToString(UiTool.GetMd5Calc().ComputeHash(fs));
                            }
                        }
                        catch (Exception e) { Debug.LogException(e); }
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(code.code);
                    bool toWrite = true;
                    if (!string.IsNullOrEmpty(fileMd5))
                    {
                        if (BitConverter.ToString(UiTool.GetMd5Calc().ComputeHash(bytes)) == fileMd5)
                        {
                            toWrite = false;
                        }
                    }
                    if (toWrite)
                    {
                        File.WriteAllBytes(path, bytes);
                        flag = true;
                    }
                }
                //if (flag) {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //}
                if (EditorApplication.isCompiling)
                {
                    EditorPrefs.SetString(UiTool.GetMD5Key("set_serialized_objects"), m_CurObj.name);
                }
                else
                {
                    m_ToSetSerializedObjects = true;
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            if (m_ToSetSerializedObjects)
            {
                m_ToSetSerializedObjects = false;
                ObjectSerialize.SerializeObject(m_NameSpaceList[m_NameSpaceIndex], m_RootComponents);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }


        private bool ErrorCheck()
        {
            HashSet<string> specialKey = new HashSet<string>() { "name", "type", "animation" };    //要求全转小写
            var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("C#");
            HashSet<string> fieldHashSet = new HashSet<string>();
            foreach (var comp in m_DrawingComponents)
            {
                var key = comp.FieldKey;
                if (!provider.IsValidIdentifier(comp.Components.Name) || specialKey.Contains(comp.Components.Name.ToLower()))
                {
                    string content = $"field [{comp.Components.Name}] is Valid Keyword";
                    if (EditorUtility.DisplayDialog("序列化失败", content, "确认"))
                    {
                        GUIUtility.systemCopyBuffer = comp.Components.Name;
                        Selection.activeObject = comp.Components.Obj;
                    }

                    return true;
                }
                if (fieldHashSet.Contains(key))
                {

                    string content = $"field [{comp.Components.Name}] is Duplicated";
                    if (EditorUtility.DisplayDialog("序列化失败", content, "确认"))
                    {
                        GUIUtility.systemCopyBuffer = comp.Components.Name;
                        Selection.activeObject = comp.Components.Obj;
                    }

                    return true;
                }

                fieldHashSet.Add(key);
            }

            return false;
        }
    }
}