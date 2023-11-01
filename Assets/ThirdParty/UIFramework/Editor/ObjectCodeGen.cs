using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UIFramework.Editor
{
    internal class GenType
    {
        public static HashSet<string> types = new HashSet<string>()
        {
            "RectTransform_Button_Image_Container",
            "RectTransform_Image_Container",
            "RectTransform_Container",
            "RectTransform_Text_Container",
        };
    }
    internal class CodeObject
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
    internal class ClassData
    {
        public List<string> usings = new List<string>();
        public string cls;
        public string baseClass;
        public bool partialClass;
        public bool publicProperty;
        public List<FieldData> fields = new List<FieldData>();
    }

    internal class FieldData
    {
        public string name;
        public string itemType;
        public string itemVar;
        public List<SupportedTypeData> components = new List<SupportedTypeData>();
    }

    public class ObjectCodeGen
    {

        static private Dictionary<string, string> nsCache = new Dictionary<string, string>();

        internal static List<CodeObject> GetCodes(ObjectComponents rootComponents, string ns)
        {
            nsCache.Clear();
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
        internal static string GetCode(string ns, ClassData cls)
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
            if (!usings.Contains("UIFramework")) { usings.Add("UIFramework"); }
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
    }
}

