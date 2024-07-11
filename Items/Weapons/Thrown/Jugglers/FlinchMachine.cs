using Stellamod.Helpers;
using Stellamod.Projectiles.Thrown.Jugglers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.Items.Weapons.Thrown.Jugglers
{
    internal class FlinchMachine : ModItem
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
            Item.damage = 252;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 5);
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Lime;
            Item.crit = 16;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlinchMachineProj>();
            Item.shootSpeed = 24;
        }
    }
}
