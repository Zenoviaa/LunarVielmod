using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Effects.Primitives;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class BouncingBall : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Parent => ref Projectile.ai[1];
    private float Scale
    {
        get
        {
            float inScale = EasingFunction.InOutSine(Timer / 30f);
            float outScale = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
            return inScale * outScale;
        }
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 28;
        Projectile.height = 28;
        Projectile.hostile = false;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }


    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 12 == 0)
        {
            var d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Pearlsand);
            Main.dust[d].noGravity = true;
        }

        if(Timer >= 90)
        {
            Projectile.hostile = true;
        }
        if (Timer % 6 == 0)
        {
            var d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                ModContent.DustType<SeafloorRockDust>());
            Main.dust[d].noGravity = true;
        }

        if (Timer % 8 == 0)
        {
            Vector2 velocity = (Projectile.position - Projectile.oldPosition);
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var bp = BubbleParticle.Spawn(pos, -velocity * 0.25f);
            bp.Scale *= Main.rand.NextFloat(0.3f, 0.6f);
            bp.gravity = 0;
        }

        if(Timer % 30 == 0)
        {
            ElectricZapParticle.Spawn(
                Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(0.3f, 0.6f));
        }
        Projectile.rotation += 0.1f;
        Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 0.4f);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(1f, 0f, ratio) * 64 * Scale;
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Transparent, ratio) * 0.66f;
    }

    private void DrawPixelatedTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.LightBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
        _outlineTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Outline");
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            SpritebatchDrawer e = SpritebatchDrawer.FromProjectile(Projectile);
            e.worldPosition = pos;
            e.color = Color.Lerp(Color.White, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.05f;
            e.scale = Vector2.One * Scale;
            Main.spriteBatch.Draw(e);
        }
        SpritebatchDrawer ballDraer = SpritebatchDrawer.FromProjectile(Projectile);
        ballDraer.scale = Vector2.One * Scale;
        Main.spriteBatch.Draw(ballDraer);

        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlineDrawer.color = Projectile.hostile ? Color.Red : Color.Yellow;
        outlineDrawer.scale = Vector2.One * Scale;
        outlineDrawer.rotation = ballDraer.rotation;
        Main.spriteBatch.Draw(outlineDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 32; f++)
        {
            var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                ModContent.DustType<SeafloorRockDust>(), Main.rand.NextVector2Circular(16, 16), Scale: 2);
            d.noGravity = true;
        }
    }
}
public class BouncingBallCore : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Slavery => ref Projectile.ai[1];
    private ref float BounceTimer => ref Projectile.ai[2];
    private Vector2 _shakeOffset;
    private Vector2[] _offsets;
    private Projectile[] _children;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = false;
        Projectile.timeLeft = 700;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall3");
            SoundEngine.PlaySound(explosionSound, Projectile.position);
            LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.2f);


     

            for (float f = 0; f < 16; f++)
            {
                Vector2 vel = Vector2.UnitY * Main.rand.NextFloat(8, 35f);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(45));
                var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                    ModContent.DustType<SeafloorRockDust>(), vel, Scale: 2);
                var dp = DustParticle.Spawn(Projectile.Center, vel);
                dp.outerColor = Color.Blue;
                dp.Scale *= 0.5f;
                d.noGravity = true;
            }
        }

        if(Timer == 100)
        {
            if(Slavery == 1)
            {
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
                SoundEngine.PlaySound(explosionSound, Projectile.position);
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                for (float f = 0; f < 32; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.innerColor = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(0f, 1f));
                    spawnParams.outerColor = Color.Turquoise;
                    var d = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                    d.dampening = 0.05f;
                    d.gravity = 0;
                    d.noTileCollide = true;
                    d.Scale *= 1.5f;

                }
            }
            if (this.OwnedByLocalClient() && Slavery == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<ZapShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
            }

            for (float f = 0; f < 16; f++)
            {
                var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                    ModContent.DustType<SeafloorRockDust>(), Main.rand.NextVector2Circular(16, 16), Scale: 2);
                d.noGravity = true;
            }
            for(float f = 0; f < 8f; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                FXUtil.GlowStretch(Projectile.Center + vel * 0.5f, vel);
            }

            if (this.OwnedByLocalClient())
            {
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<BouncingBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Slavery);
            }
        }

        if (Timer < 100)
        {
            if (Timer % 30 == 0)
            {
                ElectricZapParticle.Spawn(
                    Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                    Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(0.3f, 0.6f));
            }
            ShakeScreenPosition.Shake = MathHelper.Lerp(0f, 4f, Timer / 100f);
            if (Timer % 4 == 0)
            {
                _shakeOffset = Main.rand.NextVector2Circular(4, 4);
            }
            Projectile.velocity *= 0.97f;
            Projectile.rotation += 0.05f;
            return;
        }

        float bounceTime = 90f;
        BounceTimer++;
        if (BounceTimer >= bounceTime)
        {
            SoundStyle hammerHit = AssetRegistry.Sounds.Melee.HammerHit1;
            if (Main.rand.NextBool(2))
                hammerHit = AssetRegistry.Sounds.Melee.HammerHit2;
            hammerHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(hammerHit, Projectile.position);
            if (this.OwnedByLocalClient() && Slavery == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, 
                    ModContent.ProjectileType<ZapShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
            }

            var gd = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero);
            gd.noStretch = true;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            for (float f = 0; f < 16; f++)
            {
                var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                    ModContent.DustType<SeafloorRockDust>(), Main.rand.NextVector2Circular(16, 16), Scale: 0.5f);
                d.noGravity = true;
            }
            BounceTimer = 0f;
        }
        float dir = Slavery == 1 ? -1 : 1;
        _children ??= new Projectile[2];
        _offsets ??= new Vector2[2];
        for (int i = 0; i < _offsets.Length; i++)
        {
            ref Vector2 offset = ref _offsets[i];
            Vector2 maxOffset = -Vector2.UnitY * 512;
            maxOffset = maxOffset.RotatedBy(Timer * 0.05f * dir);

            float halfTime = bounceTime / 2f;
            float inEasing = BounceTimer / halfTime;
            inEasing = EasingFunction.OutExpo(inEasing);
            float outEasing = (BounceTimer - halfTime) / halfTime;
            outEasing = EasingFunction.InExpo(outEasing);
            float mixedEasing = inEasing * MathHelper.Lerp(1f, 0f, outEasing);
            if (i == 1)
                maxOffset *= -1;
            offset = Vector2.Lerp(Vector2.Zero, maxOffset, mixedEasing);

        }


        Player nearestPlayer = PlayerHelper.FindClosestPlayer(Projectile.Center, 2048);
        if (nearestPlayer != null)
        {
            Vector2 velToPalyer = (nearestPlayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, velToPalyer * 25, 0.008f);
        }

    
        Projectile.rotation += 0.05f * dir;
        Projectile.rotation += Projectile.velocity.Length() * 0.02f;

        int index = 0;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (index >= _offsets.Length)
                break;
            if (proj.type != ModContent.ProjectileType<BouncingBall>())
                continue;
            if (proj.ai[1] != Slavery)
                continue;

            Vector2 targetPos = Projectile.Center + _offsets[index];
            Vector2 vel = targetPos - proj.Center;
            proj.velocity = vel;
            _children[index] = proj;
            index++;
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = oldVelocity.X * -1;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = oldVelocity.Y * -1;
        return false;
    }

    private float GetTrailWidth(float ratio)
    {
        return 8;
    }

    private Color GetTrailColor(float ratio)
    {
        return new Color(45, 54, 57);
    }

    private void DrawPixelatedThornTrail(GraphicsDevice gDevice)
    {
        if (_children == null)
            return;

        for (int i = 0; i < _children.Length; i++)
        {
            float numPoints = 32;
            Vector2 start = Projectile.Center;
            Vector2 end = _children[i].Center;
            Vector2[] points = CommonDrawing.InterpolateBetweenPoints(start, end, numPoints);

            HairShader shader = ShaderContent.GetInstance<HairShader>();
            shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
            shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
            shader.WaveFrequency = 8;
            shader.XOffset = 12;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor, GetTrailWidth, shader);
        }
    }

    private void DrawWhites(SpriteBatch sb)
    {
        SpritebatchDrawer ballCoreDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        if (Timer < 100f)
        {
            ballCoreDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<BouncingBall>()], Projectile.Center);
            ballCoreDrawer.rotation = Projectile.rotation;
        }
        ballCoreDrawer.color = Color.Yellow;
        ballCoreDrawer.scale = Vector2.One * EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        ballCoreDrawer.worldPosition += _shakeOffset;
        sb.Draw(ballCoreDrawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedThornTrail);
        SpritebatchDrawer ballCoreDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        if(Timer < 100f)
        {
            OutlineRenderer.Queue(DrawWhites);
            ballCoreDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<BouncingBall>()], Projectile.Center);
            ballCoreDrawer.rotation = Projectile.rotation;
        }
        ballCoreDrawer.scale = Vector2.One * EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        ballCoreDrawer.worldPosition += _shakeOffset;
        Main.spriteBatch.Draw(ballCoreDrawer);
        return false;
        //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
