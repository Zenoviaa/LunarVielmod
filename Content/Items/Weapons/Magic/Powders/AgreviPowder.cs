using Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions;
using Stellamod.Core.PowderSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Magic.Powders
{
    internal class AgreviPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Percent increase, 1 is +100% damage
            DamageModifier = 5;
            ExplosionType = ModContent.ProjectileType<AgreviBoom>();
            /*
            SoundStyle explosionSoundStyle = new SoundStyle($"Urdveil/Assets/Sounds/Kaboom");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;*/
            ExplosionScreenshakeAmt = 8;
        }
    }
}