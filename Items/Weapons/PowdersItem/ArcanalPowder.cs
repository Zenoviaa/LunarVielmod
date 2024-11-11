using Stellamod.Items.Weapons.Igniters;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class ArcanalPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 7;
            ExplosionType = ModContent.ProjectileType<SepsisExSps>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/ArcaneExplode");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 3;
        }
    }
}