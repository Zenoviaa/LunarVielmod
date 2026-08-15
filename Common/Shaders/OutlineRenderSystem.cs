using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
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
        private List<IDrawOutlines> _outlinesToDraw;
        private List<Color> _lightColors;
        private RenderTargetProvider _playerOutlineRenderRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
        public bool canDrawNPCOutlines;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.DrawNPCs += DrawOutlines;
            On_Main.CheckMonoliths += DrawToPlayerOutlineRT;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawPlayerOutlineRTToScreen;
        }


        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawNPCs -= DrawOutlines;
            On_Main.CheckMonoliths -= DrawToPlayerOutlineRT;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawPlayerOutlineRTToScreen;

            _outlinesToDraw = null;
            _lightColors = null;
        }

        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
            canDrawNPCOutlines = true;
        }
        private void DrawToPlayerOutlineRT(On_Main.orig_CheckMonoliths orig)
        {
            if (OutlineAnyPlayers() && Main.netMode != NetmodeID.Server && !Main.gameMenu)
            {
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                SpriteBatch spriteBatch = Main.spriteBatch;

                LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
                graphicsDevice.SetRenderTarget(_playerOutlineRenderRT);
                graphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null);

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

                spriteBatch.End();
                graphicsDevice.SetRenderTarget(null);
                OutlineRenderer.Queue(DrawWhite);
            }
            orig();
        }

        private bool OutlineAnyPlayers()
        {
            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
            return clientConfig.OutlinePlayer || clientConfig.OutlineOtherPlayers;

        }

        private void DrawPlayerOutlineRTToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (!OutlineAnyPlayers())
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;

        }


        private void DrawWhite(SpriteBatch sb)
        {
            sb.Draw(_playerOutlineRenderRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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

            _outlinesToDraw ??= new List<IDrawOutlines>();
            _lightColors ??= new List<Color>();

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
