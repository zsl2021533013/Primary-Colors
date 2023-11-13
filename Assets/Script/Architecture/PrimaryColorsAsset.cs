namespace Script.Architecture
{
    public class PrimaryColorsAsset
    {
        #region Gravity

        public const float Gravity = 9.8f;
        public const float GravityScale = 4f;
        public const float MaxFallSpeed = 14f;
        public const float SpikeSensorDelay = 2f;
        public const float TargetSensorDelay = 2f;
        public const float CollectibleSensorDelay = 2f;
        public const float JumpGravityFactor = 1f;
        public const float FallGravityFactor = 1.4f;

        #endregion

        #region Player

        public const float PlayerSpawnOffset = 0.5f;
        public const float ColorChangeDuration = 0.5f;

        #endregion

        #region UI

        public const float UIFadeDuration = 1f;
        public const int BackgroundAccount = 5;
        public const float UICharDuration = 0.2f;
        public const float UISceneEnterTextKeepTime = 1f;
        public const float UISceneEnterTextFadeDuration = 0.5f;

        #endregion

        #region Object

        public const float TileResetTime = 8f;
        public const float CollectibleChasingDuration = 0.1f;
        public const float CollectibleResetDuration = 1f;
        

        #endregion
    }
}