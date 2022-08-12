
namespace EPloy.Game
{
    public static class UtilAsset
    {
        public static string GetConfigAsset(string assetName )
        {
            return UtilText.Format("Assets/Res/Configs/{0}.bytes", assetName);
        }

        public static string GetDataTableAsset(string assetName)
        {
            return UtilText.Format("Assets/Res/DataTables/Binary/{0}.bytes", assetName);
        }

        public static string GetFontAsset(string assetName)
        {
            return UtilText.Format("Assets/Res/Fonts/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return UtilText.Format("Assets/Res/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return UtilText.Format("Assets/Res/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return UtilText.Format("Assets/Res/Sounds/{0}.wav", assetName);
        }

        public static string GetObjAsset(string assetName)
        {
            return UtilText.Format("Assets/Res/Obj/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return UtilText.Format("Assets/Res/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return UtilText.Format("Assets/Res/UI/UISounds/{0}.wav", assetName);
        }
    }
}
