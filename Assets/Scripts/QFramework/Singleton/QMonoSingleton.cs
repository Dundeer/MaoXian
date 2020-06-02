
using UnityEngine;

namespace QFramework
{
    public class QMonoSingleton<T> : MonoBehaviour where T : QMonoSingleton<T>
    {
        protected static T instance = null;

        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if(FindObjectsOfType<T>().Length > 1)
                    {
                        print("More than 1!");
                        return instance;
                    }

                    if(instance == null)
                    {
                        string instanceName = typeof(T).Name;

                        print("Instance Name: " + instanceName);
                        GameObject instanceGO = GameObject.Find(instanceName);

                        if (instanceGO == null)
                            instanceGO = new GameObject(instanceName);
                        instance = instanceGO.AddComponent<T>();
                        DontDestroyOnLoad(instanceGO); //保证实例不会被释放
                        print("Add New Singleton " + instance.name + " in Game!");
                    }
                    else
                    {
                        print("Already exist: " + instance.name);
                    }
                }

                return instance;
            }
        }

        void OnDestroy()
        {
            instance = null;
        }
    }
}

