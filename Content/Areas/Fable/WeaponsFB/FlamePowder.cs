using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Projectiles;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class FlamePowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 0.5f;
            ExplosionType = ModContent.ProjectileType<KaBoom>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Kaboom");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 2;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(),
                material: ModContent.ItemType<AlcadizScrap>());
        }
    }
}