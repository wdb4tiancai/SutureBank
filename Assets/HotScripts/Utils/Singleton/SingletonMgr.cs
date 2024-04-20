
using Cysharp.Threading.Tasks;

namespace Game.Util
{
    public abstract class SingletonMgrBase<T> where T : SingletonMgrBase<T>, new()
    {
        private static T instance;

        protected SingletonMgrBase()
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

        public abstract void Init();
        public abstract void Reset();
        public abstract void Destroy();

        public abstract void Update(float dt);


    }
}

