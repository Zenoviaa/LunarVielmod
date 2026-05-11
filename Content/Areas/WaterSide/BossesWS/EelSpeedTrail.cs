using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS;

public class EelSpeedTrail : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 64;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
        Projectile.ignoreWater = true;
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {

        }

        if(Timer <= 60)
        {
            Projectile.Center = Parent.Center;
        }
    }
    private float GetSpeedWidth(float ratio)
    {
        return MathHelper.SmoothStep(342, 0, ratio);
    }
    private Color GetSpeedColor(float ratio)
    {
        float ease = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        return Color.White * EasingFunction.QuadraticBump(ratio) * ease;
    }

    private void DrawSpeedTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader shader = BasicLaserShader.Instance;
        shader.LaserTexture = TrailRegistry.Beamlight;
        shader.InnerColor = Color.White;
        shader.OuterColor = Color.SkyBlue;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetSpeedColor, GetSpeedWidth, shader, Projectile.Size * 0.5f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawSpeedTrail);
        return false;
    }
}
