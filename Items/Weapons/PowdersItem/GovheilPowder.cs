﻿using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Projectiles;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class GovheilPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 7;
            ExplosionType = ModContent.ProjectileType<GovheilKaboom>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Rune");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 1.5f;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<VirulentPlating>());
        }

    }
}