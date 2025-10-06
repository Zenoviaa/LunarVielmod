using Stellamod.Items.Materials;
using Stellamod.NPCs.Town;
using Stellamod.UI.DialogueTowning;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.NPCs.Underground;
using Terraria.ID;
using Stellamod.NPCs.Govheil;
using Stellamod.Helpers;

namespace Stellamod.Core.QuestSystem.Quests.VeiizalQuest
{
    /*
     * Hunt I (Steeru 10) (Rhino)
        Hunt II (Rabbit 10) (Minty blast)
        Hunt III (Shark 1) (Electrifying)
        Hunt IV (Eye of Cthulu) (Shotty Pitol)
        Hunt V (Pirahna 25) (Pirahna)
        Hunt VI (Deerclops) (Gordon) 
        Hunt VII (Govheil Sprayer 3) (Sr Tetunas)
        Hunt VIII (Demon 10) (Obel)
        Hunt IX (Irradia) (Drygan)
        Hunt X (Sylia) (MeredaX)
        Hunt XI (Rek) (New gun)
        Hunt XII (enemy in lunarmoth palace) (Cool dualholster laser thing with curvature)
        */
    public class HuntI : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Rhino>(), 1);
        }

        public override bool CanGiveQuest(Player player)
        {
            return true;
        }

        public override bool CheckCompletion(Player player)
        {
            int npcType = ModContent.NPCType<Steeru>();
            int killCount = Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npcType]);
            return killCount >= 10;
        }
    }

    public class HuntII : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<MintyBlast>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntI>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            int npcType = NPCID.Bunny;
            int killCount = Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npcType]);
            return killCount >= 10;
        }
    }

    public class HuntIII : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Electrifying>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            int npcType = NPCID.Shark;
            int killCount = Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npcType]);
            return killCount >= 1;
        }
    }

    public class HuntIV : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<ShottyPitol>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntIII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            int npcType = NPCID.EyeofCthulhu;
            int killCount = Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npcType]);
            return killCount >= 1;
        }
    }
    public class HuntV : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Piranha>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntIV>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            int npcType = NPCID.Piranha;
            int killCount = Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npcType]);
            return killCount >= 25;
        }
    }
    public class HuntVI : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Gordon>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntV>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            int npcType = NPCID.Deerclops;
            int killCount = Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npcType]);
            return killCount >= 1;
        }
    }
    public class HuntVII : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<SrTetanus>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntVI>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            int npcType = ModContent.NPCType<GovheilProtector>();
            int killCount = Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npcType]);
            return killCount >= 3;
        }
    }
    public class HuntVIII : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Obel>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntVII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            int npcType = NPCID.Demon;
            int killCount = Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npcType]);
            return killCount >= 10;
        }
    }
    public class HuntIX : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Drygan>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntVIII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedIrradiaBoss;
        }
    }
    public class HuntX : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<MeredaX>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntIX>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedSyliaBoss;
        }
    }
    public class HuntXI : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<MeredaX>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntX>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedRekBoss;
        }
    }
    public class HuntXII : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<MeredaX>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<HuntXI>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedRekBoss;
        }
    }
}
