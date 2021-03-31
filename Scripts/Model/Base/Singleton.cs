using System;
using UnityEngine;

namespace ETModel
{
    public class Singleton<T>
    {
        private static T _instance;

        protected Singleton()
        {
            Init();
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Activator.CreateInstance<T>();
                }
                return _instance;
            }
        }
        protected virtual void Init() { }
    }


    abstract public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static bool _bIsDestroy = false;
        private static T _instance = null;
        private void Awake()
        {
            if (_instance == null && !_bIsDestroy)
            {
                _instance = this.gameObject.GetComponent<T>() as T;
                _bIsDestroy = false;
            }
            PreRegister();
        }

        public static T Instance
        {
            get
            {
                if (_instance == null && !_bIsDestroy)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null && !_bIsDestroy)
                    {
                        GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                        _instance = obj.GetComponent<T>() as T;
                    }
                    _bIsDestroy = false;
                }
                return _instance;
            }
        }

        protected virtual void PreRegister() { }
        protected virtual void OnDestroy()
        {
            StopAllCoroutines();
        }
        protected virtual void OnApplicationQuit()
        {
            _bIsDestroy = true;
        }
        public virtual void InstantiateMonoSingleton(params object[] args) { }
    }
}