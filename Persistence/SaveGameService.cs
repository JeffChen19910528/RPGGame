using System.IO;
using System.Text.Json;
using RPGGame.Domain;

namespace RPGGame.Persistence
{
    public class SaveData
    {
        public int Version { get; set; } = GameConstants.SaveVersion;
        public string PlayerName { get; set; } = "";
        public int Level { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int EXP { get; set; }
        public int EXPToNextLevel { get; set; }
        public int CorruptionLevel { get; set; }
        public bool AcceptedDark { get; set; }
        public bool HelpedVillager { get; set; }
        public int BerserkUses { get; set; }
        public int ClassId { get; set; }
        public int CurrentChapter { get; set; }
    }

    /// <summary>Owns the save file: reading/writing JSON and rebuilding a Player from it.</summary>
    public class SaveGameService
    {
        private const string SaveFile = "savegame.json";

        public bool HasSave() => File.Exists(SaveFile);

        public void Delete()
        {
            if (File.Exists(SaveFile))
                File.Delete(SaveFile);
        }

        public void Save(Player player, int chapter)
        {
            var data = new SaveData
            {
                Version         = GameConstants.SaveVersion,
                PlayerName      = player.Name,
                Level           = player.Level,
                HP              = player.HP,
                MaxHP           = player.MaxHP,
                MP              = player.MP,
                MaxMP           = player.MaxMP,
                BaseAttack      = player.BaseAttack,
                BaseDefense     = player.BaseDefense,
                EXP             = player.EXP,
                EXPToNextLevel  = player.EXPToNextLevel,
                CorruptionLevel = player.CorruptionLevel,
                AcceptedDark    = player.AcceptedDarkPower,
                HelpedVillager  = player.HelpedVillager,
                BerserkUses     = player.TotalBerserkUses,
                ClassId         = (int)player.Class,
                CurrentChapter  = chapter
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFile, json);
        }

        /// <summary>Reads and deserializes the save file. Throws if the file is missing or invalid — caller decides how to report that.</summary>
        public (Player Player, int Chapter) Load()
        {
            string json = File.ReadAllText(SaveFile);
            var data = JsonSerializer.Deserialize<SaveData>(json)
                ?? throw new InvalidDataException("存檔資料無效");

            return (BuildPlayerFromSave(data), data.CurrentChapter);
        }

        private static Player BuildPlayerFromSave(SaveData d)
        {
            var p = new Player(d.PlayerName)
            {
                Level          = d.Level,
                HP             = d.HP,
                MaxHP          = d.MaxHP,
                MP             = d.MP,
                MaxMP          = d.MaxMP,
                BaseAttack     = d.BaseAttack,
                BaseDefense    = d.BaseDefense,
                EXP            = d.EXP,
                EXPToNextLevel = d.EXPToNextLevel,
                CorruptionLevel   = d.CorruptionLevel,
                AcceptedDarkPower = d.AcceptedDark,
                HelpedVillager    = d.HelpedVillager,
                TotalBerserkUses  = d.BerserkUses,
                Class             = (PlayerClass)d.ClassId
            };

            p.InitClassSkills();
            if (p.Level >= GameConstants.AdvancedSkillLevel)
                p.Skills.AddRange(SkillSystem.GetAdvancedSkills(p.Class));
            if (p.Level >= GameConstants.UltraSkillLevel)
                p.Skills.AddRange(SkillSystem.GetUltraSkills(p.Class));

            return p;
        }
    }
}
