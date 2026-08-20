namespace RPGGame.Presentation
{
    public enum TextSpeed { Instant, Fast, Normal, Slow }
    public enum DifficultyLevel { Easy, Normal, Hard }

    public static class GameSettings
    {
        public static TextSpeed Speed { get; set; } = TextSpeed.Normal;
        public static DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Normal;

        public static double SpeedFactor => Speed switch
        {
            TextSpeed.Instant => 0.0,
            TextSpeed.Fast    => 0.33,
            TextSpeed.Slow    => 2.0,
            _                 => 1.0
        };

        public static double EnemyDamageMultiplier => Difficulty switch
        {
            DifficultyLevel.Easy => 0.75,
            DifficultyLevel.Hard => 1.4,
            _                    => 1.0
        };

        public static double EXPMultiplier => Difficulty switch
        {
            DifficultyLevel.Easy => 1.2,
            DifficultyLevel.Hard => 0.85,
            _                    => 1.0
        };
    }
}
