using Stellamod.Items.Weapons.Igniters;
using Stellamod.Projectiles;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class AbyssalPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            DamageModifier = 12;
            ExplosionType = ModContent.ProjectileType<VoidKaboom>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/ExplosionBurstBomb");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 4;
        }
    }
}