using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

[Autoload(Side = ModSide.Client)]
public class StarDancersRenderer : ModSystem
{
    private float _timer;
    float _frameTimer;
    int _frameIndex;
    int FrameCount => 7;
    public static float starsTimer;
    public override void Load()
    {
        base.Load();
        On_Main.Draw += DrawOverEverything;
    }

    public override void Unload()
    {
        base.Unload();
        On_Main.Draw -= DrawOverEverything;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        starsTimer--;
        if (starsTimer > 0)
            _timer++;
        else
            _timer--;
        _timer = MathHelper.Clamp(_timer, 0f, 60f);
        _frameTimer++;
        if(_frameTimer >= 5)
        {
            _frameTimer = 0;
            _frameIndex++;
            _frameIndex %= FrameCount;
        }
    }
    private void DrawOverEverything(On_Main.orig_Draw orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (Main.gameMenu)
            return;
        if (_timer <= 0)
            return;

        DrawGoldenAurora();
    }

    private void DrawGoldenAurora()
    {
        var pass = AssetReferences.Effects.Niivi.StarDancerScroll.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviStarDancer.Asset, Main.screenPosition);
        drawer.color = Color.White * EasingFunction.InOutSine(_timer / 60F) * 0.5F;
        drawer.VerticalFrame(_frameIndex, FrameCount);
        drawer.sourceRect = drawer.sourceRect.Value with { Width = Main.screenWidth };
        drawer.drawOrigin = Vector2.Zero;

        var sb = Main.spriteBatch;
        //shader.Time = 0;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, pass.Shader);

        drawer.dstRect = new Rectangle(0, 0, Main.screenWidth, drawer.Height);
        drawer.spriteEffects = SpriteEffects.FlipVertically;
        sb.Draw(drawer);


        drawer.dstRect = new Rectangle(0,Main.screenHeight - drawer.Height, Main.screenWidth, drawer.Height);
        drawer.spriteEffects = SpriteEffects.FlipHorizontally;
        sb.Draw(drawer);
        sb.End();
    }
}
