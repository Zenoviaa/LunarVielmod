using Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions;
using Stellamod.Core.PowderSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Magic.Powders
{
    internal class PoisonedPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 7;
            ExplosionType = ModContent.ProjectileType<JungleBoom>();

            /*
            SoundStyle explosionSoundStyle = new SoundStyle("Urdveil/Assets/Sounds/StaalkerDescend");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;*/
            ExplosionScreenshakeAmt = 2f;
        }
    }
}