using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UIFramework.Editor
{
    internal class GenType
    {
        /// <summary>
        /// 类成员的类型限定
        /// </summary>
        public static HashSet<string> types = new HashSet<string>()
        {
            "RectTransform_Button_Image_Container",
            "RectTransform_Image_Container",
            "RectTransform_Container",
            "RectTransform_Text_Container",
        };
    }

    //生成的代码类对象
    internal class CodeObject
    {
        /// <summary>
        /// 代码的文件名字
        /// </summary>
        public readonly string ClassFileName;
        /// <summary>
        /// 类的代码内容
        /// </summary>
        public readonly string ClassCode;

        public CodeObject(string filename, string code)
        {
            this.ClassFileName = filename;
            this.ClassCode = code;
        }
    }

    //需要生成的类数据信息
    internal class ClassData
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public List<string> Usings = new List<string>();

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName;

        /// <summary>
        /// 基类名字
        /// </summary>
        public string BaseClass;

        /// <summary>
        /// 是否是分部类
        /// </summary>
        public bool IsPartialClass;

        /// <summary>
        /// 是否公共属性
        /// </summary>
        public bool IsPublicProperty;

        /// <summary>
        /// 数据成员
        /// </summary>
        public List<FieldData> Fields = new List<FieldData>();
    }

    //类成员类
    internal class FieldData
    {
        //成员的名字
        public string Name;
        //子集对象的名字
        public string ItemType;
        //子集对象的值
        public string ItemVar;
        //成员的组件列表
        public List<SupportedTypeData> Components = new List<SupportedTypeData>();
    }

    public class ObjectCodeGen
    {

        static private Dictionary<string, string> m_NameSpaceCache = new Dictionary<string, string>();

        /// <summary>
        /// 生成代码文件
        /// </summary>
        /// <param name="rootComponents"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        internal static List<CodeObject> GenerateCodeFile(ObjectComponents rootComponents, string ns)
        {
            m_NameSpaceCache.Clear();
            List<CodeObject> codes = new List<CodeObject>();
            if (rootComponents == null) { return codes; }
            List<ClassData> clses = new List<ClassData>();
            Queue<ObjectComponents> components = new Queue<ObjectComponents>();
            components.Enqueue(rootComponents);
            while (components.Count > 0)
            {
                ObjectComponents ocs = components.Dequeue();
                List<ObjectComponents> itemComponents = ocs.ChildComponents;
                if (itemComponents == null) { continue; }
                ClassData cd = null;
                for (int i = clses.Count - 1; i >= 0; i--)
                {
                    if (clses[i].ClassName == ocs.ClassName)
                    {
                        cd = clses[i];
                        break;
                    }
                }
                if (cd == null)
                {
                    cd = new ClassData();
                    cd.ClassName = ocs.ClassName;
                    clses.Add(cd);
                }
                if (!GetClassDataByObjectComponents(ocs, cd))
                {
                    // TODO class not match...
                }
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
                m_NameSpaceCache.Add(clses[i].ClassName, clses[i].BaseClass);
            }

            for (int i = 0, imax = clses.Count; i < imax; i++)
            {
                ClassData cls = clses[i];
                for (int j = cls.Fields.Count - 1; j >= 0; j--)
                {
                    cls.Fields[j].Components.Sort(typeSorter);
                }
                string code = GenerateCodeFileByClassData(ns, clses[i]);
                if (!string.IsNullOrEmpty(code))
                {
                    codes.Add(new CodeObject(string.Concat(cls.ClassName, ".cs"), code));
                }
            }
            return codes;
        }

        /// <summary>
        /// 根据ObjectComponents 给 ClassData赋值
        /// </summary>
        /// <param name="ocs"></param>
        /// <param name="classData"></param>
        /// <returns></returns>
        private static bool GetClassDataByObjectComponents(ObjectComponents ocs, ClassData classData)
        {
            if (classData.ClassName != ocs.ClassName) { return false; }
            if (classData.BaseClass != null && classData.BaseClass != ocs.BaseClass) { return false; }
            classData.BaseClass = ocs.BaseClass;
            classData.IsPartialClass |= ocs.IsPartialClass;
            classData.IsPublicProperty |= ocs.IsPublicProperty;
            List<ObjectComponents> objComponents = ocs.ChildComponents;
            for (int i = 0, imax = objComponents.Count; i < imax; i++)
            {
                ObjectComponents oc = objComponents[i];
                FieldData field = null;
                for (int j = classData.Fields.Count - 1; j >= 0; j--)
                {
                    FieldData f = classData.Fields[j];
                    if (f.Name == oc.Name)
                    {
                        field = f;
                        break;
                    }
                }
                if (field == null)
                {
                    field = new FieldData();
                    field.Name = oc.Name;
                    field.ItemType = null;
                    classData.Fields.Add(field);
                }
                for (int j = 0, jmax = oc.Count; j < jmax; j++)
                {
                    SupportedTypeData type = oc[j].Type;
                    for (int k = field.Components.Count - 1; k >= 0; k--)
                    {
                        if (field.Components[k].ShowName == type.ShowName)
                        {
                            type = null;
                            break;
                        }
                    }
                    if (type != null) { field.Components.Add(type); }
                }
                if (oc.ChildComponents != null)
                {
                    string itemClass = oc.ClassName;
                    for (int k = field.Components.Count - 1; k >= 0; k--)
                    {
                        if (field.Components[k].ShowName == itemClass)
                        {
                            itemClass = null;
                            break;
                        }
                    }
                    if (itemClass != null)
                    {
                        field.Components.Add(new SupportedTypeData(null, 10000, oc.ClassName, null, oc.ClassName, oc.ClassVarName));
                        field.ItemType = oc.ClassName;
                        field.ItemVar = oc.ClassVarName;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 根据ClassData数据，生成对应的代码类。
        /// </summary>
        /// <param name="nameSpaceName"></param>
        /// <param name="classData"></param>
        /// <returns></returns>
        private static string GenerateCodeFileByClassData(string nameSpaceName, ClassData classData)
        {
            List<string> usings = new List<string>();
            SortedList<string, SupportedTypeData[]> dataClasses = new SortedList<string, SupportedTypeData[]>();
            StringBuilder code = new StringBuilder();
            Dictionary<string, KeyValuePair<string, string>> itemClasses = new Dictionary<string, KeyValuePair<string, string>>();
            string codeIndent = "";
            if (!string.IsNullOrEmpty(nameSpaceName))
            {
                codeIndent = "\t";
                code.AppendLine(string.Format("namespace {0} {{", nameSpaceName));
                code.AppendLine();
            }
            code.AppendLine(string.Format("{0}public {1}class {2} : {3} {{",
                codeIndent, classData.IsPartialClass ? "partial " : "", classData.ClassName, classData.BaseClass));
            code.AppendLine();
            List<string> tempStrings = new List<string>();
            for (int i = 0, imax = classData.Fields.Count; i < imax; i++)
            {
                FieldData field = classData.Fields[i];
                tempStrings.Clear();
                for (int j = 0, jmax = field.Components.Count; j < jmax; j++)
                {
                    SupportedTypeData typeData = field.Components[j];
                    tempStrings.Add(typeData.Type == null ? typeData.CodeTypeName : typeData.Type.Name);
                    if (!string.IsNullOrEmpty(typeData.NameSpace) && !usings.Contains(typeData.NameSpace))
                    {
                        usings.Add(typeData.NameSpace);
                    }
                }
                string objTypeName = string.Concat(string.Join("_", tempStrings.ToArray()), "_Container");
                if (!dataClasses.ContainsKey(objTypeName))
                {
                    dataClasses.Add(objTypeName, field.Components.ToArray());
                }
                tempStrings.Clear();
                code.AppendLine(string.Format("{0}\t[SerializeField]", codeIndent));
                if (classData.IsPublicProperty)
                {
                    code.AppendLine(string.Format("{0}\tprivate {1} m_{2};", codeIndent, objTypeName, field.Name));
                    code.AppendLine(string.Format("{0}\tpublic {1} {2} {{ get {{ return m_{2}; }} }}",
                        codeIndent, objTypeName, field.Name));
                }
                else
                {
                    code.AppendLine(string.Format("{0}\tprivate {1} {2};", codeIndent, objTypeName, field.Name));
                }
                code.AppendLine();
                if (!string.IsNullOrEmpty(field.ItemType) && !string.IsNullOrEmpty(field.ItemVar) && !itemClasses.ContainsKey(objTypeName))
                {
                    itemClasses.Add(objTypeName, new KeyValuePair<string, string>(field.ItemType, field.ItemVar));
                }
            }


            foreach (KeyValuePair<string, SupportedTypeData[]> kv in dataClasses)
            {
                bool isGen = itemClasses.ContainsKey(kv.Key) && !GenType.types.Contains(kv.Key);
                if (!isGen) continue;

                code.AppendLine(string.Format("{0}\t[System.Serializable]", codeIndent));
                code.AppendLine(string.Format("{0}\t{1} class {2} {{",
                    codeIndent, classData.IsPublicProperty ? "public" : "private", kv.Key));
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
                        codeIndent, typeData.CodeTypeName, typeData.VariableName));
                    code.AppendLine(string.Format("{0}\t\tpublic {1} {2} {{ get {{ return m_{2}; }} }}",
                        codeIndent, typeData.CodeTypeName, typeData.VariableName));
                    code.AppendLine();
                }
                KeyValuePair<string, string> typeAndVar;
                if (itemClasses.TryGetValue(kv.Key, out typeAndVar))
                {
                    bool isAutoList = m_NameSpaceCache.ContainsKey(typeAndVar.Key) && m_NameSpaceCache[typeAndVar.Key] == "DYBaseResUI";
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
                    if (m_NameSpaceCache.ContainsKey(typeAndVar.Key))
                    {
                        if (m_NameSpaceCache[typeAndVar.Key] != "DYBaseResUI")
                        {
                            _isAutoList = false;
                            break;
                        }
                    }
                }
            }
            if (_isAutoList && classData.BaseClass == "DYBaseUI")
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

            if (!string.IsNullOrEmpty(nameSpaceName))
            {
                code.AppendLine();
                code.AppendLine("}");
            }
            if (!usings.Contains("UnityEngine")) { usings.Add("UnityEngine"); }
            if (!usings.Contains("UIFramework")) { usings.Add("UIFramework"); }
            if (!string.IsNullOrEmpty(nameSpaceName)) { usings.Remove(nameSpaceName); }
            usings.Sort();
            StringBuilder codeUsings = new StringBuilder();
            for (int i = 0, imax = usings.Count; i < imax; i++)
            {
                codeUsings.AppendLine(string.Format("using {0};", usings[i]));
            }
            codeUsings.AppendLine();
            return codeUsings.ToString() + code.ToString();
        }
    }
}

