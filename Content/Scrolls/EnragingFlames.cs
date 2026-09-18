using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Scrolls;

public class EnragingFlames : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = false;
        Projectile.timeLeft = 600;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(1f, 3f));
            }
            SoundEngine.PlaySound(SoundID.Item74 with { PitchVariance = 0.5f }, Projectile.position);
        }

        Vector2 center = Owner.Center;
        center.X -= 14;
        Projectile.velocity = (center - Projectile.Center);

        if (Main.rand.NextBool(16))
        {
            var fs = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 32), -Vector2.UnitY.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 3f));
            fs.Center -= Vector2.UnitY.SafeNormalize(Vector2.Zero) * 64;
            fs.fadeToColor = Color.Black * 0.35f;
            fs.color = Color.RosyBrown * 0.35f;
            fs.Scale *= 0.25f;
        }
        //  Vector2.c
        // Projectile.rotation = _initialVelocity.ToRotation() + MathHelper.PiOver2;
        if (Main.rand.NextBool(8))
        {
            Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(32, 32), ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
        }

        //   Projectile.rotation = Projectile.velocity.X * 0.025f;
        Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 1.75f * Main.essScale);
    }

    private void DrawPixelatedFlames(SpriteBatch sb, Vector2 screenPos)
    {
        // var sb = Main.spriteBatch;
        float fade = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((float)Projectile.timeLeft / 30f));
        float inScale = EasingFunction.OutExpo(Timer / 30f);
        Asset<Texture2D> waveTexture = AssetManager.GlowMask.Wave;
        WaveShader waveShader = ShaderContent.GetInstance<WaveShader>();
        waveShader.Time = Main.GlobalTimeWrappedHourly * 0.5f + Projectile.whoAmI;
        waveShader.Amplitude = 0.3f;
        waveShader.Frequency = 8;
        waveShader.XStrength = 6;
        waveShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        sb.Restart(effect: waveShader.Effect);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(waveTexture, Projectile.Center);
        drawer.rotation = Projectile.rotation;
        drawer.BottomCenterOrigin();
        drawer.color = Color.Red * fade * ExtraMath.Osc(0.6f, 1f, speed: 32, offset: Projectile.whoAmI);
        drawer.color.A = 0;
        drawer.scale *= 0.5f * inScale;
        drawer.scale.Y *= ExtraMath.Osc(1f, 1.1f, offset: Projectile.whoAmI);
        sb.Draw(drawer);

        drawer.TopCenterOrigin();
        drawer.scale.Y *= 0.4f;
        drawer.spriteEffects |= SpriteEffects.FlipVertically;
        drawer.rotation = Projectile.rotation;
        sb.Draw(drawer);

        sb.RestartDefaults();

        Asset<Texture2D> bloomLine = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromTextureAsset(bloomLine, Projectile.Center + new Vector2(0f, 12));
        //      drawer2.BottomCenterOrigin();
        drawer2.scale *= new Vector2(0.55f, 0.55f) * ExtraMath.Osc(0.8f, 1f, speed: 3) * inScale;
        drawer2.color = Color.Red * fade * 0.15f; ;
        drawer2.color.A = 0;
        drawer2.rotation = Projectile.rotation;
        sb.Draw(drawer2);

        drawer2.scale *= 2;
        drawer2.color = Color.Red * fade * 0.08f; ;
        drawer2.color.A = 0;
        sb.Draw(drawer2);

        SpritebatchDrawer blastPillar = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, Projectile.Center + new Vector2(0f, 12));
        blastPillar.BottomCenterOrigin();
        blastPillar.color = Color.Red * 0.5f * ExtraMath.Osc(0.6f, 1f, speed: 32, offset: Projectile.whoAmI) * fade;
        blastPillar.color.A = 0;
        blastPillar.scale *= 0.6f;
        blastPillar.rotation = Projectile.rotation;
        sb.Draw(blastPillar);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlames, DrawLayer.OverPlayers);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
