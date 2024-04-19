using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
namespace Game
{
    public class Engine : MonoBehaviour
    {

        public static Engine Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            DateTime now = DateTime.Now;
            UnityEngine.Random.InitState(now.Second);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            EngineMgr.Instance.Init();
        }

        private void Update()
        {
            float dtTime = Time.deltaTime;
            EngineMgr.Instance.Update(dtTime);
        }

        private void OnDestroy()
        {
            EngineMgr.Instance.Destroy();
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
            EngineMgr.Instance.ApplicationFocus(focus);
        }
    }

}
