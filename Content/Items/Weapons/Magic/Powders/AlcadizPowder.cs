using Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions;
using Stellamod.Core.PowderSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Magic.Powders
{
    internal class AlcadizPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 2;
            ExplosionType = ModContent.ProjectileType<FableExSps>();
            /*
            SoundStyle explosionSoundStyle = new SoundStyle($"Urdveil/Assets/Sounds/HeatExplosion");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;*/
            ExplosionScreenshakeAmt = 2;
        }
    }
}