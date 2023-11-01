using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
namespace UIFramework.Editor
{
    internal class CodeProperties
    {
        /// <summary>
        /// 代码的命名空间
        /// </summary>
        public string NameSpace;
        /// <summary>
        /// 代码是否是分部类
        /// </summary>
        public bool IsPartialClass;
        /// <summary>
        /// 代码的类名
        /// </summary>
        public string ClassName;
        /// <summary>
        /// 代码的基类
        /// </summary>
        public string BaseClass;
        /// <summary>
        /// 代码是否使用public属性
        /// </summary>
        public bool IsPublicProperty;
    }

    internal class CodePropertiesMgr
    {
        static Regex m_RegNamespace = new Regex(@"namespace\s+((\S+\s*\.\s*)*\S+)\s*\{");
        static Regex m_RegClassDef = new Regex(@"public\s+(partial\s+){0,1}class\s+(\S+)\s*:\s*(\S+)");
        static Regex m_RegSubclassStart = new Regex(@"\[System\.Serializable\]");
        static Regex m_RegPublicProperty = new Regex(@"public\s+\S+\s+\S+\s*\{\s*get\s*\{\s*return\s+\S+\s*;\s*\}\s*\}");

        /// <summary>
        /// 通过代码所在目录，读取代码的基本信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns>CodeProperties</returns>
        internal static CodeProperties ReadCodeInformationByPath(string path)
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
            Match matchClassDefine = m_RegClassDef.Match(code);
            if (!matchClassDefine.Success) { return null; }
            int i0 = matchClassDefine.Index + matchClassDefine.Length;
            GroupCollection groups = matchClassDefine.Groups;
            CodeProperties cp = new CodeProperties();
            cp.IsPartialClass = groups[1].Success;
            cp.ClassName = groups[2].Value;
            cp.BaseClass = groups[3].Value;
            Match matchNS = m_RegNamespace.Match(code);
            string ns = matchNS.Success ? matchNS.Groups[1].Value : null;
            if (!string.IsNullOrEmpty(ns))
            {
                ns = ns.Replace(" ", "").Replace("\t", "");
            }
            cp.NameSpace = ns;
            Match matchSubclassStart = m_RegSubclassStart.Match(code, i0);
            int i1 = code.Length;
            if (matchSubclassStart.Success) { i1 = matchSubclassStart.Index; }
            cp.IsPublicProperty = m_RegPublicProperty.Match(code, i0, i1 - i0).Success;
            return cp;
        }
    }
}
