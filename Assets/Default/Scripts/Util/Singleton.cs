using UnityEngine;
using UnityEditor;


namespace Default.Scripts.Util
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = (T)FindObjectOfType<T>();
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.LogError(e.StackTrace);
                        return null;
                    }
                }

                return _instance;
            }
        }

        private static T _instance;
    }

    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        var type = typeof(T).ToString();
                        var guid =AssetDatabase.FindAssets("t:"+ type);
                        var path = AssetDatabase.GUIDToAssetPath(guid[0]);
                        _instance = (T)AssetDatabase.LoadAssetAtPath(path,typeof(T));
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.LogError(e.StackTrace);
                        return null;
                    }
                }

                return _instance;
            }
        }

        private static T _instance;
    }
}
