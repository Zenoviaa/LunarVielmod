using Stellamod.Core.CollectionSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.QuestSystem.Quests
{
    public class CauldronCrafting : Quest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //    AddReward(ModContent.ItemType<RadiantLantern>(), 1);
            // AddReward(ModContent.ItemType<Ivythorn>(), 10);
        }

        public override bool CanGiveQuest(Player player)
        {
            return true;
        }

        public override bool CheckCompletion(Player player)
        {
            return ModContent.GetInstance<Cauldron>().JustCrafted != null;
        }

        public override void StartQuest(Player player)
        {
            base.StartQuest(player);
            //         player.QuickSpawnItem(player.GetSource_FromThis(), ModContent.ItemType<Ivythorn>(), stack: 10);
        }

        public override void QuestIntroDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {
            base.QuestIntroDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            portrait = "ZuiPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "CauldronCraftingDialogue";
        }
    }
}
