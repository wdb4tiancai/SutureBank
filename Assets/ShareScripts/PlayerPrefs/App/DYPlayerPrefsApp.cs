using UnityEngine;
#if GAME_PLATFORM_EDITOR || GAME_PLATFORM_ANDROID || GAME_PLATFORM_IOS
namespace SharePublic
{
    public class DYPlayerPrefsApp : DYPlayerPrefsBase
    {
        public override void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        public override int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public override void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        public override string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        public override void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        public override void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
#endif