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

    public class SerializedComponentWindow : EditorWindow
    {

        #region MenuItem

        [MenuItem("ProjectTools/UIWindowSerialized", false, 20)]
        [MenuItem("Cheetah/Serialize Tools/C# Serialized Component")]
        [MenuItem("Assets/Cheetah/Serialize Tools/C# Serialized Component", false)]
        public static void OpenSerializedComponentWindow()
        {
            SerializedComponentWindow win = GetWindow<SerializedComponentWindow>("Component Viewer");
            win.minSize = new Vector2(480f, 300f);
            win.mObj = Selection.activeGameObject;
            win.Show();
        }


        [MenuItem("Assets/Cheetah/Serialize Tools/C# Serialized Component", true)]
        static bool CanAssetOpenSerializedComponentWindow()
        {
            return Selection.activeGameObject != null;
        }

        #endregion


        #region assist

        private class CodeProperties
        {
            public string nameSpace;
            public bool partialClass;
            public string className;
            public string baseClass;
            public bool publicProperty;
        }

        static Regex reg_namespace = new Regex(@"namespace\s+((\S+\s*\.\s*)*\S+)\s*\{");
        static Regex reg_class_def = new Regex(@"public\s+(partial\s+){0,1}class\s+(\S+)\s*:\s*(\S+)");
        static Regex reg_subclass_start = new Regex(@"\[System\.Serializable\]");
        static Regex reg_public_property = new Regex(@"public\s+\S+\s+\S+\s*\{\s*get\s*\{\s*return\s+\S+\s*;\s*\}\s*\}");
        private static CodeProperties CheckCodeAtPath(string path)
        {
            if (!File.Exists(path)) { return null; }
            string code = null;
            try
            {
                code = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            if (code == null) { return null; }
            //Log.w(code);
            Match matchClassDefine = reg_class_def.Match(code);
            if (!matchClassDefine.Success) { return null; }
            int i0 = matchClassDefine.Index + matchClassDefine.Length;
            GroupCollection groups = matchClassDefine.Groups;
            CodeProperties cp = new CodeProperties();
            cp.partialClass = groups[1].Success;
            cp.className = groups[2].Value;
            cp.baseClass = groups[3].Value;
            Match matchNS = reg_namespace.Match(code);
            string ns = matchNS.Success ? matchNS.Groups[1].Value : null;
            if (!string.IsNullOrEmpty(ns))
            {
                ns = ns.Replace(" ", "").Replace("\t", "");
            }
            cp.nameSpace = ns;
            Match matchSubclassStart = reg_subclass_start.Match(code, i0);
            int i1 = code.Length;
            if (matchSubclassStart.Success) { i1 = matchSubclassStart.Index; }
            cp.publicProperty = reg_public_property.Match(code, i0, i1 - i0).Success;
            //Log.df("partial : {0}  public property : {1}  base class : {2}",
            //	cp.isPartialClass, cp.publicProperty, cp.baseClass);
            return cp;
        }

        #endregion

        private static MD5CryptoServiceProvider md5_calc;
        private static string GetKey(string key)
        {
            if (md5_calc == null)
            {
                md5_calc = new MD5CryptoServiceProvider();
            }
            string str = string.Concat(Application.dataPath, "SerializedComponent4CSharp", key);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            return BitConverter.ToString(md5_calc.ComputeHash(bytes));
        }

        private GameObject mObj;
        private GameObject mPrevObj;

        /// <summary>
        /// ui的根节点
        /// </summary>
        private ObjectComponents m_RootComponents = null;
        /// <summary>
        /// 绘制的组件对象
        /// </summary>
        private List<ObjectComponentsWithIndent> m_DrawingComponents = new List<ObjectComponentsWithIndent>();


        /// <summary>
        /// 是否手动选择代码保存目录
        /// </summary>
        private bool m_FolderManualEdit = false;
        /// <summary>
        /// 代码保存目录
        /// </summary>
        private string[] m_FolderList;
        /// <summary>
        /// 选择的目录Index
        /// </summary>
        private int m_FolderIndex;

        /// <summary>
        /// 是否手动选择NameSpace
        /// </summary>
        private bool m_NameSpaceManualEdit = false;
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

        private bool mDefaultPartialClass;
        private bool mDefaultPublicProperty;
        private bool mDefaultPartialItemClass;
        private bool mDefaultPublicItemProperty;

        private bool mGUIStyleInited = false;
        private GUIStyle mStyleBoldLabel;
        private GUIStyle mStyleBox;

        private bool mToSetSerializedObjects = false;

        private Vector2 mScroll;

        static private Dictionary<string, string> nsCache = new Dictionary<string, string>();

        void OnEnable()
        {
            m_FolderList = new string[1] { "Assets" };
            m_NameSpaceList = new string[1] { "GameUi" };
            m_BaseClassList = new string[1] { "BaseUi" };
            //读取配置文件
            mDefaultPartialClass = EditorPrefs.GetBool(GetKey("partial_class"), false);
            mDefaultPublicProperty = EditorPrefs.GetBool(GetKey("public_property"), true);
            mDefaultPartialItemClass = EditorPrefs.GetBool(GetKey("partial_item_class"), false);
            mDefaultPublicItemProperty = EditorPrefs.GetBool(GetKey("public_item_property"), true);
            mPrevObj = null;
            if (mObj != null)
            {
                string key = GetKey("set_serialized_objects");
                if (EditorPrefs.GetString(key, null) == mObj.name)
                {
                    mToSetSerializedObjects = true;
                }
                EditorPrefs.DeleteKey(key);
            }
        }



        void OnGUI()
        {
            if (!mGUIStyleInited)
            {
                mGUIStyleInited = true;
                mStyleBoldLabel = "BoldLabel";
                mStyleBox = "CN Box";
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
            mObj = EditorGUILayout.ObjectField("Game Object", mObj, typeof(GameObject), true) as GameObject;
            if (mObj != mPrevObj)
            {
                mPrevObj = mObj;
                m_DrawingComponents.Clear();
                if (mObj != null)
                {
                    m_RootComponents = ObjectComponentsTools.CollectComponents(mObj.transform, mObj.name, mObj.name, null);
                }
            }
            if (mObj == null && m_DrawingComponents.Count > 0)
            {
                m_DrawingComponents.Clear();
            }
            GUILayout.Space(8f);
            #region code folder
            EditorGUILayout.LabelField("生成代码目录");
            EditorGUILayout.BeginHorizontal();
            if (m_FolderManualEdit)
            {
                int index = m_FolderList.Length - 1;
                EditorGUI.BeginChangeCheck();
                m_FolderList[index] = EditorGUILayout.DelayedTextField(m_FolderList[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    m_FolderIndex = index;
                }
            }
            else
            {
                m_FolderIndex = EditorGUILayout.Popup(m_FolderIndex, m_FolderList);
            }
            if (GUILayout.Button(m_FolderManualEdit ? "选择" : "手动", GUILayout.Width(36f)))
            {
                m_FolderManualEdit = !m_FolderManualEdit;
                if (m_FolderManualEdit)
                {
                    m_FolderList[m_FolderList.Length - 1] = m_FolderList[m_FolderIndex];
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion
            GUILayout.Space(8f);
            #region name space
            EditorGUILayout.BeginHorizontal();
            if (m_NameSpaceManualEdit)
            {
                int index = m_NameSpaceList.Length - 1;
                EditorGUI.BeginChangeCheck();
                m_NameSpaceList[index] = EditorGUILayout.DelayedTextField("代码命名空间", m_NameSpaceList[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    m_NameSpaceIndex = index;
                }
            }
            else
            {
                m_NameSpaceIndex = EditorGUILayout.Popup("代码命名空间", m_NameSpaceIndex, m_NameSpaceList);
            }
            if (GUILayout.Button(m_NameSpaceManualEdit ? "选择" : "手动", GUILayout.Width(36f)))
            {
                m_NameSpaceManualEdit = !m_NameSpaceManualEdit;
                if (m_NameSpaceManualEdit)
                {
                    m_NameSpaceList[m_NameSpaceList.Length - 1] = m_NameSpaceList[m_NameSpaceIndex];
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion
            EditorGUI.BeginChangeCheck();
            mDefaultPartialClass = EditorGUILayout.Toggle("默认使用partial类", mDefaultPartialClass);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(GetKey("partial_class"), mDefaultPartialClass);
            }
            EditorGUI.BeginChangeCheck();
            mDefaultPublicProperty = EditorGUILayout.Toggle("默认使用public属性", mDefaultPublicProperty);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(GetKey("public_property"), mDefaultPublicProperty);
            }
            EditorGUI.BeginChangeCheck();
            mDefaultPartialItemClass = EditorGUILayout.Toggle("默认Item使用partial类", mDefaultPartialItemClass);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(GetKey("partial_item_class"), mDefaultPartialItemClass);
            }
            EditorGUI.BeginChangeCheck();
            mDefaultPublicItemProperty = EditorGUILayout.Toggle("默认Item使用public属性", mDefaultPublicItemProperty);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(GetKey("public_item_property"), mDefaultPublicItemProperty);
            }
            GUILayout.Space(8f);
            EditorGUILayout.LabelField("序列化节点组件列表");
            mScroll = EditorGUILayout.BeginScrollView(mScroll, false, false);
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
                EditorGUILayout.BeginVertical(mStyleBox, GUILayout.MinHeight(10f));
                GUI.backgroundColor = cachedBgColor;
                ObjectComponents ocs = m_DrawingComponents[i].Components;
                int cCount = ocs.Count;
                EditorGUILayout.LabelField(ocs.Name, mStyleBoldLabel);
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
                        m_BaseClassList[m_BaseClassList.Length - 1] = ocs.BaseClassIndex >= m_BaseClassList.Length - 1 ?
                            ocs.BaseClass : "";
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
            EditorGUI.BeginDisabledGroup(mObj == null || m_RootComponents == null);
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
                List<CodeObject> codes = GetCodes(ns);
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
                                fileMd5 = BitConverter.ToString(md5_calc.ComputeHash(fs));
                            }
                        }
                        catch (Exception e) { Debug.LogException(e); }
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(code.code);
                    bool toWrite = true;
                    if (!string.IsNullOrEmpty(fileMd5))
                    {
                        if (BitConverter.ToString(md5_calc.ComputeHash(bytes)) == fileMd5)
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
                    EditorPrefs.SetString(GetKey("set_serialized_objects"), mObj.name);
                }
                else
                {
                    mToSetSerializedObjects = true;
                }
            }
            if (GUILayout.Button("预览代码", GUILayout.Width(80f)))
            {
                Rect rect = position;
                rect.x += 32f;
                rect.y += 32f;
                rect.width = 800f;
                CodePreviewWindow pw = GetWindow<CodePreviewWindow>("代码预览");
                pw.position = rect;
                pw.codes = GetCodes(m_NameSpaceList[m_NameSpaceIndex]);
                pw.Show();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            if (mToSetSerializedObjects)
            {
                mToSetSerializedObjects = false;
                SerializeObject(m_NameSpaceList[m_NameSpaceIndex], m_RootComponents);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void SerializeObject(string ns, ObjectComponents ocs)
        {
            Queue<ObjectComponents> components = new Queue<ObjectComponents>();
            components.Enqueue(ocs);
            Stack<ObjectComponents> sortedComponents = new Stack<ObjectComponents>();
            while (components.Count > 0)
            {
                ObjectComponents oc = components.Dequeue();
                sortedComponents.Push(oc);
                for (int i = 0, imax = oc.ChildComponents.Count; i < imax; i++)
                {
                    ObjectComponents ioc = oc.ChildComponents[i];
                    if (ioc.ChildComponents != null) { components.Enqueue(ioc); }
                }
            }
            while (sortedComponents.Count > 0)
            {
                ObjectComponents oc = sortedComponents.Pop();
                //TODO find type
                string typeFullName = string.IsNullOrEmpty(ns) ? oc.ClassName : string.Concat(ns, ".", oc.ClassName);
                //Log.d(typeFullName);
                Type type = Type.GetType(typeFullName + ",Game");
                //End TODO
                //Log.d(type);
                if (type == null)
                {
                    Debug.LogErrorFormat("Cannot Find Type for {0} !", oc.ClassName);
                }
                else if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    Debug.LogErrorFormat("Type : '{0}' is not subclass of MonoBehaviour !", type.FullName);
                }
                else
                {
                    oc.Type = type;
                    Component component = oc.Obj.GetComponent(type);
                    if (component == null)
                    {
                        component = oc.Obj.AddComponent(type);
                    }
                    SerializedObject so = new SerializedObject(component);
                    List<ObjectComponents> itemComponents = oc.ChildComponents;
                    int count = itemComponents.Count;
                    for (int i = 0; i < count; i++)
                    {
                        ObjectComponents ioc = itemComponents[i];
                        SerializedProperty pObj = so.FindProperty(oc.IsPublicProperty ? "m_" + ioc.Name : ioc.Name);
                        if (pObj == null)
                        {
                            Debug.LogErrorFormat(ioc.Obj, "Cannot Find property for node '{0}' !", ioc.Obj.name);
                            continue;
                        }
                        SerializedProperty pGO = pObj.FindPropertyRelative("m_GameObject");
                        pGO.objectReferenceValue = ioc.Obj;
                        int cCount = ioc.Count;
                        for (int j = 0; j < cCount; j++)
                        {
                            ComponentData cd = ioc[j];
                            //Log.dt(ioc.name, cd.type.codeTypeName + " " + cd.type.variableName);
                            SerializedProperty pComponent = pObj.FindPropertyRelative("m_" + cd.Type.variableName);
                            if (pComponent == null)
                            {
                                Debug.LogErrorFormat(cd.Component, "Cannot Find property for Component '{0}' at '{1}' !",
                                    cd.Type.variableName, ioc.Obj.name);
                                continue;
                            }
                            pComponent.objectReferenceValue = cd.Component;
                        }
                        if (ioc.ChildComponents != null && ioc.Type != null)
                        {
                            SerializedProperty pItem = pObj.FindPropertyRelative("m_" + ioc.ClassVarName);
                            if (pItem == null)
                            {
                                Debug.LogErrorFormat(ioc.Obj, "Cannot Find item property for Component '{0}' at '{1}' !",
                                    ioc.Type.Name, ioc.Obj.name);
                            }
                            else
                            {
                                pItem.objectReferenceValue = ioc.Obj.GetComponent(ioc.Type);
                            }
                        }
                    }
                    so.ApplyModifiedProperties();
                }
            }
        }

        private List<CodeObject> GetCodes(string ns)
        {
            nsCache.Clear();
            List<CodeObject> codes = new List<CodeObject>();
            if (m_RootComponents == null) { return codes; }
            List<ClassData> clses = new List<ClassData>();
            Queue<ObjectComponents> components = new Queue<ObjectComponents>();
            components.Enqueue(m_RootComponents);
            while (components.Count > 0)
            {
                ObjectComponents ocs = components.Dequeue();
                List<ObjectComponents> itemComponents = ocs.ChildComponents;
                if (itemComponents == null) { continue; }
                ClassData cd = null;
                for (int i = clses.Count - 1; i >= 0; i--)
                {
                    if (clses[i].cls == ocs.ClassName)
                    {
                        cd = clses[i];
                        break;
                    }
                }
                if (cd == null)
                {
                    cd = new ClassData();
                    cd.cls = ocs.ClassName;
                    clses.Add(cd);
                }
                if (!GetClass(ocs, cd))
                {
                    // TODO class not match...
                }
                /*string code = GetCode(ns, ocs);
				if (!string.IsNullOrEmpty(code)) {
					codes.Add(new CodeObject(string.Concat(ocs.cls, ".cs"), code));
				}*/
                for (int i = 0, imax = itemComponents.Count; i < imax; i++)
                {
                    ObjectComponents ioc = itemComponents[i];
                    if (ioc.ChildComponents == null) { continue; }
                    components.Enqueue(ioc);
                }
            }
            Comparison<SupportedTypeData> typeSorter = SupportedTypeDataMgr.SortSupportedTypeDatas;

            for (int i = 0, imax = clses.Count; i < imax; i++)
            {
                nsCache.Add(clses[i].cls, clses[i].baseClass);
            }

            for (int i = 0, imax = clses.Count; i < imax; i++)
            {
                ClassData cls = clses[i];
                for (int j = cls.fields.Count - 1; j >= 0; j--)
                {
                    cls.fields[j].components.Sort(typeSorter);
                }
                string code = GetCode(ns, clses[i]);
                if (!string.IsNullOrEmpty(code))
                {
                    codes.Add(new CodeObject(string.Concat(cls.cls, ".cs"), code));
                }
            }
            return codes;
        }

        private class ClassData
        {
            public List<string> usings = new List<string>();
            public string cls;
            public string baseClass;
            public bool partialClass;
            public bool publicProperty;
            public List<FieldData> fields = new List<FieldData>();
        }
        private class FieldData
        {
            public string name;
            public string itemType;
            public string itemVar;
            public List<SupportedTypeData> components = new List<SupportedTypeData>();
        }

        private static bool GetClass(ObjectComponents ocs, ClassData cls)
        {
            if (cls.cls != ocs.ClassName) { return false; }
            if (cls.baseClass != null && cls.baseClass != ocs.BaseClass) { return false; }
            cls.baseClass = ocs.BaseClass;
            cls.partialClass |= ocs.IsPartialClass;
            cls.publicProperty |= ocs.IsPublicProperty;
            List<ObjectComponents> objComponents = ocs.ChildComponents;
            for (int i = 0, imax = objComponents.Count; i < imax; i++)
            {
                ObjectComponents oc = objComponents[i];
                FieldData field = null;
                for (int j = cls.fields.Count - 1; j >= 0; j--)
                {
                    FieldData f = cls.fields[j];
                    if (f.name == oc.Name)
                    {
                        field = f;
                        break;
                    }
                }
                if (field == null)
                {
                    field = new FieldData();
                    field.name = oc.Name;
                    field.itemType = null;
                    cls.fields.Add(field);
                }
                for (int j = 0, jmax = oc.Count; j < jmax; j++)
                {
                    SupportedTypeData type = oc[j].Type;
                    for (int k = field.components.Count - 1; k >= 0; k--)
                    {
                        if (field.components[k].showName == type.showName)
                        {
                            type = null;
                            break;
                        }
                    }
                    if (type != null) { field.components.Add(type); }
                }
                if (oc.ChildComponents != null)
                {
                    string itemClass = oc.ClassName;
                    for (int k = field.components.Count - 1; k >= 0; k--)
                    {
                        if (field.components[k].showName == itemClass)
                        {
                            itemClass = null;
                            break;
                        }
                    }
                    if (itemClass != null)
                    {
                        field.components.Add(new SupportedTypeData(null, 10000, oc.ClassName, null, oc.ClassName, oc.ClassVarName));
                        field.itemType = oc.ClassName;
                        field.itemVar = oc.ClassVarName;
                    }
                }
            }
            return true;
        }

        private static string GetCode(string ns, ClassData cls)
        {
            List<string> usings = new List<string>();
            SortedList<string, SupportedTypeData[]> dataClasses = new SortedList<string, SupportedTypeData[]>();
            StringBuilder code = new StringBuilder();
            Dictionary<string, KeyValuePair<string, string>> itemClasses = new Dictionary<string, KeyValuePair<string, string>>();
            string codeIndent = "";
            if (!string.IsNullOrEmpty(ns))
            {
                codeIndent = "\t";
                code.AppendLine(string.Format("namespace {0} {{", ns));
                code.AppendLine();
            }
            code.AppendLine(string.Format("{0}public {1}class {2} : {3} {{",
                codeIndent, cls.partialClass ? "partial " : "", cls.cls, cls.baseClass));
            code.AppendLine();
            List<string> tempStrings = new List<string>();
            for (int i = 0, imax = cls.fields.Count; i < imax; i++)
            {
                FieldData field = cls.fields[i];
                tempStrings.Clear();
                for (int j = 0, jmax = field.components.Count; j < jmax; j++)
                {
                    SupportedTypeData typeData = field.components[j];
                    tempStrings.Add(typeData.type == null ? typeData.codeTypeName : typeData.type.Name);
                    if (!string.IsNullOrEmpty(typeData.nameSpace) && !usings.Contains(typeData.nameSpace))
                    {
                        usings.Add(typeData.nameSpace);
                    }
                }
                string objTypeName = string.Concat(string.Join("_", tempStrings.ToArray()), "_Container");
                if (!dataClasses.ContainsKey(objTypeName))
                {
                    dataClasses.Add(objTypeName, field.components.ToArray());
                }
                tempStrings.Clear();
                code.AppendLine(string.Format("{0}\t[SerializeField]", codeIndent));
                if (cls.publicProperty)
                {
                    code.AppendLine(string.Format("{0}\tprivate {1} m_{2};", codeIndent, objTypeName, field.name));
                    code.AppendLine(string.Format("{0}\tpublic {1} {2} {{ get {{ return m_{2}; }} }}",
                        codeIndent, objTypeName, field.name));
                }
                else
                {
                    code.AppendLine(string.Format("{0}\tprivate {1} {2};", codeIndent, objTypeName, field.name));
                }
                code.AppendLine();
                if (!string.IsNullOrEmpty(field.itemType) && !string.IsNullOrEmpty(field.itemVar) && !itemClasses.ContainsKey(objTypeName))
                {
                    itemClasses.Add(objTypeName, new KeyValuePair<string, string>(field.itemType, field.itemVar));
                }
            }


            foreach (KeyValuePair<string, SupportedTypeData[]> kv in dataClasses)
            {
                bool isGen = itemClasses.ContainsKey(kv.Key) && !GenType.types.Contains(kv.Key);
                if (!isGen) continue;

                code.AppendLine(string.Format("{0}\t[System.Serializable]", codeIndent));
                code.AppendLine(string.Format("{0}\t{1} class {2} {{",
                    codeIndent, cls.publicProperty ? "public" : "private", kv.Key));
                code.AppendLine();
                code.AppendLine(string.Format("{0}\t\t[SerializeField]", codeIndent));
                code.AppendLine(string.Format("{0}\t\tprivate GameObject m_GameObject;", codeIndent));
                code.AppendLine(string.Format("{0}\t\tpublic GameObject gameObject {{ get {{ return m_GameObject; }} }}", codeIndent));
                code.AppendLine();
                for (int i = 0, imax = kv.Value.Length; i < imax; i++)
                {
                    SupportedTypeData typeData = kv.Value[i];
                    code.AppendLine(string.Format("{0}\t\t[SerializeField]", codeIndent));
                    code.AppendLine(string.Format("{0}\t\tprivate {1} m_{2};",
                        codeIndent, typeData.codeTypeName, typeData.variableName));
                    code.AppendLine(string.Format("{0}\t\tpublic {1} {2} {{ get {{ return m_{2}; }} }}",
                        codeIndent, typeData.codeTypeName, typeData.variableName));
                    code.AppendLine();
                }
                KeyValuePair<string, string> typeAndVar;
                if (itemClasses.TryGetValue(kv.Key, out typeAndVar))
                {
                    bool isAutoList = nsCache.ContainsKey(typeAndVar.Key) && nsCache[typeAndVar.Key] == "DYBaseResUI";
                    if (isAutoList)
                    {
                        code.AppendLine(string.Format("{0}\t\t[System.NonSerialized] public List<{1}> mCachedList = new List<{1}>();", codeIndent, typeAndVar.Key));
                    }
                    code.AppendLine(string.Format("{0}\t\tprivate Queue<{1}> mCachedInstances;", codeIndent, typeAndVar.Key));
                    code.AppendLine(string.Format("{0}\t\tpublic {1} GetInstance(bool ignoreSibling = false) {{", codeIndent, typeAndVar.Key));
                    code.AppendLine(string.Format("{0}\t\t\t{1} instance = null;", codeIndent, typeAndVar.Key));
                    code.AppendLine(string.Format("{0}\t\t\tif (mCachedInstances != null) {{", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t\twhile ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {{", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t\t\tinstance = mCachedInstances.Dequeue();", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t\t}}", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t}}", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tif (instance == null || instance.Equals(null)) {{", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t\tinstance = Instantiate<{1}>(m_{2});", codeIndent, typeAndVar.Key, typeAndVar.Value));
                    if (isAutoList)
                    {
                        code.AppendLine(string.Format("{0}\t\t\t\tinstance.InternalInit();", codeIndent));
                    }
                    code.AppendLine(string.Format("{0}\t\t\t}}", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tTransform t0 = m_{1}.transform;", codeIndent, typeAndVar.Value));
                    code.AppendLine(string.Format("{0}\t\t\tTransform t1 = instance.transform;", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tt1.SetParent(t0.parent);", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tt1.localPosition = t0.localPosition;", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tt1.localRotation = t0.localRotation;", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tt1.localScale = t0.localScale;", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tif (!ignoreSibling) {{", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t\tt1.SetSiblingIndex(t0.GetSiblingIndex() + 1);", codeIndent)); // node.rectTransform().SetAsLastSibling();
                    code.AppendLine(string.Format("{0}\t\t\t}}else{{", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t\tt1.SetAsLastSibling();", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t}}", codeIndent));
                    if (isAutoList)
                    {
                        code.AppendLine(string.Format("{0}\t\t\tinstance.gameObject.SetActive(true);", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\tmCachedList.Add(instance);", codeIndent));
                    }

                    code.AppendLine(string.Format("{0}\t\t\treturn instance;", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t}}", codeIndent));
                    code.AppendLine(string.Format("{0}\t\tpublic bool CacheInstance({1} instance) {{", codeIndent, typeAndVar.Key));
                    code.AppendLine(string.Format("{0}\t\t\tif (instance == null || instance.Equals(null)) {{ return false; }}", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tif (mCachedInstances == null) {{ mCachedInstances = new Queue<{1}>(); }}", codeIndent, typeAndVar.Key));
                    code.AppendLine(string.Format("{0}\t\t\tif (mCachedInstances.Contains(instance)) {{ return false; }}", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tinstance.gameObject.SetActive(false);", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tmCachedInstances.Enqueue(instance);", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\treturn true;", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t}}", codeIndent));
                    code.AppendLine();

                    code.AppendLine(string.Format("{0}\t\tpublic void CacheInstanceList(List<{1}> instanceList) {{", codeIndent, typeAndVar.Key));
                    code.AppendLine(string.Format("{0}\t\t\tgameObject.SetActive(false);", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tforeach (var instance in instanceList){{", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t\tCacheInstance(instance);", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\t}}", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t\tinstanceList.Clear();", codeIndent));
                    code.AppendLine(string.Format("{0}\t\t}}", codeIndent));
                    code.AppendLine();

                    if (isAutoList)
                    {
                        code.AppendLine(string.Format("{0}\t\tpublic void CacheInstanceList() {{", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\tgameObject.SetActive(false);", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\tforeach (var instance in mCachedList){{", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\t\tCacheInstance(instance);", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\t}}", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\tmCachedList.Clear();", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t}}", codeIndent));
                        code.AppendLine();

                        code.AppendLine(string.Format("{0}\t\tpublic void ReleaseList() {{", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\tforeach (var instance in mCachedList){{", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\t\tif (instance != null){{", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\t\t\tinstance.InternalRelease();", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\t\t\tDestroy(instance.gameObject);", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\t\t}}", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\t}}", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t\tmCachedList.Clear();", codeIndent));
                        code.AppendLine(string.Format("{0}\t\t}}", codeIndent));
                        code.AppendLine();
                    }



                    if (!usings.Contains("System.Collections.Generic")) { usings.Add("System.Collections.Generic"); }
                }
                code.AppendLine(string.Format("{0}\t}}", codeIndent));
                code.AppendLine();
            }

            bool _isAutoList = true;
            foreach (KeyValuePair<string, SupportedTypeData[]> kv in dataClasses)
            {
                KeyValuePair<string, string> typeAndVar;
                if (itemClasses.TryGetValue(kv.Key, out typeAndVar))
                {
                    if (nsCache.ContainsKey(typeAndVar.Key))
                    {
                        if (nsCache[typeAndVar.Key] != "DYBaseResUI")
                        {
                            _isAutoList = false;
                            break;
                        }
                    }
                }
            }
            if (_isAutoList && cls.baseClass == "DYBaseUI")
            {
                code.AppendLine(string.Format("{0}\tprotected override void OnReleaseList() {{", codeIndent));
                foreach (KeyValuePair<string, SupportedTypeData[]> kv in dataClasses)
                {
                    KeyValuePair<string, string> typeAndVar;
                    if (itemClasses.TryGetValue(kv.Key, out typeAndVar))
                    {
                        code.AppendLine(string.Format("{0}\t\t{1}.ReleaseList();", codeIndent, typeAndVar.Value));
                    }
                }

                code.AppendLine(string.Format("{0}\t}}", codeIndent));
                code.AppendLine();
            }

            code.AppendLine(string.Format("{0}}}", codeIndent));

            if (!string.IsNullOrEmpty(ns))
            {
                code.AppendLine();
                code.AppendLine("}");
            }
            if (!usings.Contains("UnityEngine")) { usings.Add("UnityEngine"); }
            if (!string.IsNullOrEmpty(ns)) { usings.Remove(ns); }
            usings.Sort();
            StringBuilder codeUsings = new StringBuilder();
            for (int i = 0, imax = usings.Count; i < imax; i++)
            {
                codeUsings.AppendLine(string.Format("using {0};", usings[i]));
            }
            codeUsings.AppendLine();
            return codeUsings.ToString() + code.ToString();
        }


        private class CodeObject
        {
            public readonly string filename;
            public readonly string code;
            public readonly GUIContent codeContent;
            public CodeObject(string filename, string code)
            {
                this.filename = filename;
                this.code = code;
                codeContent = new GUIContent(code);
            }
        }

        private class CodePreviewWindow : EditorWindow
        {
            public List<CodeObject> codes;
            private Vector2[] mScrolls;
            private bool mStyleInited = false;
            private GUIStyle mStyleBox;
            private GUIStyle mStyleMessage;
            void OnGUI()
            {
                if (!mStyleInited)
                {
                    mStyleInited = true;
                    mStyleBox = "CN Box";
                    mStyleMessage = "CN Message";
                }
                if (codes != null)
                {
                    if (mScrolls == null || mScrolls.Length != codes.Count)
                    {
                        mScrolls = new Vector2[codes.Count];
                    }
                    for (int i = 0, imax = codes.Count; i < imax; i++)
                    {
                        CodeObject code = codes[i];
                        EditorGUILayout.LabelField(code.filename);
                        GUILayout.Space(2f);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(4f);
                        EditorGUILayout.BeginVertical(mStyleBox);
                        mScrolls[i] = EditorGUILayout.BeginScrollView(mScrolls[i], false, false);
                        Rect rect = GUILayoutUtility.GetRect(code.codeContent, mStyleBox);
                        EditorGUI.SelectableLabel(rect, code.code, mStyleMessage);
                        EditorGUILayout.EndScrollView();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
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
    public class GenType
    {
        public static HashSet<string> types = new HashSet<string>()
        {
            "RectTransform_Button_Image_Container",
            "RectTransform_Image_Container",
            "RectTransform_Container",
            "RectTransform_Text_Container",
        };
    }
}