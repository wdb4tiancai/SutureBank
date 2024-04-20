
namespace SharePublic
{
    public class SharePlayerPrefs
    {

        private static DYPlayerPrefsBase PlayerPrefs;
        static SharePlayerPrefs()
        {
#if GAME_PLATFORM_EDITOR || GAME_PLATFORM_ANDROID || GAME_PLATFORM_IOS
            PlayerPrefs = new DYPlayerPrefsApp();
#elif GAME_PLATFORM_WEIXIN
            PlayerPrefs = new DYPlayerPrefsWX();
#endif
        }
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

    }
}
