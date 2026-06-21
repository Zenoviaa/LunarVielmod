using Microsoft.Xna.Framework;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Items.Ores;
using System.Collections.Generic;

using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.AccIS
{
    public class GrailedExtenderPowder : ModItem
    {

        public override void SetDefaults()
        {

            Item.DefaultToAccessory();

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<IgniterPlayer>().boomerang = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<EreshkinCandle, BlankAccessory>();
        }




    }
}