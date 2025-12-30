using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Trailers;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class Infernis : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 14;
            Item.shoot = ModContent.ProjectileType<InfernisSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<InfernisProj>();
            meleeWeaponType = MeleeWeaponType.Spear;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankStaff>();
        }
    }

    public class InfernisSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSpearSwingStyle(this);
            Trailer = new IyxFlamingTrail();
            useAfterImage = true;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            target.AddBuff(BuffID.OnFire, 120);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
            }
        }
    }

    public class InfernisProj : ModProjectile
    {
        private float _spinAlpha;
        private Vector2 _stretchScale;
        private enum AIState
        {
            Spin,
            Fall
        }
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 5;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.timeLeft = 660;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
        }

        private void SwitchState(AIState state)
        {
            State = state;
            Timer = 0;
        }

        public override void AI()
        {
            Timer++;
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            }

            switch (State)
            {
                case AIState.Spin:
                    _stretchScale = Vector2.One;
                    if (Timer == 1)
                    {
                        Projectile.velocity *= 2;
                    }
                    Projectile.tileCollide = false;
                    Projectile.velocity *= 0.97f;
                    Projectile.rotation += Projectile.velocity.Length() * 0.05f;
                    if (Timer >= 60)
                    {
                        Projectile.velocity *= 0.96f;
                    }
                    if (Timer >= 60)
                    {
                        Projectile.rotation = Vector2.UnitY.ToRotation() + MathHelper.PiOver4;
                        _spinAlpha = MathHelper.Lerp(_spinAlpha, 0f, 0.1f);
                    }
                    else
                    {
                        _spinAlpha = MathHelper.Lerp(_spinAlpha, 0.5f, 0.1f);
                    }
                    if (Timer >= 90)
                    {
                        SwitchState(AIState.Fall);
                    }
                    break;
                case AIState.Fall:
                    _spinAlpha = MathHelper.Lerp(_spinAlpha, 0f, 0.1f);
                    _stretchScale = Vector2.Lerp(Vector2.One, new Vector2(1f, 1.5f), Projectile.velocity.Length() / 20f);
                    if (Timer == 1)
                    {
                        Projectile.velocity = Vector2.UnitY;
                    }

                    Projectile.rotation = Vector2.UnitY.ToRotation() + MathHelper.PiOver4;
                    Projectile.tileCollide = true;
                    Projectile.extraUpdates = 1;
                    Projectile.velocity.Y += 0.2f;
                    Projectile.velocity.Y *= 1.01f;
                    break;
            }
        }


        public override void OnKill(int timeLeft)
        {
            SoundStyle infernisBoomSound = new SoundStyle("Stellamod/Assets/Sounds/Infernis1");
            infernisBoomSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(infernisBoomSound, Projectile.position);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.White,
               glowColor: Color.Yellow,
               outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);


            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.White,
               glowColor: Color.Yellow,
               outerGlowColor: Color.Red, duration: 25, baseSize: 0.2f);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX * 8,
                ModContent.ProjectileType<CinderBreakerEruptor>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitX * 8,
              ModContent.ProjectileType<CinderBreakerEruptor>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        private void DrawSpinEffect()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D spinTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Spiin").Value;
            Vector2 drawOrigin = spinTexture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.Red;
            drawColor.A = 0;
            drawColor *= _spinAlpha;
            spriteBatch.Draw(spinTexture, drawCenter, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.5f, SpriteEffects.None, 0);
        }

        private void DrawAfterImage()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D spearTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 spearOrigin = spearTexture.Size() / 2f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = (float)i / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.Red, Color.Yellow, completionRatio);
                fadeColor.A = 0;
                fadeColor *= MathHelper.SmoothStep(1f, 0f, completionRatio);
                Vector2 drawCenter = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                spriteBatch.Draw(spearTexture, drawCenter, null, fadeColor, Projectile.rotation, spearOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
        }

        private void DrawSpear(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D spearTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 spearOrigin = spearTexture.Size() / 2f;
            spriteBatch.Draw(spearTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, spearOrigin, Projectile.scale * _stretchScale, SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSpinEffect();
            DrawAfterImage();
            DrawSpear(ref lightColor);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }
    }
}
