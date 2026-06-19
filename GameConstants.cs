namespace RPGGame
{
    public static class GameConstants
    {
        public const int MaxRage              = 100;
        public const int BerserkDuration      = 4;
        public const double BerserkMultiplier = 1.5;
        public const double DamageVarianceBase  = 0.88;
        public const double DamageVarianceRange = 0.24; // ±12%
        public const int DefaultBurnDamage    = 9;
        public const int DefaultBurnTurns     = 3;
        public const int AdvancedSkillLevel   = 3;
        public const int UltraSkillLevel      = 5;
        public const int HPGainPerLevel       = 20;
        public const int MPGainPerLevel       = 10;
        public const int ATKGainPerLevel      = 3;
        public const int DEFGainPerLevel      = 2;
        public const double EXPScaleRate      = 1.6;

        // Berserk backfire (auto-triggered berserk, normal attack)
        public const int BerserkBackfireChance        = 20;
        public const double BerserkBackfireDamageRatio = 0.35;

        // Rage skill backfire (manually consumed rage cost)
        public const int RageSkillBackfireChance        = 22;
        public const double RageSkillBackfireDamageRatio = 0.40;

        // Critical bonus extra hit multiplier
        public const double CritBonusMultiplier = 0.9;

        // Post-battle MP restoration
        public const int PostBattleMPRestore = 15;

        // Player name max length enforced at input
        public const int MaxPlayerNameLength = 16;

        // Bump when SaveData schema changes to detect stale saves
        public const int SaveVersion = 1;
    }
}
