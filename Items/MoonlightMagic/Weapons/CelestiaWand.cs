using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Items.Ores;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class CelestiaWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 14;
            Item.shootSpeed = 12;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Size = 12;
            TrailLength = 18;
        }

        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 2;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<GlisteningBar>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}