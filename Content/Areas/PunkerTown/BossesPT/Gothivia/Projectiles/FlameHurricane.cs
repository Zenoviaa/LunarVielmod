using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class FlameHurricane : ModProjectile,
    IDrawToRenderTarget
{

    private float _circleRadius;
    private float _insideRadius = 800;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        Vector2 targetCenter = targetHitbox.Center();
        Vector2 center = projHitbox.Center();
        float distance = Vector2.Distance(center, targetCenter);
        if (distance > _insideRadius && distance < 5000)
        {
            return true;
        }
        return base.Colliding(projHitbox, targetHitbox);
        
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_circleRadius);
        writer.Write(_insideRadius);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _circleRadius = reader.ReadSingle();
        _insideRadius = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 800;
     
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
       
        Timer++;
        if (Timer == 1)
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            shaderSystem.TintScreen(Color.OrangeRed, 0.1f, timer: 680);
            shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.025f, timer: 560);
            shaderSystem.VignetteScreen(1f, timer: 560);

            SoundStyle fireIn = AssetRegistry.Sounds.Fire.Flamewheel;
            SoundEngine.PlaySound(fireIn, Projectile.position);
        }
        _insideRadius = 800;
    }

    public override bool PreDraw(ref Color lightColor) => false;
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }


    private void DrawFlameSwirl(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        FlameSwirlShader flameSwirlShader = ShaderContent.GetInstance<FlameSwirlShader>();
        flameSwirlShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameSwirlShader.BloomColor = Color.Red;
        flameSwirlShader.Time = Main.GlobalTimeWrappedHourly * 12;
        flameSwirlShader.AngleRadius = 0;
        flameSwirlShader.AngleCenter = 0;

      //  flameSwirlShader.Radius = 0.5f;
        SpritebatchParams flameSwirlSparams = SpritebatchParams.InWorldAndZoomed() with { effect = flameSwirlShader };
        flameSwirlSparams.sortMode = SpriteSortMode.Immediate;
        using (SpritebatchStarter.Begin(spriteBatch, flameSwirlSparams))
        {
            float easeIn = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 180f));
            float easeOuth = EasingFunction.InOutSine((float)Projectile.timeLeft / 120f);
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.InvertedVoronoi.Asset.Value, Projectile.Center);
            Vector2 size = drawer.texture.Size();
         
            drawer.scale = Vector2.One * 1.5f;
            drawer.color = Color.White * easeOuth * 0.3f * easeIn;
            drawer.color.A = 0;
            spriteBatch.Draw(drawer);

            drawer.color = Color.Red * 0.2f * easeOuth * easeIn;
            drawer.color.A = 0;
            drawer.scale *= 1.2f;
            spriteBatch.Draw(drawer);

            drawer.scale *= 0.7f;
            spriteBatch.Draw(drawer);

        }
      
    }

    private void DrawBigFlameSwirl(SpriteBatch spriteBatch, Vector2 screenPos)
     {
        FlameHurricaneShader flameSwirlShader = ShaderContent.GetInstance<FlameHurricaneShader>();
        flameSwirlShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameSwirlShader.BloomColor = Color.Red;
        flameSwirlShader.Time = Main.GlobalTimeWrappedHourly;
        flameSwirlShader.Radius = 0.15f;
        SpritebatchParams flameSwirlSparams = SpritebatchParams.InWorldAndZoomed() with { effect = flameSwirlShader };
        flameSwirlSparams.sortMode = SpriteSortMode.Immediate;

        using (SpritebatchStarter.Begin(spriteBatch, flameSwirlSparams))
        {
            float easeIn = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 180f));
            float easeOuth = EasingFunction.InOutSine((float)Projectile.timeLeft / 120f);
            int max = (int)MathF.Max(Main.screenWidth * 2, Main.screenHeight * 2);
            SpritebatchDrawer d = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.InvertedVoronoi.Asset.Value, Projectile.Center);
            Rectangle dstRectangle = new Rectangle(0, 0, max, max);
            dstRectangle.X = (int)Projectile.Center.X - dstRectangle.Width / 2;
            dstRectangle.Y = (int)Projectile.Center.Y - dstRectangle.Height / 2;

            dstRectangle.X -= (int)Main.screenPosition.X;
            dstRectangle.Y -= (int)Main.screenPosition.Y;
            d.scale *= 1;
            d.dstRect = dstRectangle;
            d.drawOrigin = Vector2.Zero;
            d.color = Color.White * easeOuth * 0.3f * easeIn;
            d.color.A = 0;
            spriteBatch.Draw(d);
        }
    }
    

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawBigFlameSwirl);
        PixelationManager.QueueSpritebatchDrawAction(DrawFlameSwirl);
    }
}
