using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CREMOT.GameplayUtilities
{
    public class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindAnyObjectByType<T>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Warning : No init verification. Call this after awake to be sure that the singleton has been set.
        /// </summary>
        public static T UnsafeInstance => _instance;

        public static bool Exist => _instance != null;

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
        }
    }
}
