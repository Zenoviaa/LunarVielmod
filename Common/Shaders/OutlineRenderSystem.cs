using Stellamod.Core.Rendering.RTs;
using Stellamod.Core.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders
{

    public class NPCOutlineDrawer : GlobalNPC
    {
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            OutlineRenderSystem renderSystem = ModContent.GetInstance<OutlineRenderSystem>();
            if (renderSystem.canDrawNPCOutlines)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone,
                    SpriteWhiteShader.Instance.Effect, Main.GameViewMatrix.TransformationMatrix);

                foreach (NPC otherNPC in Main.ActiveNPCs)
                {
                    if (otherNPC.ModNPC is IDrawOutlines outlines)
                    {
                        Point tile = npc.position.ToTileCoordinates();
                        Color lightColor = Lighting.GetColor(tile.X, tile.Y);
                        outlines.DrawOutlines(spriteBatch, Main.screenPosition, lightColor);
                    }
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone,
                    null, Main.GameViewMatrix.TransformationMatrix);
                renderSystem.canDrawNPCOutlines = false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class OutlineRenderSystem : ModSystem
    {
        private readonly List<IDrawOutlines> _outlinesToDraw = new();
        private readonly List<Color> _lightColors = new();
        public bool canDrawNPCOutlines;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.DrawNPCs += DrawOutlines;
            On_Main.DrawPlayers_AfterProjectiles += DrawPlayerOutline;
        }

        private void DrawPlayerOutline(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {

           
            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
            if(clientConfig.OutlinePlayer || clientConfig.OutlineOtherPlayers)
            {
                var sb = Main.spriteBatch;
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                using var outlineTarget = RT.Context(RenderTargets.ScreenTarget);
                using var whiteTarget = RT.Context(RenderTargets.ScreenTarget);
                using (RT.Clear(outlineTarget, Color.Transparent))
                {
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null);

                    if (clientConfig.OutlinePlayer)
                    {
                        DrawLocalPlayer(Main.LocalPlayer);
                    }

                    if (clientConfig.OutlineOtherPlayers)
                    {
                        foreach (var player in Main.ActivePlayers)
                        {
                            if (player.whoAmI == Main.myPlayer)
                                continue;
                            DrawLocalPlayer(player);
                        }
                    }

                    sb.End();
                }
                using (RT.Clear(whiteTarget, Color.Transparent))
                {
                    var whiteShader = SpriteWhiteShader.Instance;
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                        RasterizerState.CullNone, whiteShader.Effect, Main.GameViewMatrix.TransformationMatrix);
                    sb.Draw(outlineTarget, Vector2.Zero, Color.White);
                    sb.End();
                }

                OutlineShader outlineShader = OutlineShader.Instance;
                Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
                outlineShader.TexelSize = texelSize;
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                    Main.Rasterizer, outlineShader.Effect);
                sb.Draw(whiteTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                sb.End();
            }

            orig(self);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            _outlinesToDraw?.Clear();
            _lightColors?.Clear();
        }

        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
            canDrawNPCOutlines = true;
        }

        private bool OutlineAnyPlayers()
        {
            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
            return clientConfig.OutlinePlayer || clientConfig.OutlineOtherPlayers;

        }

        private void DrawWhite(SpriteBatch sb)
        {
           
        }

        private void DrawLocalPlayer(Player player)
        {
            Vector2 drawPosition = player.position;
            drawPosition.Y += player.gfxOffY;
            float rotation = player.fullRotation;


            IPlayerRenderer playerRenderer = Main.PlayerRenderer;
            playerRenderer.DrawPlayer(Main.Camera, player, drawPosition, rotation, player.fullRotationOrigin);;
        }
        private void DrawOutlines(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            _outlinesToDraw.Clear();
            _lightColors.Clear();

            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is IDrawOutlines drawOutline)
                {
                    _outlinesToDraw.Add(drawOutline);
                    Point tile = projectile.position.ToTileCoordinates();
                    Color lightColor = Lighting.GetColor(tile.X, tile.Y);
                    _lightColors.Add(lightColor);
                }
            }

            if (_outlinesToDraw.Count > 0)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone,
                    SpriteWhiteShader.Instance.Effect, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < _outlinesToDraw.Count; i++)
                {
                    IDrawOutlines drawOutlines = _outlinesToDraw[i];
                    Color lightColor = _lightColors[i];
                    drawOutlines.DrawOutlines(spriteBatch, Main.screenPosition, lightColor);
                }


                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone,
         null, Main.GameViewMatrix.TransformationMatrix);
            }

            orig(self, behindTiles);
        }


    }

    public interface IDrawOutlines
    {
        void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor);
    }
}
