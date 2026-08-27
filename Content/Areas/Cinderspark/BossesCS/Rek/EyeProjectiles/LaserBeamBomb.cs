using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Effects.Generic;
using Stellamod.Effects.RekFlames;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.EyeProjectiles;

public class LaserBeamBomb : ModProjectile,
    IDrawToRenderTarget
{
    private float _bombTimer;
    private Vector2 _initialVelocity;
    private float LifeTime => 45;
    private Asset<Texture2D> BeamTextureAsset => TextureAssets.Projectile[ModContent.ProjectileType<BigVulcanFireball>()];
    private Vector2[] _beamPoints = new Vector2[64];
    private ref float Timer => ref Projectile.ai[0];
    private ref float MaxRadians => ref Projectile.ai[1];
    private NPC Parent => Main.npc[(int)Projectile.ai[2]];
    public override string Texture => TextureRegistry.EmptyTexture;
    private float EasingInOut => EasingFunction.QuadraticBump(Timer / LifeTime);
    private Vector2 ImpactPoint => Projectile.Center + Projectile.velocity;
    private Asset<Texture2D> BeamMaskTextureAsset => ModContent.Request<Texture2D>(ModContent.GetInstance<BigVulcanFireball>().Texture + "_Mask");
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialVelocity = reader.ReadVector2();
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float lineWidth = 64;
        float collisionPoint = 0;
        Vector2 position = Projectile.Center;
        Vector2 previousPosition = position + Projectile.velocity;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.timeLeft = (int)LifeTime;
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            _initialVelocity = Projectile.velocity;
            var sound = AssetRegistry.Sounds.RekLaser with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound, Projectile.position);
        }

        float ratio = Timer / LifeTime;
        float ease = EasingFunction.InOutExpo(ratio);
        float length = ProjectileHelper.PerformBeamHitscan(Projectile, 2400);
        Projectile.Center = Parent.Center;
        Vector2 newVelocity = _initialVelocity.SafeNormalize(Vector2.Zero).RotatedBy(ease * MaxRadians) * length;
        Projectile.velocity = newVelocity;
        float waterSteps = newVelocity.Length() / 4f;
        Vector2 maxPoint = Projectile.Center + newVelocity;
        for (float f = 0; f < waterSteps; f++)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = maxPoint;
            Vector2 inBetween = Vector2.Lerp(start, end, f / waterSteps);
            Point tilePoint = inBetween.ToTileCoordinates();
            Tile tile = Main.tile[tilePoint];
            if (tile.LiquidAmount > 0)
            {
                maxPoint = inBetween;
                break;
            }
        }
        Projectile.velocity = maxPoint - Projectile.Center;
        if (Main.rand.NextBool(2))
        {
            for (int i = 0; i < 3; i++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = ImpactPoint + Main.rand.NextVector2Circular(32, 32),
                    velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 25f),
                    timeLeft = 45,
                    innerColor = Color.Yellow.ToVector4(),
                    outerColor = Color.Red.ToVector4()
                });
            }

        }
        Projectile.rotation = newVelocity.ToRotation();
        DrawUtilities.InterpolateBetweenPointsNonAlloc(ref _beamPoints, Projectile.Center, Projectile.Center + Projectile.velocity * 1.05f);
        if (ease >= 0.9f)
        {
            _bombTimer += 4;
            if (this.OwnedByLocalClient() && Timer % 2 == 0)
            {
                //This places at a lil different spot btw
                float bombRatio = _bombTimer / 60f;
                Vector2 bombVelocity = _initialVelocity.SafeNormalize(Vector2.Zero).RotatedBy(bombRatio * MaxRadians);

                float bombLength = ProjectileHelper.PerformBeamHitscan(Projectile.Center, bombVelocity, 2400);
                bombVelocity *= bombLength;


                float steps = bombVelocity.Length() / 4f;
                Vector2 maxBombPoint = Projectile.Center + bombVelocity;

                //Stop at liquid
                for (float f = 0; f < steps; f++)
                {
                    Vector2 start = Projectile.Center;
                    Vector2 end = maxBombPoint;
                    Vector2 inBetween = Vector2.Lerp(start, end, f / steps);
                    Point tilePoint = inBetween.ToTileCoordinates();
                    Tile tile = Main.tile[tilePoint];
                    if (tile.LiquidAmount > 0)
                    {
                        maxBombPoint = inBetween;
                        break;
                    }
                }

                ProjFirer bombFirer = ProjFirer.From<LaserBeamBombBoom>(Projectile);
                bombFirer.position = maxBombPoint;
                bombFirer.velocity = bombVelocity.SafeNormalize(Vector2.Zero);
                bombFirer.New();
            }
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawImpactGlow();

        SpritebatchDrawer glowCircle = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowCircle.color = Color.White;
        glowCircle.color.A = 0;
        glowCircle.scale *= 0.24f * EasingInOut;
        Main.spriteBatch.Draw(glowCircle);
        return false;
    }
    private void DrawImpactGlow()
    {
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, ImpactPoint);
        drawer.color = Color.OrangeRed * 0.2f;
        drawer.color.A = 0;
        drawer.scale *= 0.9f * EasingInOut;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.Gold * 0.2f;
        drawer.color.A = 0;
        drawer.scale *= 0.84f;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.White * 0.2f;
        drawer.color.A = 0;
        drawer.scale *= 0.84f;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);
    }
    private void DrawFlameImpact(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        BigRekFireballShader shader = ShaderContent.GetInstance<BigRekFireballShader>();
        shader.Time = Main.GlobalTimeWrappedHourly * -64;
        shader.NoiseTexture = AssetManager.Noise.SharpPerlinNoise;
        shader.InnerColor = Color.Yellow;
        shader.BloomColor = Color.DarkRed;
        shader.Strength = 3f;
        shader.MaskTexture = BeamMaskTextureAsset.Value;
        var sbParams = SpritebatchParams.InWorldAndZoomed();
        sbParams.effect = shader.Effect;
        sbParams.blendState = BlendState.Additive;
        float y = MathHelper.Lerp(1.5f, 0.2f, Projectile.velocity.Length() / 80);
        using (new SpritebatchContext(spriteBatch, sbParams))
        {
            SpritebatchDrawer impactDrawer = SpritebatchDrawer.FromTextureAsset(BeamTextureAsset, ImpactPoint);
            impactDrawer.color = Color.White;
        //    impactDrawer.worldPosition -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 144;
            impactDrawer.scale *= 1.2f * EasingInOut * 0.5f;
            impactDrawer.scale.X *= 1.5f;
            impactDrawer.rotation = Projectile.rotation;
            spriteBatch.Draw(impactDrawer);

            impactDrawer.color = Color.Orange * 1f;
            spriteBatch.Draw(impactDrawer);
        }
    }
    private void DrawFlamingBeam(GraphicsDevice graphicsDevice)
    {
        Color GetBeamColor(float ratio)
        {
            return Color.Lerp(Color.Yellow, Color.OrangeRed, ratio) * EasingInOut;
        }
        float GetBeamWidth(float ratio)
        {
            return MathHelper.SmoothStep(196, 296, ratio) * 0.1f;
        }

        FixedRichLaserShader richShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        richShader.LaserTexture = BeamTextureAsset;
        richShader.OuterColor = Color.Red;
        richShader.LaserColor = Color.White;
        richShader.InnerColor = Color.Orange;
        richShader.Time = Main.GlobalTimeWrappedHourly * 128;
        TrailDrawer.Draw(_beamPoints, GetBeamColor, GetBeamWidth, richShader, Vector2.Zero);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawFlameImpact, DrawLayer.OverNPCsAdditive);
        PixelationManager.QueuePrimitivesDrawAction(DrawFlamingBeam);
    }
}

