using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SharePublic
{
    public class ShareDebug
    {
        //是否DEBUG模式
        public static bool IsDebugOpen()
        {
#if UNITY_EDITOR
            return true;
#endif
            return false;
        }
    }
}
