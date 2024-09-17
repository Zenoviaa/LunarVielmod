using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.WorldG;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class TomeofRaining : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 62;
            Item.consumable = false;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 1);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 10;
            Item.useTime = 10;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }


        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {
            if (!Main.raining)
            {
                Main.StartRain();
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText auroeanStarfallEnded = NetworkText.FromLiteral(LangText.Misc("TomeofRaining"));
                    ChatHelper.BroadcastChatMessage(auroeanStarfallEnded, new Color(234, 96, 214));
                }
                else
                {
                    Main.NewText(LangText.Misc("TomeofRaining"), 234, 96, 214);
                }
                NetMessage.SendData(MessageID.WorldData);
            }

            return true;
        }

       
    }
}
