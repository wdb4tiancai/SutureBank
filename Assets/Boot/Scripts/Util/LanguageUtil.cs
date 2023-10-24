using System;
using System.Collections.Generic;
using UnityEngine;

public enum LanguageCfgEnum
{
    None,
}

public class LanguageCfg
{
    public Dictionary<string, string> LanguageMap = new Dictionary<string, string>();
}

public class LanguageUtil : MonoBehaviour
{
    public static Dictionary<LanguageCfgEnum, LanguageCfg> LanguageCfg = new Dictionary<LanguageCfgEnum, LanguageCfg>()
    {

    };
}
