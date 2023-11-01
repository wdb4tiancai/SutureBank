using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
namespace UIFramework.Editor
{
    public class UiTool
    {

        internal static MD5CryptoServiceProvider Md5Calc;
        internal static string GetMD5Key(string key)
        {

            string str = string.Concat("SerializedComponent4CSharp", key);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            return BitConverter.ToString(GetMd5Calc().ComputeHash(bytes));
        }

        internal static MD5CryptoServiceProvider GetMd5Calc()
        {
            if (Md5Calc == null)
            {
                Md5Calc = new MD5CryptoServiceProvider();
            }
            return Md5Calc;
        }


    }
}
