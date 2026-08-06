using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Zui;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables;

internal class ScryingIdol : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 28;
        Item.consumable = false;
        Item.rare = ItemRarityID.Green;
        Item.maxStack = 1;
        Item.value = Item.buyPrice(gold: 10);
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useAnimation = 10;
        Item.useTime = 10;
    }




    public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
    {


        if (StellaMultiplayer.IsHost)
        {
            EventWorld.StartGinzteArmy();
        }
        else
        {
            ModPacket packet = ModContent.GetInstance<Stellamod>().GetPacket();
            packet.Write((byte)MessageType.STARTGINTZEFROMCLIENT);
            packet.Write((byte)player.whoAmI);
            packet.Send(ignoreClient: player.whoAmI);
        }


        return true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Leather, 5);
        recipe.AddIngredient(ItemID.IronBar, 5);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
