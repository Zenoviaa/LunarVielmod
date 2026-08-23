using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB
{
    public class EventHorizon : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 28;
            Item.knockBack = 4;
            Item.shoot = ModContent.ProjectileType<EventHorizonSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<EventHorizonStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
            staminaCost = 4;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<ConvulgingMater, BlankSword>();
        }
    }

    public class EventHorizonRift : ModProjectile
    {
        private Vector2[] RiftPoints = new Vector2[32];
        private ref float Timer => ref Projectile.ai[0];
        private ref float RandScale => ref Projectile.ai[1];
        private bool IsLong => Projectile.ai[2] == 1;
        private float Interpolant;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.timeLeft = 80;
            Projectile.friendly = true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 startCenter = Projectile.Center - Projectile.velocity * 64;
            Vector2 endCenter = Projectile.Center;
            Vector2 center = Vector2.Lerp(startCenter, endCenter, EasingFunction.OutExpo(Timer / 30f));
            Vector2 start = center - Projectile.velocity * 16 * RandScale;
            Vector2 end = center + Projectile.velocity * 16 * RandScale;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 12, ref collisionPoint);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                if (IsLong)
                {
                    Projectile.timeLeft += 180;
                }
                if (this.OwnedByLocalClient())
                {
                    RandScale = Main.rand.NextFloat(0.5f, 1f);
                }
                SoundStyle eventHorizonSound = new SoundStyle("Stellamod/Assets/Sounds/EventHorizon1");
                eventHorizonSound.PitchVariance = 0.3f;
                if (Main.rand.NextBool(2))
                {
                    eventHorizonSound = new SoundStyle("Stellamod/Assets/Sounds/EventHorizon2");
                    eventHorizonSound.PitchVariance = 0.3f;
                }
                eventHorizonSound.Volume = 0.5f;
                SoundEngine.PlaySound(eventHorizonSound, Projectile.position);
            }
            if (Timer % 9 == 0)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Projectile.velocity.RotatedByRandom(4f) * Main.rand.NextFloat(0.1f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.innerColor = Color.Black;
                dp.outerColor = Color.DarkBlue;
            }
            Interpolant = EasingFunction.InExpo(Timer / 80f);
            if (IsLong)
            {
                Interpolant = EasingFunction.InExpo(Timer / 260f);
                Projectile.velocity = Projectile.velocity.RotatedBy(0.005f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedTrails, DrawLayer.OverNPCs);
            return false;
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.White;
        }
        private Color GetTrailColor2(float completionRatio)
        {
            return Color.Black;
        }
        private float GetTrailWidth(float completionRatio)
        {
            float baseWidth = EasingFunction.QuadraticBump(completionRatio) * 32;
            float outScale = MathHelper.Lerp(1f, 0f, Interpolant);
            float inScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 15f));
            return baseWidth * outScale * inScale;
        }
        private float GetTrailWidth2(float completionRatio)
        {
            return GetTrailWidth(completionRatio) * 0.3f;
        }

        private void RenderPixelatedTrails(GraphicsDevice graphicsDevice)
        {
            float numPoints = 32;

            float length = 16;
            if (IsLong)
                length *= 0.5f;
            Vector2 startCenter = Projectile.Center - Projectile.velocity * 64;
            Vector2 endCenter = Projectile.Center;
            Vector2 center = Vector2.Lerp(startCenter, endCenter, EasingFunction.OutExpo(Timer / 30f));
            Vector2 start = center - Projectile.velocity * length * RandScale;
            Vector2 end = center + Projectile.velocity * length * RandScale;
            for (int n = 0; n < numPoints; n++)
            {
                ref Vector2 point = ref RiftPoints[n];
                float ratio = (float)n / numPoints;
                point = Vector2.Lerp(start, end, ratio);
                point += Main.rand.NextVector2Circular(2, 2);
            }

            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Black;
            Color innerColor = Color.Lerp(Color.Violet, Color.DarkBlue, 0.75f);
            shader.InnerColor = innerColor;
            shader.OuterColor = Color.Blue;
            if (Timer < 15)
            {
                shader.OuterColor = Color.Lerp(Color.White, Color.Blue, EasingFunction.InOutSine(Timer / 15f));
                shader.InnerColor = Color.Lerp(Color.White, innerColor, EasingFunction.InOutSine(Timer / 15f));
                shader.LaserColor = Color.Lerp(Color.White, Color.Black, EasingFunction.InOutSine(Timer / 15f));
            }
            TrailDrawer.Draw(Main.spriteBatch, RiftPoints, GetTrailColor, GetTrailWidth, shader);


            var blackShader = BasicLaserAlphaShader.Instance;
            blackShader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, RiftPoints, GetTrailColor2, GetTrailWidth2, blackShader);


        }
    }

    public class EventHorizonSlash : BaseSwingProjectileV2
    {
        private bool _firedProjectile;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);

            glowColor = Color.Lerp(Color.Transparent, Color.Red * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
            useAfterImage = true;
        }

        public override void AI()
        {
            base.AI();
            if (this.OwnedByLocalClient() && !_firedProjectile && Interpolant >= 0.3f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + Projectile.velocity * 8, Projectile.velocity.RotatedBy(MathHelper.PiOver4).RotatedByRandom(1.5f),
                    ModContent.ProjectileType<EventHorizonRift>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                _firedProjectile = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            int numDust = 2;
            for (int n = 0; n < numDust; n++)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(target.Center, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f), Color.Black, Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.outerColor = Color.DarkBlue;
            }
        }

        public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {
            base.RenderSwingTrail(ref lightColor, points);
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Black;
            shader.InnerColor = Color.Lerp(Color.Violet, Color.DarkBlue, 0.75f);
            shader.OuterColor = Color.Blue;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor, GetTrailWidth, shader);


            var blackShader = BasicLaserAlphaShader.Instance;
            blackShader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor2, GetTrailWidth2, blackShader);
        }
        private Color GetTrailColor2(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Black, EasingFunction.QuadraticBump(completionRatio));
        }

        private float GetTrailWidth2(float completionRatio)
        {
            return GetTrailWidth(completionRatio) * 0.25f;
        }
        private Color GetTrailColor(float completionRatio)
        {
            return Color.White;
        }

        private float GetTrailWidth(float completionRatio)
        {
            return EasingFunction.QuadraticBump(completionRatio) * 48;
        }
    }

    //Stamina (3) - Holds the sword above you creating a small singularity
    //That does huge amounts of damage if something is inside of it before it explodes
    public class EventHorizonStaminaSlash : BaseSwingProjectileV2
    {
        private bool _createdSingularity;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle chargeSound = AssetRegistry.Sounds.Melee.ScythePull;
            chargeSound.PitchVariance = 0.1f;
            Add(new ThrustSwing
            {
                Duration = 64,
                Easing = EasingFunction.InOutExpo,
                OverrideVelocity = -Vector2.UnitY,
                ThrowDistance = 128,
                Sound = chargeSound,
            });

        }

        public override void AI()
        {
            base.AI();
            if (this.OwnedByLocalClient() && !_createdSingularity && Interpolant >= 0.4f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitY * 100, Vector2.Zero,
                    ModContent.ProjectileType<EventHorizonSingularity>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                _createdSingularity = true;
            }
        }
    }

    public class EventHorizonBoom : ModProjectile
    {
        private float _scale = 2f;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            if (Timer == 1)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);

                FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY * 2, 8, 8, 32);
                int count = 32;
                float degreesPer = 360 / (float)count;
                for (int k = 0; k < count; k++)
                {
                    float degrees = k * degreesPer;
                    Vector2 d = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                    Vector2 vel = d * 8;
                    Dust.NewDust(Projectile.Center, 0, 0, DustID.GemDiamond, vel.X * 0.5f, vel.Y * 0.5f);
                }

                var part = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Purple, baseSize: 0.2f);
                part.Scale *= 6;

                var part3 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Purple, baseSize: 0.15f);
                part3.Scale *= 4;

                var part2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Purple);
                part2.Scale *= 3;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadowExplosion"), Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARGROP"), Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb"), Projectile.position);

                for (float f = 0; f < 42; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(128, 128);
                    FXUtil.GlowStretch(Projectile.Center, velocity);
                }
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightCyan,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Scale *= 4;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TrailRegistry.BeamTrail.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            var shader = RadialBlastShader.Instance;

            float prog = Timer / 30f;
            float interp = EasingFunction.OutExpo(prog);
            shader.Offset = Vector2.Lerp(Vector2.One * 0.25f, -Vector2.One * 0.25f, interp);
            shader.Tiling = Vector2.Lerp(Vector2.One * 4, Vector2.One * 32, interp);
            shader.InnerColor = Color.Lerp(Color.White, Color.Black, interp);
            shader.OuterColor = Color.Lerp(Color.Blue, Color.Black, EasingFunction.OutSine(prog));
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.4f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
    public class EventHorizonSingularity : ModProjectile
    {
        private float _spinRotTimer;
        private float _incresionDiskFrameBottom;
        private float _incresionDiskFrameTop;
 
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => ModContent.GetInstance<VerlianSingularity>().Texture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = 12;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            base.AI();
            _spinRotTimer++;
            Timer++;
            if(Timer == 1)
            {
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY.RotatedBy(MathHelper.PiOver4).RotatedByRandom(0.25f) * 32,
ModContent.ProjectileType<EventHorizonRift>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai2: 1);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * 2).RotatedByRandom(0.25f) * 32,
ModContent.ProjectileType<EventHorizonRift>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai2: 1);
                }
            }
            ShakeScreenPosition.Shake = 1;

            float range = 400f;
            if (Timer % 2 == 0)
            {
                Vector2 point = Projectile.Center + Main.rand.NextVector2Circular(range, range);
                Vector2 velocity = Projectile.Center - point;
                velocity *= 0.1f;
                DustParticle dp = Particle<DustParticle>.Spawn(point, velocity);
                dp.outerColor = Color.Blue;
                dp.gravity = 0;
                dp.dampening = 0.1f;
            }
            if (Timer % 2 == 0)
            {
                range *= 2f;
                Vector2 point = Projectile.Center + Main.rand.NextVector2Circular(range, range);
                Vector2 velocity = Projectile.Center - point;
                velocity *= 0.1f;
                DustParticle dp = Particle<DustParticle>.Spawn(point, velocity);
                dp.innerColor = Color.Blue;
                dp.outerColor = Color.Blue;
                dp.gravity = 0;
                dp.dampening = 0.1f;
            }
            CombatMechanicsHelper.CreateEnemySuckingEffect(Projectile.Center, strength: 4, radius: range);
            Projectile.rotation = MathHelper.ToRadians(24 - (Timer / 180f) * 32);
            Projectile.scale = MathHelper.Lerp(0.2f, 1f, EasingFunction.InOutSine(Timer / 360f)) * MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
            DrawHelper.UpdateFrame(ref _incresionDiskFrameBottom, 0.8f, 1, 40);
            DrawHelper.UpdateFrame(ref _incresionDiskFrameTop, 0.8f, 1, 76);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            int numDust = 24;
            for (int n = 0; n < numDust; n++)
            {
                Vector2 center = Projectile.Center + Main.rand.NextVector2Circular(128, 128);
                SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(center, -Vector2.UnitY * Main.rand.NextFloat(0.5f, 1f), Color.DarkGray, Main.rand.NextFloat(0.5f, 5f));
                sp.initialColor = Color.DarkGray;
                sp.fadeToColor = Color.Black;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<EventHorizonBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        private void DrawMuzzleFlash(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Asset<Texture2D> muzzleFlashTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash");
            Vector2 drawOrigin = muzzleFlashTexture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            Color drawColor = Color.Violet;
            drawColor.A = 0;

            float outEase = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / 180f));
            drawColor *= outEase;
            drawColor *= 0.74f;
            float width = (float)Projectile.timeLeft / 30f;
            float outWidth = EasingFunction.InOutSine(width);
            float scale = outWidth;
            Vector2 flashScale = Vector2.One;
            flashScale.X *= 0.75f;
            flashScale.Y *= 4f * ExtraMath.Osc(0.5f, 1f, speed: 3f);
            flashScale *= scale;
            spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale, SpriteEffects.None, 0);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(RenderPixelatedSingularity, DrawLayer.OverNPCsWithOutline);
            return false;
        }

        private void RenderPixelatedSingularity(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Vector2 drawPosition = Projectile.Center - screenPos;
            Color ringDrawColor = Color.White;

            ringDrawColor *= 0.15f;
            ringDrawColor.A = 0;
            //  spriteBatch.Draw(celestialRing, drawPosition, null, ringDrawColor, Projectile.rotation, ringDrawOrigin, 4, SpriteEffects.None, 0);


            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = Projectile.scale * Vector2.One;

            float spinRotOffset = Timer * -0.01f;
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Main.DiscoColor;
            sparkyShader.Distortion = -0.15f;
            sparkyShader.Time = -Main.GlobalTimeWrappedHourly * 40;
            sparkyShader.Tiling = Vector2.One * 2;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: sparkyShader.Effect);


            var lightTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 lightDrawOrigin = lightTexture.Size() / 2f;

            float sparkyRot = Projectile.rotation + spinRotOffset;
            float scaleOsc2 = ExtraMath.Osc(0.4f, 0.5f, speed: 1);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.BlueViolet * 0.75f, sparkyRot, lightDrawOrigin, drawScale * 3 * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.BlueViolet * 0.25f, sparkyRot + 0.2f, lightDrawOrigin, drawScale * 8 * scaleOsc2, SpriteEffects.None, 0);


            var shader = SingularityShader.Instance;
            shader.OuterColor = Color.Violet;
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawPosition, null, Color.White, Projectile.rotation, drawOrigin, drawScale * 1.5f * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();

            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Purple, 0.15f), ExtraMath.Osc(0f, 1f, speed: 2));
            diskDrawColor.A = 0;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.65f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, Projectile.rotation, diskDrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, Projectile.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc, SpriteEffects.None, 0);

            diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF2").Value;
            for (float f = 0; f < 4; f++)
            {

                spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, Projectile.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(1.5f, 0.2f), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, Projectile.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(3.5f, 0.2f), SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, Projectile.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(7.5f, 0.2f), SpriteEffects.None, 0);


            Texture2D extra67 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_67").Value;
            Vector2 extra67DrawOrigin = extra67.Size() / 2f;
            Color extra67DrawColor = Color.Lerp(Color.White, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 2));
            extra67DrawColor.A = 0;
            spriteBatch.Draw(extra67, drawPosition, null, extra67DrawColor, Projectile.rotation, extra67DrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            DrawIncresionDiskBottom(spriteBatch, screenPos, Color.White);
            DrawIncresionDiskTop(spriteBatch, screenPos, Color.White);
        }
        private void DrawIncresionDiskBottom(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameBottom, columns: 5, frameWidth: 400, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Disk").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = Projectile.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            float drawScale = Projectile.scale  * 0.75f;
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.Violet;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, Projectile.rotation, drawOrigin, drawScale * 1.5f, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.Purple;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, Projectile.rotation, drawOrigin, drawScale * 2, SpriteEffects.None, 0);
        }


        private void DrawIncresionDiskTop(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameTop, columns: 4, frameWidth: 480, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Top").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.15f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = Projectile.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;

            float drawScale = Projectile.scale * 1.5f;
            float drawRotation = Projectile.rotation;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

    }
}
