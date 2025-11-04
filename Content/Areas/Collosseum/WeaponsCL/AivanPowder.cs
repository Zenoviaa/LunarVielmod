using Stellamod.Core.IgnitersNPowders;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{
    public class AivanPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 0.7f;
            ExplosionType = ModContent.ProjectileType<AivanKaboom>();

            SoundStyle explosionSoundStyle = SoundID.DD2_ExplosiveTrapExplode;
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 2;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(),
                material: ModContent.ItemType<GintzlMetal>());
        }
    }
}