using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace UIFramework.Editor
{
    /// <summary>
    /// 组件的数据结构
    /// </summary>
    internal struct ComponentData
    {
        /// <summary>
        /// 组件类型
        /// </summary>
        public SupportedTypeData Type;
        /// <summary>
        /// 组件对象
        /// </summary>
        public Component Component;
    }

    //组件的信息
    internal class ObjectComponents
    {

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type;

        /// <summary>
        /// 基类
        /// </summary>
        public string BaseClass = "MonoBehaviour";

        /// <summary>
        /// 基类的选择
        /// </summary>
        public int BaseClassIndex;

        /// <summary>
        /// 是否是分部类
        /// </summary>
        public bool IsPartialClass;

        /// <summary>
        /// 是否公共属性
        /// </summary>
        public bool IsPublicProperty;

        /// <summary>
        /// 节点对象
        /// </summary>
        public readonly GameObject Obj;

        /// <summary>
        /// 节点名字
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 类名
        /// </summary>
        public readonly string ClassName;

        /// <summary>
        /// 类的字段名
        /// </summary>
        public readonly string ClassVarName;

        /// <summary>
        /// 子集节点对象
        /// </summary>
        public readonly List<ObjectComponents> ChildComponents;

        /// <summary>
        /// 组件数量
        /// </summary>
        public int Count { get { return m_Components.Count; } }

        //获得指定组件的数据
        public ComponentData this[int i]
        {
            get { return m_Components[i]; }
        }

        /// <summary>
        /// 所有的组件数据
        /// </summary>
        private List<ComponentData> m_Components = new List<ComponentData>();


        /// <summary>
        /// 临时的组件对象
        /// </summary>
        private static List<Component> m_TempComponents = new List<Component>();

        public ObjectComponents(GameObject obj, string name, string className, string classVarName, List<ObjectComponents> childComponents)
        {
            Obj = obj;
            Name = name;
            ClassName = className;
            ClassVarName = classVarName;
            ChildComponents = childComponents;
            m_TempComponents.Clear();
            obj.GetComponents<Component>(m_TempComponents);
            for (int i = 0, imax = m_TempComponents.Count; i < imax; i++)
            {
                Component component = m_TempComponents[i];
                if (childComponents != null && !(component is Transform)) { continue; }
                SupportedTypeData std = SupportedTypeDataMgr.GetSupportedTypeData(component.GetType());
                if (std == null) { continue; }
                ComponentData data = new ComponentData();
                data.Type = std;
                data.Component = component;
                m_Components.Add(data);
            }
            m_TempComponents.Clear();
        }
    }

    //工具类用于收集所有的组件
    internal class ObjectComponentsTools
    {

        private static Regex m_VariableRegex = new Regex(@"^[_a-zA-Z][_0-9a-zA-Z]*$");
        /// <summary>
        /// 匹配有效的变量名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool MatchVariableName(string name)
        {
            if (string.IsNullOrEmpty(name)) { return false; }
            return m_VariableRegex.IsMatch(name);
        }

        //收集所有的组件
        public static ObjectComponents CollectComponents(Transform root, string rootName, string className, string classValName)
        {
            if (root == null) { return null; }
            List<ObjectComponents> components = new List<ObjectComponents>();
            Stack<Transform> trans = new Stack<Transform>(64);
            trans.Push(root);
            while (trans.Count > 0)
            {
                Transform t = trans.Pop();
                string name = t.name;
                bool isItem = t != root && name.StartsWith("i_");
                if (!isItem)
                {
                    for (int i = t.childCount - 1; i >= 0; i--)
                    {
                        trans.Push(t.GetChild(i));
                    }
                }
                if (t == root) { continue; }
                if (!isItem && !name.StartsWith("m_")) { continue; }
                name = name.Substring(2, name.Length - 2);
                string varName = name;
                string typeName = null;
                string typeVarName = null;
                if (isItem)
                {
                    typeName = string.Concat(className, "_", name);
                    typeVarName = name;
                    int ivv = name.IndexOf("||");
                    if (ivv >= 0)
                    {
                        varName = name.Substring(0, ivv);
                        typeName = name.Substring(ivv + 2, name.Length - ivv - 2);
                        typeVarName = typeName;
                    }
                    else
                    {
                        int iv = name.IndexOf('|');
                        if (iv >= 0)
                        {
                            varName = name.Substring(0, iv);
                            typeVarName = name.Substring(iv + 1, name.Length - iv - 1);
                            typeName = string.Concat(className, "_", typeVarName);
                        }
                    }
                    if (!MatchVariableName(typeName)) { continue; }
                }
                if (!MatchVariableName(varName)) { continue; }
                ObjectComponents ocs = null;
                if (isItem)
                {
                    ocs = CollectComponents(t, varName, typeName, typeVarName);
                }
                if (ocs == null)
                {
                    ocs = new ObjectComponents(t.gameObject, varName, null, null, null);
                }
                components.Add(ocs);
            }
            return new ObjectComponents(root.gameObject, rootName, className, classValName, components);
        }
    }

}