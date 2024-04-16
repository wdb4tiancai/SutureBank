namespace SharePublic
{
    public abstract class DYPlayerPrefsBase
    {
        public abstract void SetInt(string key, int value);
        public abstract int GetInt(string key, int defaultValue = 0);
        public abstract void SetString(string key, string value);
        public abstract string GetString(string key, string defaultValue = "");
        public abstract void DeleteKey(string key);
        public abstract void DeleteAll();

    }
}
