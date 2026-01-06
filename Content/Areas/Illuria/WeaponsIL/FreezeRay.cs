using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class Frosting : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
            npc.lifeRegen -= 15;
            if(!npc.boss)
                npc.velocity.X *= 0.7f;
        }
    }

    
    public class IceExplosion : ModProjectile
    {
        private IcicleSystem _icicleSystemBackingField;
        private IcicleSystem IcicleSystem
        {
            get
            {
                if (_icicleSystemBackingField == null)
                {
                    _icicleSystemBackingField = new IcicleSystem(16, 16);
                }
                return _icicleSystemBackingField;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 15;
            Projectile.friendly = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                ShakeModSystem.Shake = 4;
                int rand = Main.rand.Next(0, 2);
                SoundStyle soundStyle;
                switch (rand)
                {
                    default:
                    case 0:
                        soundStyle = AssetRegistry.Sounds.Illuria.IceImpact1;
                        break;
                    case 1:
                        soundStyle = AssetRegistry.Sounds.Illuria.IceImpact2;
                        break;
                }
                soundStyle.PitchVariance = 0.3f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                for (float f = 0; f < 2; f++)
                {
                    Vector2 initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    initialVelocity *= 6;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                    DustParticle dustParticle = Particle<DustParticle>.Spawn(Projectile.Center, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.2f, 0.5f));
                    dustParticle.innerColor = Color.SkyBlue;
                    dustParticle.outerColor = Color.Violet;
                }


                for (float f = 0; f < 2; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(360));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);

                    
                    SmokeParticle smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + initialVelocity,
                        initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 1.3f));
                    smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.14f);
                    smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                    smokeParticle.fadeToColor = Color.Black;
                }

            }
            float time = Timer / 15f;
            time = EasingFunction.OutCirc(time);

            float outInterp = (float)Projectile.timeLeft / 5f;
            float outScale = EasingFunction.InOutSine(outInterp);
            IcicleSystem.Update(Projectile.Center, Projectile.velocity, time, MathHelper.TwoPi / 16f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            IceRenderer.QueueDrawAction(DrawPixelIcicles);
            return false;
        }

        private void DrawPixelIcicles(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            IcicleSystem.Draw(spriteBatch, screenPos);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
         //   FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGray, Color.Black, 25f);
            for (float f = 0; f < 8; f++)
            {
                Vector2 initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                initialVelocity *= 6;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(360));
                initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                DustParticle dustParticle = Particle<DustParticle>.Spawn(Projectile.Center, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.2f, 2f));
                dustParticle.innerColor = Color.SkyBlue;
                dustParticle.outerColor = Color.Violet;
            }
        }
    }
    public class IceStatue : ModProjectile
    {
        private bool _spawnEffect;
        private int NPCType => (int)Projectile.ai[0];
        private ref float Direction => ref Projectile.ai[1];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            base.AI();
 
            var textureAsset = TextureAssets.Npc[NPCType];
            int frameCount = Main.npcFrameCount[NPCType];
            Rectangle frame = new Rectangle(0, 0, textureAsset.Width(), textureAsset.Height() / frameCount);
            Projectile.width = frame.Width;
            Projectile.height = frame.Height;
            Projectile.velocity.Y += 0.2f;

            if (!_spawnEffect)
            {
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGray, Color.Black, 25f);
                _spawnEffect = true;
            }

            foreach(var player in Main.ActivePlayers)
            {
                float distanceToPlayer = Vector2.Distance(Projectile.Center, player.Center);
                if(distanceToPlayer <= 64)
                {
                    Smash();
                    Projectile.Kill();
                }
            }
        }


        private void Smash()
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY, ModContent.ProjectileType<IceExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

     
        public override bool PreDraw(ref Color lightColor)
        {

            SimpleDraw(ref lightColor);
            return false;
        }
        private void SimpleDraw(ref Color lightColor)
        {

            var textureAsset = TextureAssets.Npc[NPCType];
            int frameCount = Main.npcFrameCount[NPCType];
            Rectangle frame = new Rectangle(0, 0, textureAsset.Width(), textureAsset.Height() / frameCount);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            drawColor = drawColor.MultiplyRGB(lightColor);
            Vector2 drawOrigin = frame.Size() / 2f;
            FrozenShader frozenShader = FrozenShader.Instance;
            frozenShader.TintColor = Color.LightCyan;

            SpriteEffects spriteEffects = Direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Restart(effect: frozenShader.Effect);
            spriteBatch.Draw(textureAsset.Value, drawCenter, frame, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            spriteBatch.RestartDefaults();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }

    /// <summary>
    /// Prevents enemies being frozen from spawning gores
    /// </summary>
    public class FreezeRayNoGore : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_NPC.VanillaHitEffect += PreventVanillaHitEffect;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_NPC.VanillaHitEffect -= PreventVanillaHitEffect;
        }

        private void PreventVanillaHitEffect(On_NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg, bool instantKill)
        {
            int frosting = ModContent.BuffType<Frosting>();
            if (self.HasBuff(frosting))
                return;
            orig(self, hitDirection, dmg, instantKill);
        }
    }


    public class FreezeRayNPC : GlobalNPC
    {

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            int frostingType = ModContent.BuffType<Frosting>();
            if (npc.HasBuff(frostingType))
            {

                return;
            }
            base.HitEffect(npc, hit);

        }
        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            int frostingType = ModContent.BuffType<Frosting>();
            if (npc.HasBuff(frostingType))
            {
                //Create Ice Statue here
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
                    ModContent.ProjectileType<IceStatue>(), 100, 1, Main.myPlayer, ai0: npc.type, ai1: -npc.spriteDirection);
            }
        }
    }

    public class FreezeBeam : ModProjectile
    {
        //Don't change the sample points, 3 is good enough
        private const int NumSamplePoints = 3;

        private const float MaxBeamLength = 2400f;

        public float BeamLength;
        public List<Vector2> BeamPoints;

        //No texture for this
        public override string Texture => TextureRegistry.EmptyTexture;

        ref float Size => ref Projectile.ai[0];
        float Timer;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;
            BeamPoints = new List<Vector2>();
        }

        public override void AI()
        {
            float targetBeamLength = PerformBeamHitscan();
            BeamLength = targetBeamLength;
            Timer++;

            if(Timer == 10)
            {
                for(int i = 0; i < BeamPoints.Count; i++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 point = BeamPoints[i];
                        var dp = Particle<DustParticle>.Spawn(point, Projectile.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(0.25f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 2f));
                        dp.gravity = 0.02f;
                        dp.outerColor = Color.Cyan;
                        dp.dampening = 0.1f;
                    }
                }
            }
            if (Timer == 1)
            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + direction * BeamLength;
                for (float f = 0; f < 2; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 12;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                    DustParticle dustParticle = Particle<DustParticle>.Spawn(explosionCenter, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 2f));
                    dustParticle.innerColor = Color.SkyBlue;
                    dustParticle.outerColor = Color.Violet;
                }
                for (float f = 0; f < 6; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 12;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(360));
                    initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                    SparkParticle dustParticle = LegacyParticle.NewParticle<SparkParticle>(explosionCenter, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 2f));
                    dustParticle.innerColor = Color.SkyBlue;
                    dustParticle.outerColor = Color.Violet;
                }

                for (float f = 0; f < 6; f++)
                {
                    Vector2 initialVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);

                    SmokeParticle smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(explosionCenter + initialVelocity,
                        initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 1.3f));
                    smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.4f);
                    smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                    smokeParticle.fadeToColor = Color.Black;
                }

                FXUtil.GlowCircleBoom(explosionCenter,
                    innerColor: Color.White,
                    glowColor: Color.LightSkyBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: 0.06f);

                for (float f = 0; f < 3f; f++)
                {
                    float progress = f / 3f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                    var particle = FXUtil.GlowStretch(explosionCenter, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.LightCyan;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                    particle.VectorScale *= 0.5f;

                }
                var sear = LegacyParticle.NewParticle<SearParticle>(explosionCenter, Vector2.Zero);
                sear.innerColor = Color.Cyan;
                sear.outerColor = Color.Blue;
                sear.Rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

                if (this.OwnedByLocalClient())
                {
                    Vector2 initialVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                    initialVelocity *= 2;


                    int steps = Main.rand.Next(2, 12);
                    Vector2 velocity = initialVelocity.RotatedByRandom(0.6f);
                    Vector2 icicleCenter = explosionCenter;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), icicleCenter,
                        velocity, ModContent.ProjectileType<IcicleFormation>(), 1, 1, Projectile.owner, ai1: steps, ai2: -1);

                }



            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Vector2 initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            initialVelocity *= 2;


            int steps = Main.rand.Next(2, 3);
            Vector2 velocity = initialVelocity.RotatedByRandom(0.6f);
            Vector2 icicleCenter = target.Center;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), icicleCenter,
                velocity, ModContent.ProjectileType<IcicleFormation>(), 1, 1, Projectile.owner, ai1: steps, ai2: target.whoAmI);
            target.AddBuff(ModContent.BuffType<Frosting>(), 360);

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f * Size;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        private float PerformBeamHitscan()
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = Projectile.Center;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[NumSamplePoints];


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;
            return averageLengthSample;
        }


        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);
            float w = MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
            return (Projectile.width * Projectile.scale) * osc * 2 * w;
        }
        public float WidthFunction2(float completionRatio)
        {
            return WidthFunction(completionRatio) * 0.5f;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }
        public Color ColorFunction2(float completionRatio)
        {
            return Color.White;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMuzzleFlash);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated, DrawLayer.OverNPCsWithOutline);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void DrawPixelatedMuzzleFlash(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D> muzzleFlashTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash");
            Vector2 drawOrigin = muzzleFlashTexture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            Color drawColor = Color.Cyan;
            drawColor.A = 0;

            float width = (float)Projectile.timeLeft / 30f;
            float outWidth = EasingFunction.InOutSine(width);
            float scale = outWidth;
            Vector2 flashScale = Vector2.One;
            flashScale.X *= 1.5f;
            flashScale.Y *= 1.2f;
            flashScale *= scale;
            spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale, SpriteEffects.None, 0);
           
            drawColor = Color.White;
            drawColor.A = 0;
            spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale * 0.6f, SpriteEffects.None, 0);

            Asset<Texture2D> impactTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/ZuiEffect");
            drawOrigin = impactTexture.Size() / 2f;

            Vector2 impactPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
            drawCenter = impactPoint - screenPos;
            drawColor = Color.Cyan;
            drawColor.A = 0;
            spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, scale * 1.2f, SpriteEffects.None, 0);

            drawColor = Color.White;
            drawColor.A = 0;
            spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, scale * 0.8f, SpriteEffects.None, 0);
        }
        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            //Put in the points
            //This is just a straight beam that collides with tiles
            BeamPoints.Clear();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);

            float numPoints = 64;
            for (int i = 0; i <= numPoints; i++)
            {
                Vector2 start = Projectile.Center;
                BeamPoints.Add(Vector2.Lerp(start, start + direction * (BeamLength ), i / numPoints));
            }

            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Lerp(Color.Cyan, Color.Blue, 0.75f); 
            shader.OuterColor = Color.Cyan;
            TrailDrawer.Draw(Main.spriteBatch, BeamPoints.ToArray(), ColorFunction, WidthFunction, shader);
        }
    }
    public class FreezeRay : BaseGun
    {
        public override void SetDefaults()
        {
            //  base.SetDefaults();
            remainingAmmo = 16;
            maxAmmo = 16;
            reloadWindow = 30;
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FreezeBeam>();
            Item.shootSpeed = 15;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankGun>(),
                material: ModContent.ItemType<IllurineScale>());
        }


       
        public override void ShootEffects(Vector2 position, Vector2 velocity)
        {
            //base.ShootEffects(position, velocity);
            int rand = Main.rand.Next(0, 3);
            SoundStyle shootSound;// = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7");
            switch (rand)
            {
                default:
                case 0:
                    shootSound = AssetRegistry.Sounds.Illuria.SlushShot1;
                    break;
                case 1:
                    shootSound = AssetRegistry.Sounds.Illuria.SlushShot2;
                    break;
                case 2:
                    shootSound = AssetRegistry.Sounds.Illuria.SlushShot3;
                    break;
            }

            shootSound.PitchVariance = 0.3f;
            shootSound.Volume = 0.5f;
            SoundEngine.PlaySound(shootSound, position);

            FXUtil.GlowCircleBoom(position, Color.White, Color.SkyBlue, Color.Blue, baseSize: 0.03f, duration: 15);

            for (float f = 0; f < 3; f++)
            {
                float rot = f / 8f;
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                var p = LegacyParticle.NewParticle<ImpactParticle>(position, velocity.RotatedByRandom(0.7f));
                p.fast = true;
                p.color = Color.SkyBlue;
            }

            for(float f = 0; f < 5;f++)
            {
                var dp = Particle<DustParticle>.Spawn(position, velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(0.25f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 2f));
                dp.gravity = 0.02f;
                dp.outerColor = Color.Cyan;
                dp.dampening = 0.1f;
            }
        }
    }
}
