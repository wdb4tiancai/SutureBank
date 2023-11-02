
namespace Game.Util
{
    public abstract class SingletonBase<T> where T : SingletonBase<T>, new()
    {
        private static T instance;

        protected SingletonBase()
        {
        }
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}

