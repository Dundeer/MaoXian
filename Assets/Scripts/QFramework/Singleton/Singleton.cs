
namespace QFramework
{
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        protected static T mInstance;

        private static object mLock = new object();

        protected Singleton()
        {

        }

        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = SingletonCreator.CreateSingleton<T>();
                    }
                }

                return mInstance;
            }
        }

        public virtual void OnSingletonInit()
        {

        }

        public virtual void Dispose()
        {
            mInstance = null;
        }
    }
}

