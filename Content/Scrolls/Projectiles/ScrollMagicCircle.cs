using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Scrolls.Projectiles;

public class ScrollMagicCircle : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    private ScrollAbility Ability
    {
        get => (ScrollAbility)Projectile.ai[2];
        set => Projectile.ai[2] = (float)value;
    }
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => TextureRegistry.EmptyTexture;
    public Color hintColor;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.width = 1;
        Projectile.height = 1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 6 == 0)
        {
            var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 48), -Vector2.UnitY);
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.02f;
            dp.Scale *= 0.5f;
            dp.outerColor = Color.DarkGray;
        }
        Projectile.Center = Owner.Center;
    }

    private float ScaleEasing()
    {
        float time = Timer / 120f;
        float easeIn = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(time / (0.5f)));
        float easeOut = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(time));
        float easeBig = MathHelper.Lerp(2f, 1f, EasingFunction.OutExpo(time / 0.5f));
        return easeOut * easeBig;
    }
    private Asset<Texture2D> GetMagicCircleTextureAsset()
    {
        switch (Style)
        {
            default:
            case 0:
                return AssetManager.GlowMask.MagicCircle2;
        }
    }
    private void DrawMagicCircle(SpriteBatch sb, Vector2 screenPos)
    {
        float inAlpha = EasingFunction.InOutSine(Timer / 30f);
        float outAlpha = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        float alpha = inAlpha * outAlpha;

        float outRatio = MathHelper.Lerp(1f, 0f, EasingFunction.InSine((float)Projectile.timeLeft / 60f));
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio));
        waveDrawer.color = Color.White;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        SpritebatchDrawer magicCircleDrawer = SpritebatchDrawer.FromTextureAsset(GetMagicCircleTextureAsset(), Projectile.Center);
        magicCircleDrawer.color = hintColor * alpha * ExtraMath.Osc(0.45f, 1f, speed: 24);
        magicCircleDrawer.color.A = 0;
        magicCircleDrawer.rotation = Main.GlobalTimeWrappedHourly * 1 ;
        magicCircleDrawer.scale *= 0.8f * ScaleEasing();
        sb.Draw(magicCircleDrawer);
        Main.spriteBatch.RestartDefaults();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawMagicCircle(Main.spriteBatch, Vector2.Zero);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
     //   PixelationManager.QueueSpritebatchDrawAction(DrawMagicCircle);
        //     throw new NotImplementedException();
    }
}
