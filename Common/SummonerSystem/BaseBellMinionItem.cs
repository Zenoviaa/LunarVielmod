using Microsoft.Xna.Framework;
using Stellamod.Core.XixianFlaskSystem;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem
{
    public abstract class BaseBellMinionItem : ModItem
    {
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 15;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Green;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            SetDefaults2();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            float ticks = GetAddedCastingTime();
            float seconds = ticks / 60;
            string secondsString = seconds.ToString("#.#");
            TooltipLine line = new TooltipLine(Mod, "AmountOfCastingTime",
                LangText.Common("CastingTime", secondsString));
            line.OverrideColor = Color.Lerp(new Color(80, 187, 180), Color.Black, 0.25f);
            tooltips.Add(line);
        }
        public virtual void SetDefaults2()
        {

        }


        public virtual float GetAddedCastingTime()
        {
            return 300;
        }
    }
}
