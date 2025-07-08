using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Stellamod.Core.ItemTemplates;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Ranged.Safunai.Rinavine
{
    public class Rinavine : BaseSafunaiItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Parandinea", LangText.Common("Safunai"))
            {
                OverrideColor = new Color(308, 71, 99)

            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Parendinea", "(B) Medium Damage Scaling (Grail shot) On Hit!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 18;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<RinavineProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 76;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
