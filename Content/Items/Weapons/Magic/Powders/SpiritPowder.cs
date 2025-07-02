using Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions;
using Stellamod.Core.PowderSystem;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Magic.Powders
{
    internal class SpiritPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 7;
            ExplosionType = ModContent.ProjectileType<KaBoomSpirit>();
            /*
            SoundStyle explosionSoundStyle = new SoundStyle($"Urdveil/Assets/Sounds/Briskfly");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;*/
            ExplosionScreenshakeAmt = 2f;
        }
    }
}