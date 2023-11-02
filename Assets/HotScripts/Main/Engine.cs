using System;
using UnityEngine;
namespace Game
{
    public class Engine : MonoBehaviour
    {
        public static EngineMgr EngineMgr = new EngineMgr();
        private void Awake()
        {
            DateTime now = DateTime.Now;
            UnityEngine.Random.InitState(now.Second);
            DontDestroyOnLoad(gameObject);
            _ = EngineMgr.Init(this);
        }
        private void Update()
        {
            float dtTime = Time.deltaTime;
            EngineMgr.Update(dtTime);
        }

        private void OnDestroy()
        {
            EngineMgr.Destroy();
#if (UNITY_EDITOR)
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
#endif
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }


        private void OnApplicationFocus(bool focus)
        {
            EngineMgr.ApplicationFocus(focus);
        }
    }

}
