using Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions;
using Stellamod.Core.PowderSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Magic.Powders
{
    internal class GovheilPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 7;
            ExplosionType = ModContent.ProjectileType<GovheilKaboom>();

            /*
            SoundStyle explosionSoundStyle = new SoundStyle($"Urdveil/Assets/Sounds/Binding_Abyss_Rune");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;*/
            ExplosionScreenshakeAmt = 1.5f;
        }
    }
}