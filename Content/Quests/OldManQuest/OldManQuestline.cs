using Stellamod.Common.QuestSystem;
using Stellamod.Content.Items.Materials;
using Stellamod.Items.Accessories;
using Stellamod.NPCs.Underground;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Quests.OldManQuest
{
    public class CollectFlowersI : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<OnionOfHeight>(), 1);
        }

        public override bool CanGiveQuest(Player player)
        {
            return true;
        }

        public override bool CheckCompletion(Player player)
        {
            return player.CountItem(ItemID.GlowingMushroom) >= 15;
        }
    }
    public class CollectFlowersII : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<OnionOfUselessness>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<CollectFlowersI>(player);
        }

        public override bool CheckCompletion(Player player)
        {
            return player.CountItem(ModContent.ItemType<Mushroom>()) >= 50;
        }
    }
    public class CollectFlowersIII : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<OnionOfSight>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<CollectFlowersII>(player) && Main.hardMode;
        }

        public override bool CheckCompletion(Player player)
        {
            return player.HasItem(ItemID.StrangePlant1) ||
                player.HasItem(ItemID.StrangePlant2) ||
                player.HasItem(ItemID.StrangePlant3) ||
                player.HasItem(ItemID.StrangePlant4);
        }
    }
    public class CollectFlowersIV : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AddReward(ModContent.ItemType<OnionOfStrength>(), 1);
            IsAutoQuest = true;
        }

        public override bool CanGiveQuest(Player player)
        {
            return HasCompletedQuest<CollectFlowersIII>(player) && NPC.downedPlantBoss;
        }

        public override bool CheckCompletion(Player player)
        {
            return NPC.killCount[ModContent.NPCType<RedFlower>()] + NPC.killCount[ModContent.NPCType<WhiteFlower>()] >= 10;
        }
    }
}
