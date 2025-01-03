﻿using Terraria;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class GothivCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 4;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 2;
        }

        public override int GetPowderSlotCount()
        {

            return 2;
        }
    }
}