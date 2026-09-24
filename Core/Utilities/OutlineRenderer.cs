using Stellamod.Common.Shaders;
using Stellamod.Core.Rendering.RTs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

/// <summary>
/// Renders an outline behind NPCs
/// </summary>
[Autoload(Side = ModSide.Client)]
public class OutlineRenderer : ModSystem
{
    public delegate void DrawAction(SpriteBatch spriteBatch);
    private readonly Queue<DrawAction> _drawQueue = new();
    public override void Load()
    {
        base.Load();
        On_Main.DoDraw_DrawNPCsOverTiles += DrawOverNPCs;
    }

    private void DrawOverNPCs(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
    {
        if (!Main.gameMenu && _drawQueue.Count > 0)
        {
            SpriteBatch sb = Main.spriteBatch;
            using var outlineTarget = RT.Context(RenderTargets.ScreenTarget);
            using (RT.Clear(outlineTarget, Color.Transparent))
            {
                var whiteShader = SpriteWhiteShader.Instance;
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                    RasterizerState.CullNone, whiteShader.Effect, Main.GameViewMatrix.TransformationMatrix);
                while (_drawQueue.Count > 0)
                {

                    DrawAction action = _drawQueue.Dequeue();
                    action(sb);

                }
                sb.End();
            }

            OutlineShader outlineShader = OutlineShader.Instance;
            Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
            outlineShader.TexelSize = texelSize;
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                Main.Rasterizer, outlineShader.Effect);
            sb.Draw(outlineTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            sb.End();
        }
        orig(self);
    }


    public static void Queue(DrawAction drawAction)
    {
        OutlineRenderer renderer = ModContent.GetInstance<OutlineRenderer>();
        renderer._drawQueue.Enqueue(drawAction);
    }
}
