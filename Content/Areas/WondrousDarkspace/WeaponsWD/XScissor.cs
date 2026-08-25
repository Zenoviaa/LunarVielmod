using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class XScissor : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 15;
        Item.shoot = ModContent.ProjectileType<XScissorSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<XScissorStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Dualsword;
        staminaDamageMultiplier=3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<
            HypnotizedSoul,
            BlankSword>();
    }
}

public class XScissorCut : ModProjectile
{
    private float _scale;
    private ref float Timer => ref Projectile.ai[0];
    private float Time => 80;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 7;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.localNPCHitCooldown = (int)(Time / 2);
        Projectile.timeLeft = (int)Time;
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
            _scale = 1f;
            SoundStyle curveSound = AssetRegistry.Sounds.Melee.Crosshatchcut;
            curveSound.PitchVariance = 0.3f;
            curveSound.Volume = 0.5f;
            SoundEngine.PlaySound(curveSound, Projectile.position);
            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi);
                vel *= Main.rand.NextFloat(10, 15);
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, vel, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.innerColor = Color.White;
                dp.outerColor = Color.Pink;
            }
            for (float f = 0; f < 2; f++)
            {
                float direction = MathHelper.Lerp(-1f, 1f, (f + 1) / 2f);
                var fx = FXUtil.GlowStretch(Projectile.Center, Vector2.UnitY.RotatedBy(MathHelper.PiOver4) * direction * 16);
                fx.VectorScale.X *= 16;
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 16);
            if (MultiplayerHelper.IsHost)
            {
                ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.White, 0.2f, 15);
            }
        }

        if(Timer % 8 == 0 && Timer < 15)
        {
            for (float f = 0; f < 2; f++)
            {
                float direction = MathHelper.Lerp(-1f, 1f, (f + 1) / 2f);
                Vector2 vel = Vector2.UnitY.RotatedBy(MathHelper.PiOver4) * direction * 16;
                vel = vel.RotatedByRandom(MathHelper.ToRadians(55));
                var fx = FXUtil.GlowStretch(Projectile.Center, vel);
                fx.VectorScale.X *= 16;
            }
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.frameCounter++;
        float frameSpeed = 5;
        if (Projectile.frame == 2)
            frameSpeed = 20;
        else if (Projectile.frame > 2)
        {
            frameSpeed = 3;
        }
         
        if (Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.Kill();
            }
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    private void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        Vector2 offset = Vector2.Zero;
        offset = Vector2.Lerp(new Vector2(1, -1) * 32, Vector2.Zero, EasingFunction.OutExpo(Timer / Time));
        float alpha = EasingFunction.OutExpo(Timer / 30f);

        drawer.worldPosition += offset;
        drawer.color = Color.White;
        drawer.color = Color.Lerp(drawer.color, Color.Black, EasingFunction.InOutSine(Timer / Time)) * alpha;
        drawer.color.A = 0;
        drawer.scale = Vector2.Lerp(Vector2.One, Vector2.Zero, EasingFunction.InExpo(Timer / Time)) * 0.65f * _scale;
        Main.spriteBatch.Draw(drawer);


        drawer.color = Color.Lerp(Color.Pink, Color.LightPink, ExtraMath.Osc(0f, 1f, speed: 16));
        drawer.color = Color.Lerp(drawer.color, Color.Black, EasingFunction.InOutSine(Timer / Time) * 3) * alpha;
        drawer.color.A = 0;
        drawer.scale *= 1.5f;
        Main.spriteBatch.Draw(drawer);


        drawer.color = Color.Lerp(Color.Purple, Color.MediumPurple, ExtraMath.Osc(0f, 1f, speed: 16));
        drawer.color = Color.Lerp(drawer.color, Color.Black, EasingFunction.InOutSine(Timer / Time) * 3) * alpha;
        drawer.color.A = 0;
        drawer.scale *= 1.1f;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.White;
        drawer.color = Color.Lerp(drawer.color, Color.Black, EasingFunction.InOutSine(Timer / Time)) * alpha * EasingFunction.QuadraticBump(Timer / 15);
        drawer.color.A = 0;
        drawer.scale = Vector2.Lerp(Vector2.One, Vector2.Zero, EasingFunction.InExpo(Timer / Time)) * 0.65f * _scale;
        drawer.scale *= 2f;
        Main.spriteBatch.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated, DrawLayer.OverNPCs);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class XScissorMiniSlash : ModProjectile
{
    private Vector2[] RiftPoints = new Vector2[32];
    private ref float Timer => ref Projectile.ai[0];
    private ref float RandScale => ref Projectile.ai[1];
    private bool IsLong => Projectile.ai[2] == 1;
    private float Interpolant;
    public override string Texture => TextureRegistry.EmptyTexture;
    private float Time => 40;
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
        Projectile.timeLeft = (int)Time;
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
                for(float f = 0; f < 10; f++)
                {
                    Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi);
                    vel *= Main.rand.NextFloat(10, 15);
                    DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, vel, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                    dp.innerColor = Color.White;
                    dp.outerColor = Color.Pink;
                }
                FXUtil.ShakeCamera(Projectile.Center, 1024, 16);
                if (MultiplayerHelper.IsHost)
                {
                    ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.White, 0.2f, 15);
                }
                Projectile.timeLeft += 60;
            }
            if (this.OwnedByLocalClient())
            {
                RandScale = Main.rand.NextFloat(0.5f, 1f);
            }
        }
        if (Timer % 9 == 0)
        {
            DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Projectile.velocity.RotatedByRandom(4f) * Main.rand.NextFloat(0.1f, 0.5f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            dp.innerColor = Color.Black;
            dp.outerColor = Color.Pink;
        }
        Interpolant = EasingFunction.InExpo(Timer / Time);
        if (IsLong)
        {
            Interpolant = EasingFunction.InExpo(Timer / 260f);
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
        return Color.White;
    }
    private float GetTrailWidth(float completionRatio)
    {
        float baseWidth = EasingFunction.QuadraticBump(completionRatio) * 8;
        if (IsLong)
            baseWidth *= 3;
        float outScale = (float)Projectile.timeLeft / 30f;
        outScale = EasingFunction.InOutSine(outScale);
        float inScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 15f));
        return baseWidth * outScale * inScale;
    }
    private float GetTrailWidth2(float completionRatio)
    {
        return GetTrailWidth(completionRatio) * 1.2f;
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
        shader.LaserColor = Color.White;
        Color innerColor = Color.Lerp(Color.Violet, Color.Pink, 0.75f);
        shader.InnerColor = innerColor;
        shader.OuterColor = Color.Purple;
        if (Timer < 15)
        {
            shader.OuterColor = Color.Lerp(Color.White, Color.Purple, EasingFunction.InOutSine(Timer / 15f));
            shader.InnerColor = Color.Lerp(Color.White, innerColor, EasingFunction.InOutSine(Timer / 15f));
            shader.LaserColor = Color.Lerp(Color.White, Color.Black, EasingFunction.InOutSine(Timer / 15f));
        }
        TrailDrawer.Draw(Main.spriteBatch, RiftPoints, GetTrailColor, GetTrailWidth, shader);


        if (IsLong)
            return;
        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Color.White;
        bloomTrailShader.OuterColor = Color.Purple;
        TrailDrawer.Draw(Main.spriteBatch, RiftPoints, GetTrailColor, GetTrailWidth2, bloomTrailShader);


    }
}

