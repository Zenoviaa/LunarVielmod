using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.TextureAssets;

namespace Stellamod.Core.Shaders
{

    public class OutlineRenderSystem : ModSystem
    {
        private List<IDrawOutlines> _outlinesToDraw;
        private List<Color> _lightColors;
        private RenderTarget2D _playerOutlineRenderRT;
        private Vector2 _previousScreenSize;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.DrawNPCs += DrawOutlines;

            On_Main.CheckMonoliths += DrawToPlayerOutlineRT;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawPlayerOutlineRTToScreen;
            ResizeRenderTarget(true);
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
            }
            orig();
        }


        public override void PostUpdateEverything()
        {
            ResizeRenderTarget(false);
        }

        private bool OutlineAnyPlayers()
        {
            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
            return clientConfig.OutlinePlayer || clientConfig.OutlineOtherPlayers;

        }
        private void ResizeRenderTarget(bool load)
        {
            if (!OutlineAnyPlayers())
                return;

            if (!Main.gameMenu && !Main.dedServ || load && !Main.dedServ)
            {
                Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
                if (currentScreenSize != _previousScreenSize)
                {
                    Main.QueueMainThreadAction(() =>
                    {

                        if (_playerOutlineRenderRT != null && !_playerOutlineRenderRT.IsDisposed)
                            _playerOutlineRenderRT.Dispose();

                        _playerOutlineRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    });
                }

                _previousScreenSize = currentScreenSize;
            }
        }

        private void DrawPlayerOutlineRTToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (!OutlineAnyPlayers())
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, SpriteWhiteShader.Instance.Effect, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(_playerOutlineRenderRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
        }



        private void DrawLocalPlayer(Player player)
        {
            float outlineOffset = 2;
            Vector2 drawPosition = player.position;
            drawPosition.Y += player.gfxOffY;
            Vector2 left = drawPosition + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPosition + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPosition + Vector2.UnitY * -outlineOffset;
            Vector2 dowm = drawPosition + Vector2.UnitY * outlineOffset;
            float rotation = player.fullRotation;


            IPlayerRenderer playerRenderer = Main.PlayerRenderer;
            playerRenderer.DrawPlayer(Main.Camera, player, left, rotation, player.fullRotationOrigin);
            playerRenderer.DrawPlayer(Main.Camera, player, right, rotation, player.fullRotationOrigin);
            playerRenderer.DrawPlayer(Main.Camera, player, up, rotation, player.fullRotationOrigin);
            playerRenderer.DrawPlayer(Main.Camera, player, dowm, rotation, player.fullRotationOrigin);
        }
        private void DrawOutlines(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
  
            _outlinesToDraw ??= new List<IDrawOutlines>();
            _lightColors ??= new List<Color>();

            _outlinesToDraw.Clear();
            _lightColors.Clear();
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.ModNPC is IDrawOutlines drawOutline)
                {
                    _outlinesToDraw.Add(drawOutline);
                    Point tile = npc.position.ToTileCoordinates();
                    Color lightColor = Lighting.GetColor(tile.X, tile.Y);
                    _lightColors.Add(lightColor);
                }
            }

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
