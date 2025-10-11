using Stellamod.Core.QuestSystem;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Quests.DelgrimQuest
{
    public abstract class DelgrimQuest : Quest
    {
        public override void QuestIntroDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {
            base.QuestIntroDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            portrait = "DelgrimPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;
        }
    }

    public class MysteriousPlacesI : DelgrimQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<GintzlMetal>(), 100);
        }

        public override bool CanGiveQuest(Player player)
        {
            return true;
        }

        public override bool CheckCompletion(Player player)
        {
            return player.GetModPlayer<MyPlayer>().ZoneColloseum;
        }
    }
    public class MysteriousPlacesII : DelgrimQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<VillagersBroochA>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<MysteriousPlacesI>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.GetModPlayer<MyPlayer>().ZoneAbyss;
        }
    }
    public class MysteriousPlacesIII : DelgrimQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<CinderedQuiver>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<MysteriousPlacesII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.GetModPlayer<MyPlayer>().ZoneDrakonic;
        }
    }
    public class MysteriousPlacesIV : DelgrimQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Maelstrom>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<MysteriousPlacesIII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.GetModPlayer<MyPlayer>().ZoneGovheil;
        }
    }
    public class MysteriousPlacesV : DelgrimQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<MagicalBroochA>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<MysteriousPlacesIV>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.GetModPlayer<MyPlayer>().ZoneAlcadzia;
        }


    }
    public class MysteriousPlacesVI : DelgrimQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<DelgrimsHammer>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<MysteriousPlacesV>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.GetModPlayer<MyPlayer>().ZoneIlluria;
        }
    }

    public class MysteriousPlacesVII : DelgrimQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ItemID.SoulofLight, 100);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<MysteriousPlacesVI>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.ZoneHallow;
        }
    }

    public class MysteriousPlacesVIII : DelgrimQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ItemID.SoulofLight, 100);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<MysteriousPlacesVII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.GetModPlayer<MyPlayer>().ZoneMothlight;
        }
    }
}
