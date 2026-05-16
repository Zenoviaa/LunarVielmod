using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Projectiles.Paint;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class InkingSpire : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 48;
            Item.shoot = ModContent.ProjectileType<InkingSpireSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<InkingSProj>();
            meleeWeaponType = MeleeWeaponType.Sword;
            staminaCost = 1;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<KaleidoscopicInk>());
        }
    }

    public class InkingSpireSlash : BaseSwingProjectileV2
    {
        public bool Hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);
            Trailer = TrailPresets.InkingSpire;
            swordBeamLength = 32;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }
    }
}