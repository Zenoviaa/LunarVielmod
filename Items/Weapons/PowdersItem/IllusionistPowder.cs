using Stellamod.Items.Weapons.Igniters;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class IllusionistPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 9;
            ExplosionType = ModContent.ProjectileType<EldritchBoom>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/StormDragon_LightingZap");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 1.5f;
        }
    }
}