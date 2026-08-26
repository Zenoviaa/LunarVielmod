using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
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

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.EyeProjectiles;

public class LaserBeamBomb : ModProjectile,
    IDrawToRenderTarget
{
    private Vector2 _initialVelocity;
    private float LifeTime => 100f;
    private Asset<Texture2D> BeamTextureAsset => TextureAssets.Projectile[ModContent.ProjectileType<BigVulcanFireball>()];
    private Vector2[] _beamPoints = new Vector2[64];
    private ref float Timer => ref Projectile.ai[0];
    private ref float MaxRadians => ref Projectile.ai[1];
    private NPC Parent => Main.npc[(int)Projectile.ai[2]];
    public override string Texture => TextureRegistry.EmptyTexture;
    private float EasingInOut => EasingFunction.QuadraticBump(Timer / LifeTime);
    private Vector2 ImpactPoint => Projectile.Center + Projectile.velocity;
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
        if(Timer == 1)
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
        Projectile.rotation = newVelocity.ToRotation();
        DrawUtilities.InterpolateBetweenPointsNonAlloc(ref _beamPoints, Projectile.Center, Projectile.Center + Projectile.velocity * 1.05f);
        if(Timer % 10 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                //This places at a lil different spot btw
                float bombRatio = Timer / LifeTime;
                Vector2 bombVelocity = _initialVelocity.SafeNormalize(Vector2.Zero).RotatedBy(bombRatio * MaxRadians);
             
                float bombLength  = ProjectileHelper.PerformBeamHitscan(Projectile.Center, bombVelocity, 2400);
                bombVelocity *= bombLength;


                float steps = bombVelocity.Length() / 16f;
                Vector2 maxBombPoint = Projectile.Center + bombVelocity;

                //Stop at liquid
                for (float f = 0; f < steps; f++)
                {
                    Vector2 start = Projectile.Center;
                    Vector2 end = maxBombPoint;
                    Vector2 inBetween = Vector2.Lerp(start, end, f / steps);
                    Point tilePoint = inBetween.ToTileCoordinates();
                    Tile tile = Main.tile[tilePoint];
                    if(tile.LiquidAmount > 0)
                    {
                        maxBombPoint = inBetween;
                        break;
                    }
                }
           
                ProjFirer bombFirer = ProjFirer.From<LaserBeamBombBoom>(Projectile);
                bombFirer.position = maxBombPoint;
                bombFirer.velocity = Vector2.Zero;
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
        return false;
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
        PixelationManager.QueuePrimitivesDrawAction(DrawFlamingBeam);
    }
}

public class LaserBeamBombBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private float LifeTime => 90;
    private float Sparkle_Time => 60f;
    private ref float Timer => ref Projectile.ai[0];
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
        if(Timer >= Sparkle_Time)
        {
            Projectile.hostile = true;
        }

        if(Timer == Sparkle_Time)
        {
            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.position);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            for (float f = 0; f < 4; f++)
            {
                Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if(Timer < Sparkle_Time)
        {
            float sparkleRatio = Timer / Sparkle_Time;
            float ease = EasingFunction.InOutSine(sparkleRatio);
            Vector2 scale = Vector2.Lerp(Vector2.One, Vector2.Zero, ease);
            float rot = MathHelper.Lerp(MathHelper.TwoPi * 2, 0, ease);
            SpritebatchDrawer sparkleDrawer = SpritebatchDrawer.FromTextureAsset(AssetRegistry.GlowMasks.Star2.Value, Projectile.Center);
            sparkleDrawer.color = Color.White;
            sparkleDrawer.color.A = 0;
            sparkleDrawer.rotation = rot;
            sparkleDrawer.scale = scale;
            Main.spriteBatch.Draw(sparkleDrawer);
        }
        return false;
    }
}