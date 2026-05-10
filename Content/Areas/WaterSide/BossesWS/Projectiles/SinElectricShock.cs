using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class PrismaticElectricBolt : ModProjectile
{
    private Vector2 _initialVelocity;
    private float _randRadians;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.timeLeft = 180;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            _initialVelocity = Projectile.velocity;
        }


        if (Timer % 30 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                float radians = MathHelper.ToRadians(10);
                _randRadians = Main.rand.NextFloat(-radians, radians);
                Projectile.velocity = _initialVelocity.RotatedBy(_randRadians);
                Projectile.netUpdate = true;
            }
        }
    }
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

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class PrismaticLightningRenderer : PixelPrimitiveRenderer<PrismaticLightningRenderer>
{
    public override BaseShader PrepareShader()
    {
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.White;
        shader.InnerColor = Main.DiscoColor;
        shader.OuterColor = Color.Goldenrod;
        shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
        shader.BloomTexture = AssetManager.LaserTextures.TexturedLaser2;
        return shader;
    }

    public override Color GetTrailColor(float completionRatio)
    {
        float osc = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
        return Color.Lerp(Color.White, Main.DiscoColor, osc);
    }

    public override float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(16, 32, completionRatio) * MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
    }
}

public class SinElectricShock : ModProjectile
{
    private Vector2[] _shockPos;
    private Vector2[] _sparkPos;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        _shockPos = new Vector2[16];
        _sparkPos = new Vector2[16];
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 30;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity = -Vector2.UnitY;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
            var fp = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Goldenrod);
            fp.Scale *= 1.5f;
        }
        if (Timer >= 15)
            Projectile.hostile = false;
        Projectile.velocity = Projectile.velocity.RotatedBy(0.03f);
        if (Timer % 8 == 0)
        {
            DustParticle sp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(128, 128), Main.rand.NextVector2Circular(12, 12), Color.White, 0.7f);
            sp.fast = true;
            sp.gravity = 0;
            sp.noTileCollide = true;
        }

        if (Timer % 8 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(128, 128), Vector2.Zero, Color.White, 0.3f);
            sp.gravity = 0;
        }

        float inScale = EasingFunction.InOutSine(Timer / 10f);
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        for (int i = 0; i < _shockPos.Length; i++)
        {
            ref Vector2 position = ref _shockPos[i];
            Vector2 offset = new Vector2();

            float radians = (float)i / (float)_shockPos.Length * MathHelper.TwoPi;
            radians += Timer * 0.03f;

            float radius = ExtraMath.Osc(80, 128, speed: 18, offset: Projectile.whoAmI);

            radius *= inScale * outScale;
            offset.X += MathF.Sin(radians) * radius;
            offset.Y += MathF.Cos(radians) * radius;
            offset = Vector2.Lerp(offset, Vector2.Zero, (MathF.Sin(Timer * 0.5f + i) + 0.5f) * 0.1f);
            offset += Main.rand.NextVector2Circular(6, 6);
            position = Projectile.Center + offset;

            _sparkPos[i] = Projectile.Center + offset.RotatedBy(MathHelper.PiOver4) * 0.2f * Main.rand.NextFloat(1f, 1.5f);
        }

    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        base.ModifyHitPlayer(target, ref modifiers);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawBloom(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D bloomTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowScale = Vector2.One * 0.25f;
        float rotation = Main.GlobalTimeWrappedHourly * 4;
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomDrawer.color = Main.DiscoColor;
        bloomDrawer.color.A = 0;
        bloomDrawer.color *= 0.1f;
        bloomDrawer.color *= outScale;
        spriteBatch.Draw(bloomDrawer);
        for (int i = 0; i < _shockPos.Length; i += 2)
        {
            Vector2 pos = _shockPos[i];

            Color glowColor = Color.Lerp(Color.White, Main.DiscoColor, 0.6f);
            glowColor.A = 0;
            glowColor *= 0.2f;
            glowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6, offset: i);
            glowColor *= outScale;

            bloomDrawer.worldPosition = pos;
            bloomDrawer.color = glowColor;
            bloomDrawer.scale = glowScale;
            spriteBatch.Draw(bloomDrawer);
        }

        SpritebatchDrawer spiralDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);

        Color spiralGlowColor = Color.Lerp(Color.White, Main.DiscoColor, 0.6f);
        spiralGlowColor.A = 0;
        spiralGlowColor *= 0.2f;
        spiralGlowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6);
        spiralGlowColor *= outScale;
        spiralDrawer.color = spiralGlowColor;
        spriteBatch.Draw(spiralDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawBloom);
        PrismaticLightningRenderer.Queue(_shockPos);
        PrismaticLightningRenderer.Queue(_sparkPos);
        return false;
    }
}
