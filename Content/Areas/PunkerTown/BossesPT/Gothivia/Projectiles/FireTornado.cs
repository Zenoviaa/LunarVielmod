using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class FireTornado : ModProjectile, 
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = false;
        Projectile.timeLeft = 600;
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float lineWidth = 96;
        float collisionPoint = 0;
        Vector2 position = Projectile.Center;
        Vector2 previousPosition = Projectile.Center + Projectile.velocity; ;

        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint))
            return true;
        return base.Colliding(projHitbox, targetHitbox);
    }

    public override void AI()
    {
        base.AI();
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        Timer++;
        if(Timer >= 120 && Timer < 540)
        {
            Projectile.hostile = true;
        }
        else
        {
            Projectile.hostile = false;
        }
        if(Timer == 1)
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            shaderSystem.TintScreen(Color.Red, 0.1f, timer: 60);
            shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.025f, timer: 60);
        }

        float t = 60;
        if(Timer > t && Timer < t  + 45)
        {
            FXUtil.ApplyContrast(MathHelper.Lerp(0.5f, 0f, EasingFunction.InOutExpo((Timer-t) / 45f)));
        }
    
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }

    private void DrawPixelatedTornado(SpriteBatch sb, Vector2 sp)
    {
        var fireTornad = ShaderContent.GetInstance<FireTornadoShader>();
        //var pass = AssetReferences.Effects.GothinFlames.FireTornado.CreatePixelPass();
        fireTornad.Time = Main.GlobalTimeWrappedHourly * 0.1f;
        fireTornad.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        fireTornad.GradientTopColor = new Color(224, 187, 122);
        fireTornad.GradientBottomColor = new Color(59, 19, 13);
        fireTornad.FlameyTexture  = AssetManager.Noise.FlamethrowerNoise.Value;
        fireTornad.NoiseTexture = AssetManager.Noise.Whirly.Value;

        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, 
            DepthStencilState.None, RasterizerState.CullNone, fireTornad.Effect, Main.GameViewMatrix.TransformationMatrix);
  
        //sb.Restart(effect: effect.Value);

        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromProjectile(Projectile);
        drawer2.color = Color.White;

        drawer2.BottomCenterOrigin();
        drawer2.scale.Y *= 2;
        drawer2.scale.X *= 2;
        //drawer2.scale *= 2f;
       
        float ease2 = Timer / 144;
        ease2 = EasingFunction.InOutExpo(ease2);
        drawer2.scale *= MathHelper.Lerp(4f, 1f, ease2);
        drawer2.color = Color.Lerp( Color.Transparent, drawer2.color, ease2);

        float time = Timer - 540;
        float ease = time / 60f;
        ease = EasingFunction.InOutSine(ease);
        drawer2.scale *= MathHelper.Lerp(1f, 2f, ease);
        drawer2.color = Color.Lerp(drawer2.color, Color.Transparent, ease);

        var drawer3 = drawer2;
        drawer3.scale *= 1.75f;
        drawer3.color = Color.Lerp(drawer3.color, Color.Black, 0.85f) * 0.35f;
        sb.Draw(drawer3);

        var drawer4 = drawer2;
        drawer4.scale *= 1.25f;
        drawer4.color = Color.Lerp(drawer4.color, Color.Black, 0.2f) * 0.15f;
        sb.Draw(drawer4);

        sb.Draw(drawer2);

        var drawer5 = drawer2;
        drawer5.scale *= 2.5f;
        drawer5.color *= 0.2f;
        drawer5.color.A = 0;
        sb.Draw(drawer5);

        sb.End();

        fireTornad.FlameyTexture = AssetManager.Noise.PainterlyNoise.Value;
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
            DepthStencilState.None, RasterizerState.CullNone, fireTornad.Effect, Main.GameViewMatrix.TransformationMatrix);


        drawer5.color *= 1.25f;
        drawer5.scale *= 0.5f;
   
        sb.Draw(drawer5);



        sb.RestartDefaults();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedTornado);
 //       throw new System.NotImplementedException();
    }
}
