using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.QuestSystem
{
    internal class QuestResetter : ModItem
    {
        private int _useIndex;
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
            // DisplayName.SetDefault("Teraciz");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool? UseItem(Player player)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            questPlayer.ActiveQuests.Clear();
            questPlayer.CompletedQuests.Clear();
            questPlayer.RewardQuests.Clear();
            questPlayer.RecalculateUI = true;
            return true;
        }
    }
}
