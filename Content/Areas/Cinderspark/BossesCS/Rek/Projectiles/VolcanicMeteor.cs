using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Effects.GothinFlames;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public class VolcanicMeteor : ModProjectile
{
    private float _scale;
    private float _timer;
    private float _glowAlpha;
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private int SegmentIndex => (int)Projectile.ai[1];
    private ref float SurfaceLava => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_timer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _timer = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.timeLeft = 240;
        Projectile.light = 0.7f;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        _timer++;
        if (Parent.ModNPC is RekBoss boss)
        {
            var segment = boss.Segments[SegmentIndex];
            segment.position = Projectile.Center;
            segment.velocity = Projectile.velocity;
            segment.rotation += MathF.Sign(Projectile.velocity.X) * 0.05f;
            _scale = segment.SizeMultiplier;
        }
        _glowAlpha = EasingFunction.InOutSine(_timer / 60f);
        _glowAlpha *= MathHelper.Lerp(1f, 0f, (_timer - 60f) / 180f);
        if (Main.rand.NextBool(5))
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

        Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.25f;
        if(Projectile.Center.Y - 64 > SurfaceLava)
        {

            Projectile.Kill();
        }
    }

    private void DrawFlameTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(96, 16, ratio) * _scale;
        }
        Color GetTrailColor(float ratio)
        {
            return DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.Orange, Color.Red, Color.DarkRed, Color.Black) * EasingFunction.OutSine(ratio);
            //    return Color.Lerp(Color.Lerp(Color.White, Color.Yellow, EasingFunction.OutQuad(ratio)), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), EasingFunction.OutQuad(ratio)), EasingFunction.OutExpo(ratio)) * _afterImageAlpha;
        }

        GothinFlameTrailShader flameTrailShader = ShaderContent.GetInstance<GothinFlameTrailShader>();
        flameTrailShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameTrailShader.BloomColor = Color.Red;
        flameTrailShader.TransformMatrix = TrailDrawer.WorldViewPoint2;

        flameTrailShader.LaserTexture = AssetManager.LaserTextures.Aura.Value;
        flameTrailShader.Time = Main.GlobalTimeWrappedHourly * 24;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawFlameTrail, DrawLayer.OverNPCsAdditive);
        return false;
    }
    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
      
        var glowCircle = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(glowCircle, Projectile.Center);
        glowDrawer.scale *= 0.48f * _scale;
        glowDrawer.color = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 12)) * ExtraMath.Osc(0.5f, 0.75f, speed: 8) * _glowAlpha;
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            ProjFirer firer = ProjFirer.From<MeteorBoom>(Projectile);
            firer.velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero) * 1024;
            firer.New();
        }
    }
}