public class XScissorSlash : BaseSwingProjectileV2
{
    private bool _hit;
    private bool _spawnedClone;
    private bool _spawnedCut;
    public override void DefineCombo()
    {
        base.DefineCombo();
        //trailOffsetOverride = 1;
        ComboBuilder comboBuilder = new ComboBuilder();
        if (SwingDirection == 1)
            comboBuilder.AddSwordSlash1(duration: 30, xSwingRadius: 129, ySwingRadius: 32, hitCount: 1, swingDegrees: 210);
        else
            comboBuilder.AddSwordSlash1(duration: 30, xSwingRadius: 100, ySwingRadius: 46, hitCount: 1, swingDegrees: 210);

        if (SwingDirection == 1)
            comboBuilder.AddSwordSlash2(duration: 28, xSwingRadius: 154, ySwingRadius: 46, hitCount: 1, swingDegrees: 175);
        else
            comboBuilder.AddSwordSlash2(duration: 28, xSwingRadius: 122, ySwingRadius: 32, hitCount: 1, swingDegrees: 175);

        if (SwingDirection == 1)
            comboBuilder.AddSwordSlash1(duration: 26, xSwingRadius: 129, ySwingRadius: 32, hitCount: 1, swingDegrees: 210);
        else
            comboBuilder.AddSwordSlash1(duration: 26, xSwingRadius: 100, ySwingRadius: 46, hitCount: 1, swingDegrees: 210);

        if (SwingDirection == 1)
            comboBuilder.AddSwordSlash2(duration: 24, xSwingRadius: 154, ySwingRadius: 46, hitCount: 1, swingDegrees: 175);
        else
            comboBuilder.AddSwordSlash2(duration: 24, xSwingRadius: 122, ySwingRadius: 32, hitCount: 1, swingDegrees: 175);

        comboBuilder.AddChakramUppercut(duration: 21, xSwingRadius: 96, hitCount: 1, swingDegrees: 199);
        comboBuilder.AddSwordSlash3(duration: 48, xSwingRadius: 129, ySwingRadius: 48, hitCount: 1, swingDegress: 276);
        comboBuilder.AddToProjectile(this);


        //   outlineColor = Color.Yellow;
        Trailer = TrailPresets.XScissor;
        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.LightBlue;
        bloom.outerBloomColor = Color.Purple;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        additive = true;
        useAfterImage = true;
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 8, ratio) * 4f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Purple * 0.9f, Color.Lerp(Color.DeepSkyBlue, Color.Violet, ExtraMath.Osc(0f, 1f, speed: 24)), ratio);
    }

    public override void AI()
    {
        base.AI();
        if (Timer % 32 == 0)
        {
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            SparkleParticle dp = SparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.3f);
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.innerColor = Color.White;
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.fast = true;
        }

        float spawnTime = IsFinishingSwing() ? 0.1f : 0.25f;
        if (!_spawnedClone && Interpolant > spawnTime)
        {
          
            MirrorProjectile();
            _spawnedClone = true;
        }

        if (!_spawnedCut && Interpolant > 0.4f)
        {
            if (IsFinishingSwing())
            {
                if (this.OwnedByLocalClient())
                {
                    Vector2 vel = -Vector2.UnitY;
                    vel = vel.RotatedBy(MathHelper.PiOver4 * SwingDirection);
                    vel *= 15;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 128, vel,
                        ModContent.ProjectileType<XScissorMiniSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                Owner.velocity += Projectile.velocity * 0.1f;
            }
            _spawnedCut = true;
        }
        if (Main.rand.NextBool(100))
        {
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.White,
                outerColor = Color.Aquamarine,
                scaleRange = new Vector2(0.4f, 0.6f)
            };
            DustParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
        }
        glowColor = Color.Lerp(Color.Transparent, Color.White * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));

    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            Vector2 position = target.Center;
            Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.White,
                    outerColor: Color.Cyan,
                    fadeToColor: Color.DarkBlue,
                    distortOut: true);
            }
            _hit = true;
        }

        if (ComboIndex == ComboCount - 1)
        {
            SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadianceCast1;
            fireSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(fireSound, Projectile.position);
            for (float f = 0; f < 8; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                EmberParticle ep = LegacyParticle.NewParticle<EmberParticle>(Owner.Center, vel);
                ep.innerColor = Color.White;
                ep.outerColor = Color.Cyan;
                ep.fadeToColor = Color.DarkBlue;
            }

        }
        target.AddBuff(BuffID.Confused, 15);
        target.AddBuff(BuffID.ShadowFlame, 15);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
}


