using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class GrassDirtPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 3;
            ExplosionType = ModContent.ProjectileType<GrassExSps>();

            SoundStyle explosionSoundStyle = SoundID.DD2_ExplosiveTrapExplode;
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 1.5f;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<Ivythorn>());
        }
    }
}