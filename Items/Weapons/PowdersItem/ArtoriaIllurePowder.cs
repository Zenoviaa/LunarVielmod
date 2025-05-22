using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class ArtoriaIllurePowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 6;
            ExplosionType = ModContent.ProjectileType<IlluredBoom>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Green");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 3;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<IllurineScale>());
        }
    }
}