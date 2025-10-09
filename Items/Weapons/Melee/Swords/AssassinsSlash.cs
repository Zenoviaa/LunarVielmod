using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Core.Lights;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Swords;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Core.Effects;
using Stellamod.Trailing;

namespace Stellamod.Items.Weapons.Melee.Swords
{
    public class AssassinsSlash : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<AssassinsSlashSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<AssassinsSlashStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
        }
    }

    public class AssassinsSlashSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            useAfterImage = true;
            SwingV2Helper.AddSwordSwingStyle(this);
            Trailer = TrailPresets.Assassin;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
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

    public class AssassinsSlashStaminaSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;
            Trailer = TrailPresets.Assassin;

            Add(new OvalSwing
            {
                Duration = 48,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 24,
                SwingDegrees = 315,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Owner.velocity -= swingDirection * 2;
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                _hit = true;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<Assassinate>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: target.whoAmI);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

        }
    }

    public class Assassinate : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int NPC => (int)Projectile.ai[1];
        private ref float SlashCount => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            base.AI();
            NPC myNpc = Main.npc[NPC];
            if (!myNpc.active)
            {
                Projectile.Kill();
            }

            Timer++;
            if(Timer == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
      ModContent.ProjectileType<AssassinsSpawnEffect>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
            }
            if(Timer <= 10)
            {
                SpecialEffectsPlayer player = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                player.blackWhiteStrength = 0.66f;
                player.blackWhiteThreshold = 0.5f;
            }
            if(Timer >= 20)
            {
                SpecialEffectsPlayer player = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                player.blackWhiteStrength = 1f;
                player.blackWhiteThreshold = 0.5f;
            }
            if (Timer == 25)
            {
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(myNpc.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
                    ModContent.ProjectileType<AssassinsSpawnEffect>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
                        ModContent.ProjectileType<AssassinsSlashProj>(), 0, 1, Projectile.owner, 0, 0);
                    SlashCount++;
                    if (SlashCount >= 10)
                    {
                        Projectile.Kill();
                    }
                }
            }
            if (Timer >= 25)
            {
                Timer = 20;
            }
        }
    }
}