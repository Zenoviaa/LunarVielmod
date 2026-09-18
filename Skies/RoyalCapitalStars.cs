using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity;
using Stellamod.Content.Biomes;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Skies
{
    public class RoyalCapitalStars : ModSystem
    {
        private Vector2 _parallax;
        private Vector2 _lastCameraPos;
        public bool IsActive => Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneAlcadzia 
            || NPC.AnyNPCs(ModContent.NPCType<VerlianSingularity>()) 
            || NPC.AnyNPCs(ModContent.NPCType<E>()) 
            || Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMoonspiralTower 
            || NPC.AnyNPCs(ModContent.NPCType<RoyalFox>());
        public float Opacity;
        public bool inStarField;
        public bool UseFade => NPC.AnyNPCs(ModContent.NPCType<RoyalFox>());
        public override void PreUpdateEntities()
        {
            base.PreUpdateEntities();
            inStarField = false;
        }
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.DrawNPCs += DrawBlack;
            On_Main.DrawWaters += DrawBlackWaters;
            On_Main.DrawDust += DrawStars;
        }


        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawNPCs -= DrawBlack;
            On_Main.DrawWaters -= DrawBlackWaters;
            On_Main.DrawDust -= DrawStars;
        }
        private void DrawBlackWaters(On_Main.orig_DrawWaters orig, Main self, bool isBackground)
        {
            if (inStarField)
            {
                return;
            }
            orig(self, isBackground);
        }

        private void DrawBlack(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (inStarField)
            {
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                graphicsDevice.Clear(Color.Black);
            }
   
            orig(self, behindTiles);
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            if (IsActive)
            {
                Opacity = MathHelper.Lerp(Opacity, 1f, 0.1f);
            }
            else
            {
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.1f);
                if (Opacity < 0.01f)
                    Opacity = 0f;
            }
        }


        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            Parallax();
        }
        private void Parallax()
        {
            Vector2 parallaxAmt = new Vector2(0.5f, 0.5f);
            Vector2 refPosition = Main.Camera.UnscaledPosition;
            Vector2 diff = _lastCameraPos - refPosition;
            _parallax += diff * parallaxAmt;
            _lastCameraPos = refPosition;
        }
        private Vector2 GetScreenOffset(float scale)
        {
            //Apply an offset so the texture doesn't move when you're moving
            //This will wrap inside the shader
            Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
            Vector2 screenoffset = Main.screenPosition * texelSize;
            screenoffset *= (1f / scale);
            return screenoffset;
        }

        private void DrawStars(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (Opacity != 0)
            {
                var starsTexture = TextureRegistry.StarNoise2;
                var noiseTexture = TextureRegistry.BlurryPerlinNoise2;
                MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalStars"];

                eff.Shader.Parameters["primaryTexture"].SetValue(starsTexture.Value);
                eff.Shader.Parameters["primaryTextureSize"].SetValue(starsTexture.Value.Size());
                eff.Shader.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                eff.Shader.Parameters["screenOffset"].SetValue(GetScreenOffset(scale: 1));
                eff.UseImage2(noiseTexture);
                eff.Shader.Parameters["parallax"].SetValue(-_parallax * 0.00005f);
                eff.Shader.Parameters["gradientFade"].SetValue(UseFade ? 1.0f : 0.0f);
                eff.UseOpacity(Opacity);
                eff.Apply();

                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(
                    SpriteSortMode.Deferred, 
                    BlendState.AlphaBlend, 
                    SamplerState.PointWrap, 
                    DepthStencilState.None, 
                    Main.Rasterizer, 
                    eff.Shader, 
                    Main.BackgroundViewMatrix.TransformationMatrix);

                Vector2 drawOrigin = starsTexture.Value.Size() * 0.5f;
                BackgroundDrawParameters draw = DrawUtilities.CalculateScaledBackgroundDraw(starsTexture.Value.Size());
                spriteBatch.Draw(starsTexture.Value,
                   draw.DrawOffset,
                    draw.SourceRectangle, Color.White * 0.3f, 0, drawOrigin, 4, SpriteEffects.None, 0);

                spriteBatch.End();
            }
        }
    }
}
