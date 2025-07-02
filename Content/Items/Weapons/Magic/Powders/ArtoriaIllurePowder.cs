using Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions;
using Stellamod.Core.PowderSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Magic.Powders
{
    internal class ArtoriaIllurePowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 6;
            ExplosionType = ModContent.ProjectileType<IlluredBoom>();
            /*
            SoundStyle explosionSoundStyle = new SoundStyle($"Urdveil/Assets/Sounds/Green");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;*/
            ExplosionScreenshakeAmt = 3;
        }
    }
}