using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    public class MushyPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 0.5f;
            ExplosionType = ModContent.ProjectileType<MushyBoom>();

            SoundStyle explosionSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/Green");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 4f;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<Mushroom>());
        }
    }
}