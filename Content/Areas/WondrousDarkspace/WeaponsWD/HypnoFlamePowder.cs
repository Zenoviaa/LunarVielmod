using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Projectiles;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class HypnoFlamePowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 5;
            ExplosionType = ModContent.ProjectileType<KaBoomShade>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/ExplosionBurstBomb");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 2f;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<HypnotizedSoul>());
        }
    }
}