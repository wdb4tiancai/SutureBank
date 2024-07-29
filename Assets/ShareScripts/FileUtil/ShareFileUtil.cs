namespace SharePublic
{
    public class ShareFileUtil
    {
        private static FileUtilBase instance;

        public static FileUtilBase Instance
        {
            get
            {
                if (instance == null)
                {
#if GAME_PLATFORM_EDITOR || GAME_PLATFORM_ANDROID || GAME_PLATFORM_IOS
                    instance = new FileUtilApp();
#endif
                }
                return instance;
            }
        }
    }
}



