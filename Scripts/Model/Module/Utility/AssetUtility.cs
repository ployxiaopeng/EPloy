using GameFramework;

namespace ETModel
{
    public static class AssetUtility
    {
        public static string GetConfigAsset(string assetName, LoadType loadType)
        {
            return Utility.Text.Format("Assets/Res/Configs/{0}.{1}", assetName, loadType == LoadType.Text ? "txt" : "bytes");
        }
         
        public static string GetConfigAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Configs/{0}", assetName);
        }

        public static string GetDataTableAsset(string assetName, LoadType loadType)
        {
            return Utility.Text.Format("Assets/Res/DataTables/Binary/{0}.{1}", assetName, loadType == LoadType.Text ? "txt" : "bytes");
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Entities/{0}.prefab", assetName);
        }

        public static string GetUIWndAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UIWnd/{0}.prefab", assetName);
        }

        public static string GetUICommonAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/{0}.prefab", assetName);
        }

        public static string GetHotfixDLLAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/HotfixDLL/{0}.bytes", assetName);
        }
    }
}
