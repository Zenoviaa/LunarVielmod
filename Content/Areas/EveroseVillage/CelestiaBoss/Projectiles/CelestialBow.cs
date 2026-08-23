using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.EveroseVillage.CelestiaBoss.Projectiles;


public class CelestialBow : ModProjectile
{
    private Vector2 _mirageOffset;
    private Vector2 _pullScale;
    private Vector2 _targetPullScale;
    private int _frameCounter;
    private ref float Timer => ref Projectile.ai[0];
    private Player Target => Main.player[(int)Projectile.ai[1]];
    private ref float AttackTimer => ref Projectile.ai[2];
    public int style;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(style);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        style = reader.ReadInt32();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 7;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 180;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            _targetPullScale = Vector2.One;
            var p = MoonSpiralParticle.Spawn(Projectile.Center, Vector2.Zero);
            p.color = Color.Teal;
            for (float f = 0; f < 8; f++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(128, 128);
                Vector2 vel = (Projectile.Center - spawnPos);
                vel *= 0.1f;
                var fx = FXUtil.GlowStretch(spawnPos, vel);
                fx.OuterGlowColor = Color.Green;
                fx.VectorScale *= 0.4f;
            }

        }

        if (Timer % 4 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
            var d = Dust.NewDustPerfect(pos, DustID.GemEmerald, Scale: 1f);
            d.noGravity = true;
        }

        if (Timer >= 0)
        {
            //Ready bow line
            AttackTimer++;
            if (AttackTimer == 10 || AttackTimer == 20 || AttackTimer == 30)
            {

                _targetPullScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.8f), (Projectile.frame + 1) / 4f);
                Projectile.frame = (int)Math.Floor(AttackTimer / 10f);
            }



            if (AttackTimer > 60)
            {
                if (this.OwnedByLocalClient() && AttackTimer == 70)
                {
                    int projStyle = 0;
                    if (style == 1)
                        projStyle = 2;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation.ToRotationVector2(),
                        ModContent.ProjectileType<CelestialArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Target.whoAmI, ai2: projStyle);
                }
                if (AttackTimer == 70 || AttackTimer == 80 || AttackTimer == 90)
                {

                    _targetPullScale = Vector2.One;
                    Projectile.frame++;
                }

            }
        }


        if (Timer % 4 == 0)
        {
            //Visual effect purely, doesn't need to be net synced.
            _mirageOffset = Main.rand.NextVector2Circular(3, 3);
        }
        Projectile.velocity.X *= 0.94f;
        if (Timer < 5)
        {
            Projectile.velocity.Y -= 0.05f;
        }
        else
        {
            Projectile.velocity.Y *= 0.94f;
        }

        _pullScale = Vector2.Lerp(_pullScale, _targetPullScale, 0.1f);

        Vector2 aimingDirection = (Target.Center - Projectile.Center);
        float aimingRotation = aimingDirection.ToRotation();
        float rotOffset = MathHelper.Lerp(-MathHelper.Pi + MathHelper.PiOver4, 0, EasingFunction.OutCirc(Timer / 60f));
        Projectile.rotation = aimingRotation + rotOffset;
        if (AttackTimer > 100)
        {
            Projectile.velocity = Projectile.rotation.ToRotationVector2() * 0.2f;
        }
    }

    private void DrawPixelatedBows(SpriteBatch sb, Vector2 screenPos)
    {
        float alpha = EasingFunction.InSine(Timer / 30f);
        alpha *= (float)(EasingFunction.Clamp(Projectile.timeLeft / 30f));
        Vector2 pullScale = _pullScale;
        pullScale *= MathHelper.Lerp(1.45f, 1f, EasingFunction.InSine(Timer / 60f));


        float come = EasingFunction.InSine(Timer / 70f);
        Vector2 inOffset = Vector2.Lerp(-Projectile.rotation.ToRotationVector2() * 128, Vector2.Zero, come);

        SpritebatchDrawer backGlowDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow"), Projectile.Center); ;
        backGlowDrawer.scale *= pullScale * 2;
        backGlowDrawer.color = Color.Black * 0.5f * alpha;
        //  glowDrawer.color.A = 0;
        backGlowDrawer.worldPosition += inOffset;
        Main.spriteBatch.Draw(backGlowDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center); ;
        glowDrawer.scale *= pullScale * 0.5f;
        glowDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f * alpha;
        glowDrawer.color.A = 0;
        glowDrawer.worldPosition += inOffset;
        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer spiralVortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center); ;
        spiralVortexDrawer.scale *= pullScale * 0.5f;
        spiralVortexDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.1f * alpha;
        spiralVortexDrawer.color.A = 0;
        spiralVortexDrawer.worldPosition += inOffset;
        spiralVortexDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(spiralVortexDrawer);

        SpritebatchDrawer bowDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        bowDrawer.scale *= pullScale;
        bowDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.5f * alpha;
        bowDrawer.color.A = 0;
        bowDrawer.worldPosition += inOffset;
        Main.spriteBatch.Draw(bowDrawer);


        bowDrawer.worldPosition -= Projectile.rotation.ToRotationVector2() * 8;
        bowDrawer.worldPosition += _mirageOffset;
        bowDrawer.color =
            Color.Lerp(Color.DarkTurquoise, Color.DarkGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f * alpha;
        bowDrawer.color.A = 0;
        bowDrawer.scale *= 1.3f;
        Main.spriteBatch.Draw(bowDrawer);


        float lineOut = (AttackTimer - 60f) / 30f;
        lineOut = EasingFunction.InOutSine(lineOut);
        float lineOutAlpha = MathHelper.Lerp(1f, 0f, lineOut);
        SpritebatchDrawer bloomlineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomlineDrawer.color = Color.Teal * come * alpha * lineOutAlpha;
        bloomlineDrawer.color.A = 0;

        float dist = Vector2.Distance(Projectile.Center, Target.Center);
        float bloomLineSize = dist / bloomlineDrawer.texture.Width;
        bloomlineDrawer.scale.X *= bloomLineSize;
        bloomlineDrawer.scale.Y *= 0.025f;
        bloomlineDrawer.LeftCenterOrigin();
        bloomlineDrawer.drawOrigin.X += 64;
        bloomlineDrawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(bloomlineDrawer);

        SpritebatchDrawer arrowDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<CelestialArrow>()], Projectile.Center);
        arrowDrawer.rotation = Projectile.rotation;
        arrowDrawer.scale.Y *= 0.5f;
        arrowDrawer.scale.X *= MathHelper.Lerp(0.5f, 1f, come);
        arrowDrawer.color = Color.LightGreen * come * alpha * lineOutAlpha;
        arrowDrawer.color.A = 0;

        Main.spriteBatch.Draw(arrowDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBows);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class CelestialArrow : ModProjectile
{
    private Vector2 _stretchScale;
    private Vector2 _mirageOffset;
    private ref float Timer => ref Projectile.ai[0];
    private Player Target => Main.player[(int)Projectile.ai[1]];
    private ref float Style => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _stretchScale = Vector2.One;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            _stretchScale = Vector2.One;
            if (Style == 0)
            {
                for (float f = 0; f < 4f; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel = vel.RotatedByRandom(MathHelper.PiOver4 / 2f);
                    vel *= Main.rand.NextFloat(5f, 15f);
                    DustParticle dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel);
                    dp.outerColor = Color.Turquoise;
                    dp.gravity = 0;
                    dp.dampening = 0.05f;
                    dp.noTileCollide = true;
                }
            }


            GlowDonutParticle d = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 2);
            d.outerColor = Color.Turquoise;
            d.fadeToColor = Color.DarkTurquoise;
            d.Scale *= 0.3f;

            GlowDonutParticle d2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 4);
            d2.outerColor = Color.Turquoise;
            d2.fadeToColor = Color.DarkTurquoise;
            d2.Scale *= 0.15f;
            Projectile.velocity *= 8;

            if (Style == 0 || Style == 2)
            {
                SoundStyle shootSound1 = AssetRegistry.Sounds.Celestia.SmallBowShoot1 with { PitchVariance = 0.3f };
                SoundStyle shootSound2 = AssetRegistry.Sounds.Celestia.SmallBowShoot2 with { PitchVariance = 0.3f };
                switch (Main.rand.Next(2))
                {
                    case 0:
                        SoundEngine.PlaySound(shootSound1, Projectile.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(shootSound2, Projectile.position);
                        break;
                }
                //     SoundEngine.PlaySound(backflipSound, NPC.position);
            }
            if (Style == 1)
            {
                Projectile.velocity *= 0.6f;
            }
        }

        if (Timer % 4 == 0)
        {
            //Visual effect purely, doesn't need to be net synced.
            _mirageOffset = Main.rand.NextVector2Circular(3, 3);
        }
        if (Timer % 2 == 0)
        {
            for (float f = 0; f < 3; f++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
                var d = Dust.NewDustPerfect(pos, DustID.GemEmerald, Scale: 1f);
                d.noGravity = true;
            }

        }

        if (Style == 0)
        {
            if (Projectile.velocity.Length() < 30)
                Projectile.velocity *= 1.2f;

            float dotProduct = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), (Target.Center - Projectile.Center).SafeNormalize(Vector2.Zero));
            if (dotProduct > 0)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Target.Center, degreesToRotate: 0.5f);
            }
        }
        else if (Style == 1 || Style == 2)
        {
            if (Projectile.velocity.Length() < 15)
                Projectile.velocity *= 1.1f;

        }


        if (Timer % 6 == 0)
        {
            DustParticle dp = DustParticle.Spawn(Projectile.Center, Projectile.velocity * 0.1f);
            dp.outerColor = Color.Turquoise;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.Scale *= 0.66f;
        }

        if (Timer % 12 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(30) * 0.1f));
            sp.Scale *= 0.5f;
            sp.flickering = true;
            sp.outerColor = Color.Turquoise;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.dampening = 0.05f;
        }
        if (Style == 1)
        {
            if (Projectile.Bottom.Y > Target.Top.Y)
            {
                Projectile.tileCollide = true;
            }
        }
        else
        {
            if (Timer > 10)
            {
                Projectile.tileCollide = true;
            }
        }


        Vector2 targetScale = Vector2.Lerp(Vector2.One, new Vector2(1.5f, 0.6f), Projectile.velocity.Length() / 25f);
        _stretchScale = Vector2.Lerp(_stretchScale, targetScale, 0.1f);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    private void DrawTrails(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.InnerColor = Color.Turquoise;
        laserShader.OuterColor = Color.Turquoise;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Turquoise;
        b.OuterColor = Color.DarkTurquoise;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth2, b, Projectile.Size * 0.5f);
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 2;
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(10, 0, ratio);
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.LightGreen, Color.Turquoise, ratio) * 0.3f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrails);
        SpritebatchDrawer celestialArrowDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        celestialArrowDrawer.scale *= _stretchScale;
        celestialArrowDrawer.color = Color.Lerp(Color.Teal, Color.Turquoise, ExtraMath.Osc(0f, 1f, speed: 6)) * 0.35f;
        celestialArrowDrawer.color.A = 0;
        Main.spriteBatch.Draw(celestialArrowDrawer);


        celestialArrowDrawer.worldPosition -= Projectile.rotation.ToRotationVector2() * 8;
        celestialArrowDrawer.worldPosition += _mirageOffset;
        celestialArrowDrawer.color =
            Color.Lerp(Color.DarkTurquoise, Color.DarkGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f;
        celestialArrowDrawer.color.A = 0;
        celestialArrowDrawer.scale *= 1.3f;
        Main.spriteBatch.Draw(celestialArrowDrawer);

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            pos += Projectile.Size * 0.5f;
            celestialArrowDrawer.worldPosition = pos;
            celestialArrowDrawer.color = Color.Lerp(Color.Turquoise, Color.Black, i / (float)Projectile.oldPos.Length) * 0.1f;
            celestialArrowDrawer.color.A = 0;
            Main.spriteBatch.Draw(celestialArrowDrawer);

        }

        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        flareDrawer.color = Color.Lerp(Color.Turquoise, Color.Black, EasingFunction.InSine(Timer / 30f)) * 0.6f;
        flareDrawer.scale = Vector2.Lerp(Vector2.One * 0.65f, Vector2.Zero, EasingFunction.InSine(Timer / 30f));
        flareDrawer.color.A = 0;
        Main.spriteBatch.Draw(flareDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Turquoise, Color.DarkTurquoise);
        fx.Scale *= 0.66f;
        float numDust = 4;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 vel = -Projectile.velocity;
            vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(6, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Turquoise;
            var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.Scale *= 0.5f;
        }

        for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
        {
            if (Main.rand.NextBool(4))
            {
                Vector2 vel = -(Projectile.oldPos[i] - Projectile.oldPos[i + 1]);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(25));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(2, 7);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.LightGreen;
                spawnParams.outerColor = Color.Turquoise;
                spawnParams.scaleRange *= 0.66f;
                var dp = DustParticle.Spawn(Projectile.oldPos[i] + Projectile.Size * 0.5f, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.gravity = 0;

            }
        }

        if (Style == 1)
        {
            SoundStyle arrowRainHit = AssetRegistry.Sounds.Celestia.ArrowRainArrowhitground with { PitchVariance = 0.4f };
            SoundEngine.PlaySound(arrowRainHit, Projectile.position);
        }
    }
}

