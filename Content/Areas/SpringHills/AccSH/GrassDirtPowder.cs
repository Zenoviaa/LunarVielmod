using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class GrassDirtPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            DamageModifier = 0.5f;
            ExplosionType = ModContent.ProjectileType<GrassExSps>();

            SoundStyle explosionSoundStyle = SoundID.DD2_ExplosiveTrapExplode;
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 1.5f;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(),
                material: ModContent.ItemType<Ivythorn>());
        }
    }
}