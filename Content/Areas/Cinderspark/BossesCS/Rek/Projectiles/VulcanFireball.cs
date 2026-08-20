using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Effects.GothinFlames;
using Stellamod.Effects.RekFlames;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class BigVulcanFireball : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
        ProjectileID.Sets.TrailCacheLength[Type] = 48;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 140;
    }

    public override void AI()
    {

        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var sound = new SoundStyle("Stellamod/Assets/Sounds/RekFireballShoot") with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound, Projectile.position);

            FXUtil.ShakeCamera(Projectile.position, 1024, 4);
            for (float f = 0; f < 6; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(5f, 45);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 48), vel);
                dp.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
                dp.outerColor = Color.Red;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.Scale *= Main.rand.NextFloat(1f, 1.5f);
            }
            for (float f = 0; f < 6; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(5f, 45);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), DustID.Torch, vel, Scale: 2f);
            }
            for (float f = 0; f < 6; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(5f, 45);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), DustID.Lava, vel, Scale: 2f);
            }
        }

        if (Main.rand.NextBool(4))
        {
            switch (Main.rand.Next(2))
            {
                case 0:
                    DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    sp.gravity = 0f;
                    sp.fast = true;
                    sp.dampening = 0.1f;
                    break;
                case 1:
                    FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                    sp2.gravity = 0f;
                    sp2.fast = true;
                    sp2.dampening = 0.1f;
                    break;
            }
        }

        if(Timer < 15)
        {
            Projectile.velocity *= 1.03f;
         
        }
        else
        {
 
        }
        Projectile.velocity.Y += 0.4f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        var p = PlayerHelper.FindClosestPlayer(Projectile.Center, 1024);
        if(p != null && Timer > 15)
        {
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, p.Center, degreesToRotate: 0.5f);
        }


    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X * 0.6f;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -oldVelocity.Y * 0.6f;
        return false;
    }
    private void DrawFlameTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(96, 64, ratio) * 0.5f;
        }
        float GetTrailWidth2(float ratio)
        {
            return GetTrailWidth(ratio) * 2f;
        }
        Color GetTrailColor(float ratio)
        {
            return DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.White, Color.OrangeRed,  Color.Red, Color.DarkRed, Color.Black);
            //    return Color.Lerp(Color.Lerp(Color.White, Color.Yellow, EasingFunction.OutQuad(ratio)), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), EasingFunction.OutQuad(ratio)), EasingFunction.OutExpo(ratio)) * _afterImageAlpha;
        }

        Color GetTrailColor2(float ratio)
        {
            return Color.Lerp(GetTrailColor(ratio), Color.DarkRed, 0.5f) * 0.3f;
        }

        GothinFlameTrailShader flameTrailShader = ShaderContent.GetInstance<GothinFlameTrailShader>();
        flameTrailShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameTrailShader.BloomColor = Color.Red;
        flameTrailShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
    

        flameTrailShader.LaserTexture = AssetManager.LaserTextures.FlameTrail.Value;
        flameTrailShader.Time = Main.GlobalTimeWrappedHourly * 24;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, Projectile.Size * 0.5f);

        flameTrailShader.LaserTexture = TrailRegistry.WhispyTrail.Value;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor2, GetTrailWidth2, flameTrailShader, Projectile.Size * 0.5f);
    }

    private void DrawFlameBall(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        BigRekFireballShader shader = ShaderContent.GetInstance<BigRekFireballShader>();
        shader.Time = Main.GlobalTimeWrappedHourly * -64;
        shader.NoiseTexture = AssetManager.Noise.SharpPerlinNoise;
        shader.InnerColor = Color.Yellow;
        shader.BloomColor = Color.DarkRed;
        shader.Strength = 3f;
        shader.DitherTexture = AssetManager.Dithering.Dither8x8Double;
        shader.SpriteSize = TextureAssets.Projectile[Type].Size();
        var sbParams = SpritebatchParams.InWorldAndZoomed();
        sbParams.effect = shader.Effect;
        sbParams.blendState = BlendState.Additive;
        float y = MathHelper.Lerp(1.5f, 0.2f, Projectile.velocity.Length() / 80);
        using (new SpritebatchContext(spriteBatch, sbParams))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = Color.White;
            drawer.scale *= 1.4f;
            drawer.scale.Y *= 0.75f * y;
            spriteBatch.Draw(drawer);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {


        PixelationManager.QueueSpritebatchDrawAction(DrawFlameBall, DrawLayer.OverNPCsAdditive);
        PixelationManager.QueuePrimitivesDrawAction(DrawFlameTrail, DrawLayer.OverNPCsAdditive);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
        for (float n = 0; n < 2f; n++)
        {
            var spawnParams = new DustParticleSpawnParams();
            spawnParams.innerColor = Color.OrangeRed;
            spawnParams.outerColor = Color.Red;
            spawnParams.scaleRange = new Vector2(0.1f, 3f);
            DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
        }
        if (this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<PacmanBoom>(Projectile);
            firer.New();
        }

        SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
        sp.initialColor = Color.White * 0.14f;
    }
}
