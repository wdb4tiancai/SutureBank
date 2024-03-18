using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Data
{
    public class ExternalTypeUtil
    {

        public static Game.Util.AntiCheat.JInt NewJInt(jint jint)
        {
            return new Game.Util.AntiCheat.JInt(jint.Value);
        }
    }
}