using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Backgrounds;
using Stellamod.Common.Shaders;
using Stellamod.Core.Effects;
using Stellamod.Core.LunarLightingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds
{
    public class CustomBGGlobalLightPlayer : ModPlayer
    {
        public static float LightStrength;
        public override void ResetEffects()
        {
            base.ResetEffects();
            LightStrength = 0;
        }
        public override void PostUpdate()
        {
            base.PostUpdate();
            if (CustomBGManager.drawingCustomBG)
            {
                LightStrength = 0.01f;
            }
            ModContent.GetInstance<LunarLightingRenderer>().AmbientLight = Color.White.ToVector3();
     
        }
    }
    public class CustomBGGlobalWall : GlobalWall
    {

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {

            float lightStrength = CustomBGGlobalLightPlayer.LightStrength;
            if (lightStrength > 0)
            {
                r = MathHelper.Clamp(r + lightStrength, 0, 1);
                g = MathHelper.Clamp(g + lightStrength, 0, 1);
                b = MathHelper.Clamp(b + lightStrength, 0, 1);
            }
        }
    }
    public class CustomBGManager : ModSystem
    {
        private IShader _currentShader;
        public List<CustomBG> Backgrounds = new List<CustomBG>();
        public bool onScreen;
        public Color? darkenBGColor;
        public static bool drawingCustomBG;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.DoDraw_WallsTilesNPCs += DrawBehindWalls;
            On_OverlayManager.Draw += DrawBackgrounds;
            Backgrounds = ModContent.GetContent<CustomBG>().ToList();
        }

        public override void Unload()
        {
            base.Unload();
            Backgrounds = null;
        }


        public override void OnModUnload()
        {
            base.OnModUnload();        
            On_Main.DoDraw_WallsTilesNPCs -= DrawBehindWalls;
            On_OverlayManager.Draw -= DrawBackgrounds;
        }
        private void DrawBackgrounds(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if(layer == RenderLayers.Background)
            {
                
                DrawLoop();
                darkenBGColor = null;
            }
            orig(self, spriteBatch, layer, beginSpriteBatch);

        }

        private void DrawBehindWalls(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
        {
           // DrawLoop();
            orig(self);
        }

        private void DrawLoop()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            //Sort the list by their priority, so the higest priority one is in front
            drawingCustomBG = false;
            foreach (var bg in Backgrounds)
            {
                bg.SetDrawDefaults();
                bg.ParallaxYOffset = -100;
                bg.Alpha += bg.IsActive() ? 0.01f : -0.01f;
                bg.Alpha = MathHelper.Clamp(bg.Alpha, 0, 1);

                if (bg.Alpha != 0)
                {
                    drawingCustomBG = true;
                    DrawBG(bg);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
        private void DrawLoop2()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Sort the list by their priority, so the higest priority one is in front
            drawingCustomBG = false;

            foreach (var bg in Backgrounds)
            {
                bg.SetDrawDefaults();
                bg.ParallaxYOffset = -100;
                bg.Alpha += bg.IsActive() ? 0.01f : -0.01f;
                bg.Alpha = MathHelper.Clamp(bg.Alpha, 0, 1);
                if (bg.Alpha != 0)
                {


                    drawingCustomBG = true;
                    DrawBG(bg);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
        private void DrawBG(CustomBG bg)
        {
            _currentShader = null;
            for (int i = 0; i < bg.Layers.Count; i++)
            {
                DrawBGLayer(bg, bg.Layers[i], bg.Alpha);
            }
        }

        private void DrawBGLayer(CustomBG bg, CustomBGLayer bgLayer, float drawAlpha)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Main.ColorOfTheSkies * drawAlpha;
            if (bg.ignoreSkyColor)
            {
                drawColor = Color.White * drawAlpha;
            }
            if (bg.NoSurfaceLight)
                drawColor = Color.White * drawAlpha;
            if (darkenBGColor.HasValue)
            {
                drawColor = drawColor.MultiplyRGB(darkenBGColor.Value);
            }

            Vector2 defaultParallax = new Vector2();
            defaultParallax.X = Main.screenPosition.X * bgLayer.Parallax * bg.LocalParallaxSpeed;
            defaultParallax.Y = Main.screenPosition.Y * bgLayer.Parallax * bg.LocalParallaxSpeed;
            defaultParallax += bgLayer.ParallaxOffset;
            if (!bg.parallaxInBothWays)
                defaultParallax.Y *= 0;

            int width = (int)bgLayer.Texture.Size().X;
            int height = (int)bgLayer.Texture.Size().Y;

            int worldSurfaceY = bg.GetParallaxYStartHeight();
            /*
            if (!bg.NoSurfaceOffset)
            {
                worldSurfaceY -= 1100;
            }*/

            int diffY = (int)(worldSurfaceY - Main.screenPosition.Y);
            int parallaxY = (int)(diffY * -0.4f * bg.ParallaxYFactor);

            if (bg.NoParallaxY)
                parallaxY = 0;

            Vector2 drawPosition = Vector2.Zero + bgLayer.DrawOffset + new Vector2(0, -parallaxY);
            drawPosition += bg.DrawOffset;
            if (!bg.NoSurfaceOffset)
            {
                int minY = -380;
                if (drawPosition.Y <= minY)
                    drawPosition.Y = minY;
                drawPosition.Y -= 800;
            }

            drawPosition.Y += bg.ParallaxYOffset;
            float drawScale = 2 * bg.DrawScale * bgLayer.DrawScale;

            if (bgLayer.Shader != null)
            {
                BeginEffectLayer(spriteBatch, bgLayer);
            } 
            else
            {

                float combinedParallaxY = parallaxY + defaultParallax.Y * 0.001f;
                BeginParallaxLayer(spriteBatch, defaultParallax.X, combinedParallaxY);
            }
            spriteBatch.Draw(
                bgLayer.Texture.Value,
                drawPosition,
                null,
                drawColor.MultiplyRGB(bg.DrawColor) * bg.Alpha,
                0f,
                default,
                scale: drawScale,
                SpriteEffects.None,
                0f
            );
        }

        private void BeginEffectLayer(SpriteBatch spriteBatch, CustomBGLayer bgLayer)
        {
            Effect effect = bgLayer.Shader == null ? null : bgLayer.Shader.Effect;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred,
                bgLayer.BlendState,
                SamplerState.PointWrap,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                effect);

            _currentShader = bgLayer.Shader;
            _currentShader?.ApplyToEffect();
        }

        private void BeginParallaxLayer(SpriteBatch spriteBatch, float parallaxX, float parallaxY)
        {
            BackgroundParallaxShader backgroundShader = BackgroundParallaxShader.Instance;
            backgroundShader.Parallax = new Vector2(parallaxX * 0.001f, parallaxY);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                effect: backgroundShader.Effect);
        }
    }
}
