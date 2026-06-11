using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class SpiralDashTrail : ModProjectile,
    IDrawToRenderTarget
{
    private float Time => 40;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)Time;
        Projectile.ignoreWater = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Parent.ModNPC is RoyalFox fox)
        {
            Projectile.Center = fox.HeadPosition;
        }
    }

    private float GetSpiralDashTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 96, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
    }
    private float GetSpiralDashTrailWidth2(float completionRatio)
    {
        return GetSpiralDashTrailWidth(completionRatio) * 1.3f;
    }
    private Color GetSpiralDashTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio) * EasingFunction.QuadraticBump(Timer / Time);
    }

    private void DrawSpiralDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader bloomShader = ShaderContent.GetInstance<BasicLaserShader>();
        bloomShader.LaserTexture = AssetManager.LaserTextures.CometTrail;
        bloomShader.InnerColor = Color.SkyBlue;
        bloomShader.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, bloomShader, Projectile.Size * 0.5f);

        BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
        basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
        basicLaserShader.InnerColor = Color.SkyBlue;
        basicLaserShader.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, basicLaserShader, Projectile.Size * 0.5f);


        basicLaserShader.InnerColor = Color.White;
        basicLaserShader.OuterColor = Color.DarkGray;
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth, basicLaserShader, Projectile.Size * 0.5f);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawSpiralDashTrail, DrawLayer.OverPlayers);
    }
}
