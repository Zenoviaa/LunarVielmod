using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Projectiles;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class ShadowFlamePowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 5;
            ExplosionType = ModContent.ProjectileType<KaBoomShade>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/ExplosionBurstBomb");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 2f;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<MiracleThread>());
        }
    }
}