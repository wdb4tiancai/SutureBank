#if GAME_PLATFORM_WEIXIN
using WeChatWASM;

namespace SharePublic
{
    public class DYPlayerPrefsWX : DYPlayerPrefsBase
    {
        public override void SetInt(string key, int value)
        {
            WX.StorageSetIntSync(key, value);
        }
        public override int GetInt(string key, int defaultValue = 0)
        {
            return WX.StorageGetIntSync(key, defaultValue);
        }
        public override void SetString(string key, string value)
        {
            WX.StorageSetStringSync(key, value);
        }
        public override string GetString(string key, string defaultValue = "")
        {
            return WX.StorageGetStringSync(key, defaultValue);
        }
        public override void DeleteKey(string key)
        {
            try
            {
                WX.StorageDeleteKeySync(key);
            }
            catch (System.Exception)
            {
            }
        }
        public override void DeleteAll()
        {

        }
    }
}
#endif