public class XScissorStaminaSlash : BaseSwingProjectileV2
{
    private bool _hit;
    private bool _spawnedClone;
    private bool _spawnedCut;
    public override void DefineCombo()
    {
        base.DefineCombo();
        trailOffsetOverride = 1;
        ComboBuilder comboBuilder = new ComboBuilder();
        comboBuilder.AddSwordSlash2(duration: 40, xSwingRadius: 129, ySwingRadius: 48, hitCount: 1, swingDegrees: 276);
        comboBuilder.AddToProjectile(this);

        //   outlineColor = Color.Yellow;
        if(Main.netMode != NetmodeID.Server)
        {
            Trailer = TrailPresets.XScissor;
            //Bloom
            useBloom = true;
            bloom.innerBloomColor = Color.LightBlue;
            bloom.outerBloomColor = Color.Purple;
            bloom.bloomWidthFunction = GetBloomWidth;
            bloom.bloomColorFunction = GetBloomColor;
            additive = true;
            useAfterImage = true;
        }
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 8, ratio) * 4f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Purple * 0.9f, Color.Lerp(Color.DeepSkyBlue, Color.Violet, ExtraMath.Osc(0f, 1f, speed: 24)), ratio);
    }

    public override void AI()
    {
        base.AI();
        if (Timer % 32 == 0)
        {
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            SparkleParticle dp = SparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.3f);
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.innerColor = Color.White;
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.fast = true;
        }
        if (!_spawnedClone && Interpolant > 0.1f)
        {
            MirrorProjectile();
            _spawnedClone = true;
        }
        if (!_spawnedCut && Interpolant > 0.4f && !isChildProjectile)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            string slash = $"Stellamod/Assets/Sounds/AssassinsSlashProj{Main.rand.Next(2, 5)}";
            SoundStyle sound = new SoundStyle(slash);
            sound.PitchVariance = 0.3f;
         //   SoundEngine.PlaySound(sound, Projectile.position);

            Vector2 pos = Owner.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 128;
            if (IsFinishingSwing())
            {
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Projectile.velocity.SafeNormalize(Vector2.Zero),
                        ModContent.ProjectileType<XScissorCut>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner, ai2: 1);
                }
                Owner.velocity += Projectile.velocity * 0.1f;
                for (float f = 0; f < 8; f++)
                {
                    Vector2 vel = Vector2.One.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 12f);
                    DustParticle dp = DustParticle.Spawn(pos, vel);
                    dp.outerColor = Color.Pink;
                    dp.Scale = Main.rand.NextFloat(0.6f, 1f);
                }
            }

    
            _spawnedCut = true;
        }
        if (Main.rand.NextBool(100))
        {
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.White,
                outerColor = Color.Aquamarine,
                scaleRange = new Vector2(0.4f, 0.6f)
            };
            DustParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
        }
        glowColor = Color.Lerp(Color.Transparent, Color.White * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));

    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            Vector2 position = target.Center;
            Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.White,
                    outerColor: Color.Cyan,
                    fadeToColor: Color.DarkBlue,
                    distortOut: true);
            }
            _hit = true;
        }

        if (ComboIndex == ComboCount - 1)
        {
            SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadianceCast1;
            fireSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(fireSound, Projectile.position);
            for (float f = 0; f < 8; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                EmberParticle ep = LegacyParticle.NewParticle<EmberParticle>(Owner.Center, vel);
                ep.innerColor = Color.White;
                ep.outerColor = Color.Cyan;
                ep.fadeToColor = Color.DarkBlue;
            }

        }
        target.AddBuff(BuffID.Confused, 15);
        target.AddBuff(BuffID.ShadowFlame, 15);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
}