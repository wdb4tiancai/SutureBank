using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UIFrameWork.Editor
{
    public class ObjectSerialize
    {
        /// <summary>
        /// 序列号ui预制上的代码
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="ocs"></param>
        internal static void SerializeObject(string ns, ObjectComponents ocs)
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

                string typeFullName = string.IsNullOrEmpty(ns) ? oc.ClassName : string.Concat(ns, ".", oc.ClassName);

                Type type = Type.GetType(typeFullName + ",HotScripts");

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
                            //AntiCheatHelper.dt(ioc.name, cd.type.codeTypeName + " " + cd.type.variableName);
                            SerializedProperty pComponent = pObj.FindPropertyRelative("m_" + cd.Type.VariableName);
                            if (pComponent == null)
                            {
                                Debug.LogErrorFormat(cd.Component, "Cannot Find property for Component '{0}' at '{1}' !",
                                    cd.Type.VariableName, ioc.Obj.name);
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

    }
}
