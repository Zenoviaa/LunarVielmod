using Stellamod.Helpers;
using Stellamod.Projectiles.Thrown.Jugglers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown.Jugglers
{
    internal class BasicBaseball : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Alcarishasd",  Helpers.LangText.Common("Juggler"))
            {
                OverrideColor = ColorFunctions.JugglerWeaponType
            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 5);
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Blue;
            Item.crit = 16;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BasicBaseballProj>();
            Item.shootSpeed = 24;
        }
    }
}
