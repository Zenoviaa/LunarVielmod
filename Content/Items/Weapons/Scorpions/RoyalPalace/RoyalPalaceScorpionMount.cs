﻿using Stellamod.Content.Buffs.Scorpion;
using Stellamod.Core.ScorpionMountSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Scorpions.RoyalPalace
{
    internal class RoyalPalaceScorpionMount : BaseScorpionMount
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            MountData.buff = ModContent.BuffType<RoyalPalaceScorpionMountBuff>();
            scorpionItem = (BaseScorpionItem)new Item(ModContent.ItemType<RoyalPalaceScorpion>()).ModItem;
        }
    }
}
