using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Colosseum.Common;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Test
{
    public class ColosseumResetter : ModItem
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
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");
        }

        public override bool? UseItem(Player player)
        {
            if (MultiplayerHelper.IsHost)
            {
                ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
                colosseumSystem.Reset();
            }
            else
            {
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.ResetColosseum).Send(-1);
            }
            return true;
        }
    }
}
