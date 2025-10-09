using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Melee.Swords;
using Stellamod.Projectiles.Slashers.ScarecrowSaber;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class ScarecrowSaber : BaseSwingItemV2
    {
        private float _swingDir = 1;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<ScarecrowSaberBasicSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<ScarecrowSaberSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
        }
        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<ScarecrowSaberPlayer>().CooldownTimer <= 0;
        }


        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(2))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.CopperCoin);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(7))
            {
                int dist = 40;
                Vector2 targetExplosionPos = target.Center;
                for (int i = 0; i < 200; ++i)
                {
                    if (Main.npc[i].active && (Main.npc[i].Center - targetExplosionPos).Length() < dist)
                    {
                        Main.npc[i].HitEffect(0, damageDone);
                    }
                }
                for (int i = 0; i < 15; ++i)
                {
                    target.AddBuff(BuffID.OnFire, 300, true);
                    int newDust = Dust.NewDust(new Vector2(targetExplosionPos.X - (dist / 2), targetExplosionPos.Y - (dist / 2)), dist, dist, DustID.CopperCoin, 0f, 0f, 40, default(Color), 2.5f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].velocity *= 5f;
                    newDust = Dust.NewDust(new Vector2(targetExplosionPos.X - (dist / 2), targetExplosionPos.Y - (dist / 2)), dist, dist, DustID.CopperCoin, 0f, 0f, 40, default(Color), 1.5f);
                    Main.dust[newDust].velocity *= 3f;
                }
            }
        }
    }
    public class ScarecrowSaberBasicSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSpearSwingStyle(this);
            Trailer = TrailPresets.LightSpand;
            useAfterImage = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            target.AddBuff(BuffID.OnFire, 120);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }
    }

}