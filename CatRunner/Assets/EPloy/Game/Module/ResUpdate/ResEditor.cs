using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class ResEditor
{
    public static UnityEngine.Object LoadAssetAtPath(string AssetName, Type type)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath(AssetName, type);
#else
            return null;
#endif
    }

    public static UnityEngine.Object LoadMainAssetAtPath(string AssetName)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadMainAssetAtPath(AssetName);
#else
            return null;
#endif
    }

    public static int HasAsset(string assetName)
    {
#if UNITY_EDITOR
        UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetName);
        if (obj == null) return 0;
        int result = obj.GetType() == typeof(UnityEditor.DefaultAsset) ? 3 : 2;
        obj = null;
        UnityEditor.EditorUtility.UnloadUnusedAssetsImmediate();
        return result;
#else
            return 0;
#endif

    }
}

