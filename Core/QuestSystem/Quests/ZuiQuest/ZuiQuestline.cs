using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Accessories;
using Stellamod.Items.Armors.Witchen;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Special;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Town;
using Stellamod.UI.DialogueTowning;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.QuestSystem.Quests.ZuiQuest
{
    public abstract class ZuiQuest : Quest
    {
        public override void QuestIntroDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {
            base.QuestIntroDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            portrait = "ZuiPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;
        }
    }

    public class TalkToZui : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Ivythorn>(), 10);
        }

        public override bool CanGiveQuest(Player player)
        {
            return true;
        }

        public override bool CheckCompletion(Player player)
        {
            return ModContent.GetInstance<DialogueTowningUISystem>().WhosTalking == ModContent.NPCType<Zui>();
        }
    }

    public class CraftAtCauldron : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<AnotherRock>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<TalkToZui>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return ModContent.GetInstance<Cauldron>().JustCrafted != null;
        }
    }

    public class GoTrainI : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<TomeofRaining>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<CraftAtCauldron>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedStoneGolemBoss;
        }
    }

    public class GoTrainII : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<GoTrainI>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedJackBoss;
        }
    }
    public class ReadyUp : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ItemID.BundleofBalloons, 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<GoTrainII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.HasItem(ModContent.ItemType<VoidKey>());
        }
    }
    public class DestroySingularity : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Hookarama>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<ReadyUp>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedSOMBoss;
        }
    }
    public class KillWallofFlesh : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<EckasectSire>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<DestroySingularity>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return Main.hardMode;
        }
    }
    public class KillPlantera : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<ChromaCutter>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<KillWallofFlesh>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return NPC.downedPlantBoss;
        }
    }
    public class GoTrainIII : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<IshtarKey>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<KillPlantera>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedZuiBoss;
        }
    }
    public class GetVoidalPassageway : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<WitchenHat>(), 1);
            AddReward(ModContent.ItemType<WitchenRobe>(), 1);
            AddReward(ModContent.ItemType<WitchenPants>(), 1);

            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<GoTrainIII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.HasItem(ModContent.ItemType<VoidalPassageway>());
        }
    }

    public class DestroySingularityII : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Hookarama>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<GetVoidalPassageway>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedSupernovaFragmentBoss;
        }
    }
    public class DestroySingularityIII : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Hookarama>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<DestroySingularityII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedSupernovaFragmentBoss;
        }
    }
    public class KillEreshkigal : ZuiQuest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<Hookarama>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<DestroySingularityIII>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return DownedBossSystem.downedSupernovaFragmentBoss;
        }
    }
}
