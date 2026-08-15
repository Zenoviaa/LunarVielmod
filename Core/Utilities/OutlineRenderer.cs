using Stellamod.Common.Shaders;
using Stellamod.Core.Rendering;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

[Autoload(Side = ModSide.Client)]
public class OutlineRenderer : ModSystem
{
    public delegate void DrawAction(SpriteBatch spriteBatch);
    private RenderTargetProvider _outlineRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private Queue<DrawAction> _drawQueue;
    private int _screenDrawTimer;
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderEthereal;
        On_Main.DoDraw_DrawNPCsOverTiles += DrawOverNPCs;
    }

    private void RenderEthereal(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;

        _screenDrawTimer--;



        if (_drawQueue == null)
            return;


        if (_drawQueue.Count <= 0)
            return;

        SpriteBatch sb = Main.spriteBatch;
        GraphicsDevice graphicsDevice = sb.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_outlineRT);
        graphicsDevice.Clear(Color.Transparent);

        var whiteShader = SpriteWhiteShader.Instance;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, 
            RasterizerState.CullNone, whiteShader.Effect, Main.GameViewMatrix.TransformationMatrix);
        while (_drawQueue.Count > 0)
        {
           
            DrawAction action = _drawQueue.Dequeue();
            action(sb);

        }
        _screenDrawTimer = 120;
        sb.End();

    }

    private void DrawOverNPCs(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
    {
        if (!Main.gameMenu)
        {
            DrawToScreen();
        }
        orig(self);
    }

    public void DrawToScreen()
    {
        if (_screenDrawTimer <= 0)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        OutlineShader outlineShader = OutlineShader.Instance;
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
        outlineShader.TexelSize = texelSize;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
            Main.Rasterizer, outlineShader.Effect);
        spriteBatch.Draw(_outlineRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        spriteBatch.End();
    }

    public static void Queue(DrawAction drawAction)
    {
        OutlineRenderer renderer = ModContent.GetInstance<OutlineRenderer>();
        renderer._drawQueue ??= new Queue<DrawAction>();
        renderer._drawQueue.Enqueue(drawAction);
    }
}
