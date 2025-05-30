﻿using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class EYIgniter : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 1;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 2;
        }

        public override int GetPowderSlotCount()
        {

            return 3;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankCard>(), material: ModContent.ItemType<Ivythorn>());
        }
    }
}