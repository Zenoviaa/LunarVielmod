using Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions;
using Stellamod.Core.PowderSystem;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Magic.Powders
{
    internal class AbyssalPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            DamageModifier = 5;
            ExplosionType = ModContent.ProjectileType<VoidKaboom>();
            /*
            SoundStyle explosionSoundStyle = new SoundStyle($"Urdveil/Assets/Sounds/ExplosionBurstBomb");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;*/
            ExplosionScreenshakeAmt = 4;
        }
    }
}