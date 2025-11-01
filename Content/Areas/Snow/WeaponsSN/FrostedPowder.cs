using Stellamod.Content.Items.Materials;
using Stellamod.Core.IgnitersNPowders;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Snow.WeaponsSN
{
    public class FrostedPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 0.75f;
            ExplosionType = ModContent.ProjectileType<FrostbiteProj>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Frosty");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 2;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(),
                material: ModContent.ItemType<WinterbornShard>());
        }
    }
}