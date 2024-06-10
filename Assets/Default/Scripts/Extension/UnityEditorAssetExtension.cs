using UnityEditor;
using UnityEngine;

namespace Default.Scripts.Extension
{
    public static class UnityEditorAssetExtensions
    {
        /// <summary> 폴더 애셋으로부터 Assets로 시작하는 로컬 경로 얻기 </summary>
        public static string GetLocalPath(this UnityEditor.DefaultAsset @this)
        {
            bool success =
                UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(@this, out string guid, out long _);

            if (success)
                return UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            else
                return null;
        }

        /// <summary> 폴더 애셋으로부터 절대 경로 얻기 </summary>
        public static string GetAbsolutePath(this UnityEditor.DefaultAsset @this)
        {
            string path = GetLocalPath(@this);
            if (path == null)
                return null;

            path = path.Substring(path.IndexOf('/') + 1);
            return Application.dataPath + "/" + path;
        }

        /// <summary> 폴더 애셋으로부터 DirectoryInfo 객체 얻기 </summary>
        public static System.IO.DirectoryInfo GetDirectoryInfo(this DefaultAsset @this)
        {
            string absPath = GetAbsolutePath(@this);
            return (absPath != null) ? new System.IO.DirectoryInfo(absPath) : null;
        }
    }
}
