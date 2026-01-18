using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR
{
    public class StopSignSword : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 18;
            Item.shoot = ModContent.ProjectileType<StopSignSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<StopSignThrow>();
            meleeWeaponType = MeleeWeaponType.Sword;
            staminaCost = 1;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<TerrorFragments, BlankSword>();
        }
    }

    public class StopSignThrow : ModProjectile
    {
        private float _spinAlpha;
        private bool _doneHitStop;
        public override string Texture => ModContent.GetInstance<StopSignSword>().Texture;

        private ref float Timer => ref Projectile.ai[0];
        private ref float HitStopTimer => ref Projectile.ai[1];

        private Vector2 InitialVelocity;
    
        private Player Owner => Main.player[Projectile.owner];

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(InitialVelocity);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            InitialVelocity = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            _spinAlpha = EasingFunction.QuadraticBump((float)Projectile.timeLeft / 180f) * 0.5f;
            Timer++;
            if(Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
            }

            if(HitStopTimer > 0)
            {
                HitStopTimer--;
                Projectile.velocity = Vector2.Zero;
                return;
            }

            if(Timer < 15)
            {
                InitialVelocity *= 1.1f;
            } else if (Timer < 60)
            {
                InitialVelocity *= 0.94f;
            }
            if (Timer % 16 == 0)
            {
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(4, 4), Scale: Main.rand.NextFloat(0.5f, 1f));
                sp.flickering = true;
                sp.gravity = 0;
                sp.innerColor = Color.Red;
                sp.outerColor = Color.DarkRed;
            }

            float ratio = (float)(Projectile.timeLeft - 30) / 60f;
            ratio = MathHelper.Clamp(ratio, 0f, 1f);
            float interp = MathHelper.Lerp(1f, 0f, ratio);
            float ease = EasingFunction.InOutSine(interp);
            Vector2 velocity = Vector2.Lerp(InitialVelocity, (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * InitialVelocity.Length() * 2.5f, ease);

            if(Timer > 100)
            {
                InitialVelocity *= 1.02f;
            }
            float distanceToOwner = Vector2.Distance(Owner.Center, Projectile.Center);
            if(distanceToOwner <= 64 && Timer > 30)
            {
                Projectile.Kill();
            }
            Projectile.velocity = velocity;
            Projectile.rotation += Projectile.velocity.Length() * 0.025f + 0.02f;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float numDust = 16;
            for (int n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, velocity, Color.White, Main.rand.NextFloat(0.5f, 1f));
                sp.flickering = true;
                sp.innerColor = Color.Red;
                sp.outerColor = Color.DarkRed;
                sp.gravity = 0f;
                sp.dampening = 0.1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_doneHitStop)
            {
                HitStopTimer = 6;
                _doneHitStop = true;
                Projectile.netUpdate = true;
            }
          
            int numDust = 6;
            FXUtil.ShakeCamera(target.Center, 1024, 2);
            for (int n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Lavender,
                    outerColor = Color.Violet,
                    scaleRange = new Vector2(0.4f, 0.7f)
                };

                var smokeParticle = SmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY, Color.DarkGray, Main.rand.NextFloat(0.5f, 2f));
                smokeParticle.initialColor = Color.Lerp(Color.Lavender, Color.Black, 0.7f);
            }
        }
        private float TrailWidthFunction(float p)
        {
            return MathHelper.SmoothStep(0, 24, p) * EasingFunction.QuadraticBump(p);
        }

        private Color TrailColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightCyan, p);
            return trailColor;
        }

        private void DrawPixelTrails(GraphicsDevice graphicsDevice)
        {
            float numPoints = 48;
            Vector2[] circlePos = new Vector2[(int)numPoints];
            for(int i = 0; i < circlePos.Length; i++)
            {
                Vector2 rotationOffset = Projectile.rotation.ToRotationVector2() * 48;
                rotationOffset = rotationOffset.RotatedBy((float)i / numPoints * MathHelper.TwoPi * 1f + Main.GlobalTimeWrappedHourly * 8);
                Vector2 offsetPosition = Projectile.Center + rotationOffset;
                circlePos[i] = offsetPosition;
            }

            GradientTrailShader gradientTrailShader = GradientTrailShader.Instance;
            gradientTrailShader.GradientTexture = StopSignSlash.GradientTexture;
            gradientTrailShader.LaserTexture = AssetRegistry.Textures.Trails.BasicSlash_Wide1;
            TrailDrawer.Draw(Main.spriteBatch, circlePos, TrailColorFunction, TrailWidthFunction, gradientTrailShader);
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
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 position = Projectile.oldPos[i];
                drawCenter = position + Projectile.Size * 0.5f;
                drawCenter -= Main.screenPosition;
                float ratio = (float)i / (float)Projectile.oldPos.Length;
                Color glowColor = drawColor;
                glowColor *= MathHelper.SmoothStep(1f, 0f, ratio) * 0.3f;
                spriteBatch.Draw(spinTexture, drawCenter, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.5f * MathHelper.SmoothStep(1f, 0f, ratio), SpriteEffects.None, 0);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelTrails);
            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 position = Projectile.oldPos[i];
                Vector2 drawCenter = position + Projectile.Size * 0.5f;
                drawCenter -= Main.screenPosition;
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Vector2 drawOrigin = texture.Size() / 2f;
                float ratio = (float)i / (float)Projectile.oldPos.Length;
                Color glowColor = Color.Lerp(Color.White, Color.Black, ratio) * 0.05f;
                glowColor.A = 0;
                spriteBatch.Draw(texture, drawCenter, null, glowColor, Projectile.oldRot[i], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            this.DrawCentered(ref lightColor);
            DrawSpinEffect();
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            return false;
        }


    }
    public class StopSignSlash : BaseSwingProjectileV2
    {
        public static Texture2D GradientTexture { get; private set; }
        public static Texture2D GradientTexture2 { get; private set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.QueueMainThreadAction(() =>
            {
                GradientTexture2 = DrawHelper.CreateGradient(Color.Black, Color.Red, Color.White);
                GradientTexture = DrawHelper.CreateGradient( Color.White, Color.Black, Color.Red);
            });

        }

        public override void Unload()
        {
            base.Unload();
            Main.QueueMainThreadAction(() =>
            {
                GradientTexture?.Dispose();
                GradientTexture = null;
                GradientTexture2?.Dispose();
                GradientTexture2 = null;
            });
        }

        public override void DefineCombo()
        {
            base.DefineCombo();
            ComboBuilder comboBuilder = new ComboBuilder();
            comboBuilder.AddSwordSlash1(duration: 17)
                .AddSwordSlash2(duration: 17)
                .AddSwordSlash1(duration: 17)
                .AddSwordSlash2(duration: 17)
                .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 720, hitCount: 2)
                .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 720, hitCount: 2)
                .AddSwordSlash3(duration: 38, swingDegress: 720, hitCount: 3);
            comboBuilder.AddToProjectile(this);
            useAfterImage = true;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Red * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }

        private float TrailWidthFunction(float p)
        {
            return MathHelper.SmoothStep(0, 24, p) * EasingFunction.QuadraticBump(Interpolant);
        }

        private Color TrailColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightCyan, p);
            return trailColor;
        }

        public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {
            base.RenderSwingTrail(ref lightColor, points);
            GradientTrailShader gradientTrailShader = GradientTrailShader.Instance;
            gradientTrailShader.GradientTexture = GradientTexture;
            gradientTrailShader.LaserTexture = AssetRegistry.Textures.Trails.BasicSlash_Wide1;
            TrailDrawer.Draw(Main.spriteBatch, points, TrailColorFunction, TrailWidthFunction, gradientTrailShader);
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

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
