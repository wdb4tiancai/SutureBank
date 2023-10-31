﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace UIFramework.Editor
{
    internal class SupportedTypeDataMgr
    {
        [SupportedComponentType]
        static SupportedTypeData DefineTypeTransform()
        {
            return new SupportedTypeData(typeof(Transform), int.MinValue, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeRectTransform()
        {
            return new SupportedTypeData(typeof(RectTransform), int.MinValue, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeText()
        {
            return new SupportedTypeData(typeof(UnityEngine.UI.Text), 100, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeButton()
        {
            return new SupportedTypeData(typeof(UnityEngine.UI.Button), 101, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeToggle()
        {
            return new SupportedTypeData(typeof(UnityEngine.UI.Toggle), 101, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeSlider()
        {
            return new SupportedTypeData(typeof(UnityEngine.UI.Slider), 101, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeScrollbar()
        {
            return new SupportedTypeData(typeof(UnityEngine.UI.Scrollbar), 101, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeInputField()
        {
            return new SupportedTypeData(typeof(UnityEngine.UI.InputField), 101, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeImage()
        {
            return new SupportedTypeData(typeof(UnityEngine.UI.Image), 102, null, null, null, null);
        }
        [SupportedComponentType]
        static SupportedTypeData DefineTypeRawImage()
        {
            return new SupportedTypeData(typeof(UnityEngine.UI.RawImage), 102, null, null, null, null);
        }

        internal static Dictionary<int, SupportedTypeData> SupportedTypeDatas;

        internal static SupportedTypeData GetSupportedTypeData(Type type)
        {
            if (type == null) { return null; }
            if (SupportedTypeDatas == null) { SupportedTypeDatas = new Dictionary<int, SupportedTypeData>(); }
            if (SupportedTypeDatas.Count <= 0)
            {
                Type tComponent = typeof(Component);
                Type attr = typeof(SupportedComponentTypeAttribute);
                Type dele = typeof(DefineSupportedTypeDelegate);
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0, imax = assemblies.Length; i < imax; i++)
                {
                    Type[] types = assemblies[i].GetTypes();
                    for (int j = 0, jmax = types.Length; j < jmax; j++)
                    {
                        Type tt = types[j];
                        MethodInfo[] methods = tt.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        for (int k = 0, kmax = methods.Length; k < kmax; k++)
                        {
                            MethodInfo method = methods[k];
                            if (!Attribute.IsDefined(method, attr)) { continue; }
                            DefineSupportedTypeDelegate func = Delegate.CreateDelegate(dele, method, false) as DefineSupportedTypeDelegate;
                            if (func == null)
                            {
                                Debug.LogErrorFormat("Method '{0}.{1}' with 'SupportedComponentType' should match 'DefineSupportedTypeDelegate' !", tt.FullName, method.Name);
                                continue;
                            }
                            SupportedTypeData td = null;
                            try { td = func(); } catch (Exception e) { Debug.LogException(e); }
                            if (td == null || td.type == null) { continue; }
                            if (!td.type.IsSubclassOf(tComponent))
                            {
                                Debug.LogErrorFormat("Type should be sub class of 'Component' ! Error type : '{0}' .", td.type.FullName);
                                continue;
                            }
                            string showName = td.showName;
                            if (string.IsNullOrEmpty(showName)) { showName = td.type.Name; }
                            string codeTypeName = td.codeTypeName;
                            string nameSpace = td.nameSpace;
                            if (string.IsNullOrEmpty(codeTypeName))
                            {
                                codeTypeName = td.type.Name;
                                nameSpace = td.type.Namespace;
                            }
                            string variableName = td.variableName;
                            if (string.IsNullOrEmpty(variableName))
                            {
                                variableName = td.type.Name;
                                variableName = variableName.Substring(0, 1).ToLower() + variableName.Substring(1);
                            }
                            td = new SupportedTypeData(td.type, td.priority, showName, nameSpace, codeTypeName, variableName);
                            SupportedTypeDatas.Add(td.type.GetHashCode(), td);
                        }
                    }
                }
            }
            SupportedTypeData data;
            return SupportedTypeDatas.TryGetValue(type.GetHashCode(), out data) ? data : null;
        }

        internal static int SortSupportedTypeDatas(SupportedTypeData l, SupportedTypeData r)
        {
            if (l.priority == r.priority) { return string.Compare(l.type.Name, r.type.Name); }
            return l.priority < r.priority ? -1 : 1;
        }
    }
}