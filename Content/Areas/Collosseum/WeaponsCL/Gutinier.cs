
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;
using Stellamod.Content.Trailers;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{
    public class Gutinier : BaseSwingItemV2
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
            Item.shoot = ModContent.ProjectileType<GutinierSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<GutinierStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Spear;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }


    public class GutinierSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSpearSwingStyle(this);
            Trailer = new DesertWindyTrail();
            useAfterImage = true;
        }


        public override void AI()
        {
            base.AI();

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

    public class GutinierGustBlast : ModProjectile
    {
        private Vector2[] _windPositions;
        private Vector2[] WindPositions
        {
            get
            {
                if(_windPositions == null)
                {
                    _windPositions = new Vector2[60];
                }

                float maxRange = 128f;
                float ease = EasingFunction.QuadraticBump(Timer / 30f);
                float range = ease * maxRange;

                Vector2 start = Projectile.Center - Vector2.UnitX * range;
                Vector2 end = Projectile.Center + Vector2.UnitX * range;
                VectorMath.FillArr(_windPositions, start, end);
                return _windPositions;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            SoundStyle windHit = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Magic.WindHit1 : AssetRegistry.Sounds.Magic.WindHit2;
            windHit.PitchVariance = 0.3f;
            SoundEngine.PlaySound(windHit, target.position);
        }

        private Color StripColors(float progressOnStrip)
        {
            return Color.Lerp(Color.Transparent, Color.LightGray, EasingFunction.QuadraticBump(progressOnStrip)) * 0.5f;
        }

        private float StripWidth(float progressOnStrip)
        {
            float baseWidth = 80;
            return MathHelper.SmoothStep(baseWidth, baseWidth, progressOnStrip);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, WindPositions, StripColors, StripWidth, shader);
        }
    }

    public class GutinierSpearThrow : ScarletProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 24;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
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

                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                afterImageColor *= 0.15f;
                afterImageColor.A = 0;

                float drawScale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                drawScale *= 0.5f;
                spriteBatch.Draw(zuiEffectTexture, oldDrawCenter, null, afterImageColor, oldRot, zuiEffectDrawOrigin, drawScale, SpriteEffects.None, 0);

                spriteBatch.Draw(texture, oldDrawCenter, null, afterImageColor * 0.5f, oldRot, drawOrigin, 1f, SpriteEffects.None, 0);
            }


            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            drawCenter.Y += ExtraMath.Osc(0f, -4f, speed: 3);
            spriteBatch.Draw(texture, drawCenter, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float num = 2f;
            for (float f = 0; f < num * 3; f++)
            {
                float progress = f / num * 3;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightGray;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(12, 25);
                particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                particle.VectorScale *= 0.25f;
            }
        }
    }

    public class GutinierStaminaSlash : BaseSwingProjectileV2
    {
        private bool _shotProjectile;
        public override void DefineCombo()
        {
            base.DefineCombo();
            Trailer = new DesertWindyTrail();
            useAfterImage = true;

            SoundStyle chargeSound = AssetRegistry.Sounds.Melee.ScythePull;
            chargeSound.PitchVariance = 0.1f;

            SoundStyle spearSlash2 = SoundRegistry.SpearSlash2;
            spearSlash2.PitchVariance = 0.25f;
            Add(new ThrustSwing
            {
                Duration = 25,
                Easing = (float lerpValue) => EasingFunction.QuadraticBump(lerpValue),
                OverrideVelocity = -Vector2.UnitY,
                ThrowDistance = 64,
                Sound = chargeSound,
            });

            Add(new ThrustSwing
            {
                Duration = 25,
                ThrowDistance = 200,
                Easing = (float lerpValue) => EasingFunction.QuadraticBump(lerpValue),
                Sound = spearSlash2
            });
        }

        public override void AI()
        {
            base.AI();
            switch (ComboIndex)
            {
                case 0:
                    if(Interpolant >= 0.5f && !_shotProjectile && this.OwnedByLocalClient())
                    {
                        SoundStyle windThrow = AssetRegistry.Sounds.Magic.WindCast1;
                        windThrow.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(windThrow, Projectile.position);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero,
                            ModContent.ProjectileType<GutinierGustBlast>(), Projectile.damage, 8, Projectile.owner);
                        _shotProjectile = true;
                    }
                    break;
                case 1:
                    if(Interpolant >= 0.5f && !_shotProjectile && this.OwnedByLocalClient())
                    {
                        SoundStyle windThrow = AssetRegistry.Sounds.Magic.WindCast2;
                        windThrow.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(windThrow, Projectile.position);


                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 15,
                            ModContent.ProjectileType<GutinierSpearThrow>(), Projectile.damage, 2, Projectile.owner);
                        _shotProjectile = true;
                    }
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
            int combo = ComboIndex + 1;
            int dir = comboPlayer.ComboDirection;
      
            if (combo < ComboCount)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.MouseWorld - Owner.Center, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Projectile.owner, ai2: combo, ai1: dir);
            }
        }
    }
}