public class LaserBeamBombBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private float LifeTime => 120;
    private float Sparkle_Time => 45;
    private ref float Timer => ref Projectile.ai[0];
    private float Time => LifeTime - Sparkle_Time;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.timeLeft = (int)LifeTime;
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer >= Sparkle_Time)
        {
            FXUtil.ApplyContrast(MathHelper.Lerp(0.5f, 0f, Timer / Time));
            Projectile.hostile = true;
        }

        if (Timer == Sparkle_Time)
        {
            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.position);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);


            if (this.OwnedByLocalClient())
            {
                ProjFirer firer = ProjFirer.From<MeteorBoom>(Projectile);
                firer.position = Projectile.Center;
                firer.ai1 = -0.25f;
                firer.velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero) * 1524;
                firer.New();

            }
        }
    }
    private void DrawPixelatedFlameBoom(SpriteBatch sb, Vector2 sp)
    {
        NoisyBoomShader boomShader = ShaderContent.GetInstance<NoisyBoomShader>();
        boomShader.Time = Main.GlobalTimeWrappedHourly * 8;
        boomShader.NoiseColor = Color.Red;
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = boomShader };

        float time = (Timer - Sparkle_Time) / Time;
        float ease = EasingFunction.OutExpo(time);
        float ease2 = EasingFunction.InOutSine(time);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Red * 0.8f * ExtraMath.Osc(0.6f, 1f, speed: 6) * MathHelper.Lerp(1f, 0f, ease2);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.2f * MathHelper.Lerp(0f, 1f, ease);
        sb.Draw(glowDrawer);
        using (SpritebatchStarter.Begin(sb, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.FlameVortexNoise.Asset, Projectile.Center);
            drawer.scale = Vector2.One * MathHelper.Lerp(0.2f, 1.56f, ease);
            drawer.color = Color.Lerp(Color.Gold, Color.Transparent, ease2) * 2.0f;
            sb.Draw(drawer);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < Sparkle_Time)
        {
            float sparkleRatio = Timer / Sparkle_Time;
            float ease = EasingFunction.InOutSine(sparkleRatio);
            Vector2 scale = Vector2.Lerp(new Vector2(0.5f), new Vector2(1.5f), ease) * 2;
            float rot = (-Projectile.velocity).ToRotation();
            SpritebatchDrawer sparkleDrawer = SpritebatchDrawer.FromTextureAsset(AssetRegistry.GlowMasks.SimpleGlowCircle.Value, Projectile.Center);
            sparkleDrawer.color = Color.OrangeRed * 0.9f * EasingFunction.QuadraticBump(sparkleRatio);
 
            sparkleDrawer.color.A = 0;
            sparkleDrawer.rotation = rot;
            sparkleDrawer.scale = scale * new Vector2(1f, 0.12f);
            Main.spriteBatch.Draw(sparkleDrawer);
          
            
            sparkleDrawer.color = Color.Yellow * 0.9f * EasingFunction.QuadraticBump(sparkleRatio);

            sparkleDrawer.color.A = 0;
            sparkleDrawer.scale *= 0.75f;
            Main.spriteBatch.Draw(sparkleDrawer);
        }
        else
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlameBoom);
        }
        return false;
    }
}