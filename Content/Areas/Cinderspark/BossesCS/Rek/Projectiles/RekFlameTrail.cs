using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.GothinFlames;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class RekFlameTrail : ModProjectile
{
    private float _afterImageAlpha;
    private ref float Timer => ref Projectile.ai[1];
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1800;
    }


    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.light = 0.78f;
        Projectile.timeLeft = 100;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        Projectile.Center = Parent.Center;
        _afterImageAlpha = EasingFunction.OutExpo(Timer / 40f);
        _afterImageAlpha *= MathHelper.Lerp(1f, 0f, Timer / 180f);
    }
    private void DrawFlameTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(96, 16, ratio) * _afterImageAlpha;
        }
        Color GetTrailColor(float ratio)
        {
            return DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.Orange, Color.Red, Color.DarkRed, Color.Black) * _afterImageAlpha * EasingFunction.OutSine(ratio); 
            //    return Color.Lerp(Color.Lerp(Color.White, Color.Yellow, EasingFunction.OutQuad(ratio)), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), EasingFunction.OutQuad(ratio)), EasingFunction.OutExpo(ratio)) * _afterImageAlpha;
        }

        GothinFlameTrailShader flameTrailShader = ShaderContent.GetInstance<GothinFlameTrailShader>();
        flameTrailShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameTrailShader.BloomColor = Color.Red;
        flameTrailShader.TransformMatrix = TrailDrawer.WorldViewPoint2;

        flameTrailShader.LaserTexture = AssetManager.LaserTextures.Aura.Value;
        flameTrailShader.Time = Main.GlobalTimeWrappedHourly * 24;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, Projectile.Size * 0.5f);
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawFlameTrail, DrawLayer.OverNPCsAdditive);
        return false;
    }
}
