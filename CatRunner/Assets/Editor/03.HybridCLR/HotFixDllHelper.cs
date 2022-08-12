using UnityEditor;
using System.IO;

namespace EPloy.Editor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class HotFixDllHelper
    {
        [MenuItem("EPloy/Hotfix/更新HotfixDll", false, 10)]
        static void UpdateHotfixDll()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                //复制dll pdb
                File.Copy(EPloyEditorPath.EditorHotfixDLL, EPloyEditorPath.AssetHotfixDLL, true);
                //刷新资源
                AssetDatabase.Refresh();

                Log.Info("更新HotFix dll!");
            }
        }
    }
}
