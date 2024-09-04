namespace GameMain.Scripts.Utility
{
    public static class PathManager
    {
        public static string GetSceneAsset(string assetName)
        {
            return $"Assets/Resources/Scenes/{assetName}.unity";
        }
        
        public static string GetLevelAsset(string assetName)
        {
            return $"Assets/Resources/Scenes/Levels/{assetName}.unity";
        }
        
        public static string GetCharacterAsset(string assetName)
        {
            return $"Character/{assetName}";
        } 
        
        public static string GetParticleAsset(string assetName)
        {
            return $"Particles/{assetName}";
        } 

        public static string GetUIAsset(string assetName)
        {
            return $"UI/{assetName}";
        }
        
        public static string GetDataAsset(string assetName)
        {
            return $"Data/{assetName}";
        }
    }
}