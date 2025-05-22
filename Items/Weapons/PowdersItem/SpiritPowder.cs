using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class SpiritPowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 7;
            ExplosionType = ModContent.ProjectileType<KaBoomSpirit>();

            SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Briskfly");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 2f;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<AlcaricMush>());
        }
    }
}