using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Trailers;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{
    public class StalkersTallon : BaseSwingItemV2
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 18;
            Item.shoot = ModContent.ProjectileType<StalkersTallonSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<StalkersTallonStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Spear;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }

    public class StalkersTallonSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSpearSwingStyle(this);
            Trailer = new DesertBlazingTrail();
            useAfterImage = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 1f);
            }
        }
    }
    public class StalkersTallonStaminaSlash : BaseSwingProjectileV2
    {
        private bool _spawnedBird;
        public override void DefineCombo()
        {
            base.DefineCombo();
            useAfterImage = true;
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;
            Add(new OvalSwing
            {
                Duration = 45,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => EasingFunction.Anticipation2(lerpValue),
                Sound = swingSound1,
            });

            Trailer = new DesertBlazingTrail();
            useAfterImage = true;
        }


        public override void AI()
        {
            base.AI();
            if (!_spawnedBird && Interpolant >= 0.5f)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                        ModContent.ProjectileType<GrandStalkingBird>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner);
                }
                _spawnedBird = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 1f);
            }
        }
    }

    public class GrandStalkingBird : ScarletProjectile
    {
        //It's a bird that flies and picks up its target and carries them into the air lmao
        private enum AIState
        {
            Chasing,
            Pickup
        }
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private int TargetNPCIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            TrailCacheLength = 32;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            switch (State)
            {
                case AIState.Chasing:
                    if (Timer == 1)
                    {
                        Projectile.scale = 0.01f;
                        TargetNPCIndex = -1;
                    }
                    //Scale in as it flies
                    Projectile.velocity *= 1.001f;
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.1f);
                    break;
                case AIState.Pickup:
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.1f);
                    if (TargetNPCIndex == -1 || !Main.npc[TargetNPCIndex].active)
                        Projectile.Kill();

                    NPC targetNpc = Main.npc[TargetNPCIndex];
                    Vector2 velocity = (Projectile.Center - targetNpc.Center);
               
                    Projectile.velocity.X *= 0.9f;
                    targetNpc.velocity = velocity;
                    if(Timer >= 30)
                    {
                        Projectile.velocity.Y += 0.25f;
                    }
                    else
                    {
                        if (Projectile.velocity.Y > -10)
                            Projectile.velocity.Y -= 0.5f;
                    }

                    if(Timer >= 90)
                    {
                        targetNpc.velocity = Vector2.UnitY * 15;
                        if (this.OwnedByLocalClient())
                        {
                            float damage = Projectile.damage;
                            damage *= 3f;
                            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                                ModContent.ProjectileType<CinderBoom>(), (int)damage, 3);

                        }

                        for (float f = 0; f < 4; f++)
                        {
                            Vector2 v = Vector2.UnitY * 64;
                            v = v.RotatedBy(f / 4f * MathHelper.TwoPi);
                            v = v.RotatedBy(MathHelper.PiOver4);
                            FXUtil.GlowStretch(Projectile.Center, v);
                        }
                        ShakeModSystem.Shake = 4;
                        SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                        hitSound.PitchVariance = 0.2f;
                        SoundEngine.PlaySound(hitSound, targetNpc.position);

                        for (int i = 0; i < 7; i++)
                        {
                            Dust.NewDustPerfect(targetNpc.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
                        }

                        for (int i = 0; i < 7; i++)
                        {
                            Dust.NewDustPerfect(targetNpc.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
                        }

                        FXUtil.ShakeCamera(targetNpc.Center, 1024, 32);
                        FXUtil.GlowCircleBoom(targetNpc.Center,
                            innerColor: Color.White,
                            glowColor: Color.Yellow,
                            outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                        Projectile.Kill();
                    }
                    if (Timer % 6 == 0)
                    {
                        LegacyParticle.NewParticle<EmberParticle>(Projectile.Center, Vector2.UnitY, newColor: Color.White);
                    }
                    break;
            }
            //Create soem little sparkles as it trails
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>());
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                State = state;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (TargetNPCIndex == -1 && !target.boss)
            {
                SoundStyle sunstalkerPickupSound = new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Attack");
                sunstalkerPickupSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(sunstalkerPickupSound, target.position);

                FXUtil.GlowCircleBoom(target.Center, Color.LightGoldenrodYellow, Color.DarkGoldenrod, Color.Black);
                ShakeModSystem.Shake = 4;

                //Simple glow explosion
                float numDust = 16;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.LightGoldenrodYellow);
                }

                //Need to net update so other clients get this data
                //This is only called on the cilent that owns the projectile!
                TargetNPCIndex = target.whoAmI;
                SwitchState(AIState.Pickup);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            //Draw cool little trailing effect
            Texture2D zuiEffectTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 zuiEffectDrawOrigin = zuiEffectTexture.Size() / 2f;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            //Cool little after image on the trail
            for (int i = 0; i < TrailCacheLength; i++)
            {
                Vector2 oldDrawCenter = OldCenterPos[i];
                oldDrawCenter -= Main.screenPosition;
                float oldRot = OldCenterRot[i];
                float completionRatio = (float)i / (float)TrailCacheLength;

                Color afterImageColor = Color.Lerp(Color.Yellow, Color.Transparent, completionRatio);
                afterImageColor *= 0.15f;
                afterImageColor.A = 0;

                float drawScale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                drawScale *= 0.5f;
                spriteBatch.Draw(zuiEffectTexture, oldDrawCenter, null, afterImageColor, oldRot, zuiEffectDrawOrigin, drawScale, SpriteEffects.None, 0);

                spriteBatch.Draw(texture, oldDrawCenter, null, afterImageColor * 0.5f, oldRot, drawOrigin, 1f, SpriteEffects.None, 0);
            }


            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            drawCenter.Y += ExtraMath.Osc(0f, -4f, speed: 3);



            Color drawColor = Color.Yellow;
            drawColor.A = 0;
            spriteBatch.Draw(texture, drawCenter, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
