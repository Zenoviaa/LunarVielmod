using Microsoft.Xna.Framework;
using Stellamod.Common.WeaponTypes;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public class TomeExpandingTooltip : ExpandingArtifactTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            base.ModifyExpandableTooltips(item, lines);
            if(item.ModItem is AbstractMagicTome tome)
            {
                TooltipLine helpLine = new TooltipLine(Mod, "TomeHelp", LangText.Common("TomeHelp"));
                lines.Add(helpLine);
            }
        }
    }

    /// <summary>
    /// Base class for the magic tome attack style, it'll automatically set some defaults for you
    /// </summary>
    public abstract class AbstractMagicTome : ModItem
    {
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 24;
            Item.damage = 42;
            Item.knockBack = 1;
            Item.DamageType = DamageClass.Magic;
            Item.shootSpeed = 15f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.UseSound = SoundID.Item20;

            Item.rare = ItemRarityID.Green;
            Item.mana = 5;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            SetDefaults2();
        }

        /// <summary>
        /// Sets the color of the little dust particles that come off of the tome, defaults to white
        /// </summary>
        /// <returns></returns>
        public virtual Color GetTomeHintColor()
        {
            return Color.White;
        }

        public virtual void SetDefaults2()
        {

        }
    }
}
