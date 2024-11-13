using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Skies
{
    public class RoyalCapitalStars : ModSystem
    {
        private Vector2 _parallax;
        private Vector2 _lastCameraPos;
        public bool IsActive => Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneAlcadzia;
        public float Opacity;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.DrawDust += DrawStars;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawDust -= DrawStars;
        }
        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            if (IsActive)
            {
                Opacity = MathHelper.Lerp(Opacity, 0.5f, 0.1f);
            }
            else
            {
                Opacity = MathHelper.Lerp(Opacity, 0f, 0.1f);
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

        private void DrawStars(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (Opacity != 0)
            {
                var starsTexture = TextureRegistry.StarNoise;
                var noiseTexture = TextureRegistry.StarNoise2;
                MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalStars"];

                eff.Shader.Parameters["primaryTexture"].SetValue(starsTexture.Value);
                eff.Shader.Parameters["primaryTextureSize"].SetValue(starsTexture.Value.Size());
                eff.Shader.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                eff.UseImage2(noiseTexture);
                eff.Shader.Parameters["uImageOffset"].SetValue(-_parallax * 0.0005f);
                eff.Shader.Parameters["uOpacity"].SetValue(Opacity);
                eff.Apply();

                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, eff.Shader, Main.BackgroundViewMatrix.TransformationMatrix);

                spriteBatch.Draw(starsTexture.Value, 
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), 
                    new Rectangle((int)-_parallax.X, (int)-_parallax.Y, Main.screenWidth, Main.screenHeight), Color.White * 0.3f);

                spriteBatch.End();
            }
        }
    }

    public class RoyalCapitalSky : CustomSky
    {
        private Vector2 _parallax;
        private Vector2 _lastCameraPos;
        private bool _active;
        private float _strength;
        private float _windSpeed;

        public float Strength { get => _strength; }
        public float Fogginess { get; set; }

        public override void Activate(Vector2 position, params object[] args)
        {
            _active = true;
        }

        public override void Deactivate(params object[] args)
        {
            _active = false;
        }

        public override bool IsActive() =>
            _strength > 0.001f && !Main.gameMenu;

        public override void Reset()
        {
            _active = false;
        }

        public override void Update(GameTime gameTime)
        {
            Parallax();
            Wind();
        }

        private void Parallax()
        {
            Vector2 parallaxAmt = new Vector2(0.5f, 0.25f);
            Vector2 refPosition = Main.Camera.UnscaledPosition;
            Vector2 diff = _lastCameraPos - refPosition;
            _parallax += diff * parallaxAmt;
            _lastCameraPos = refPosition;
        }

        private void Wind()
        {
            _windSpeed += 0.0025f;// Main.WindForVisuals * 0.005f;
            _windSpeed = _windSpeed % 10f;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {

            //Don't
            if (maxDepth >= 11 && minDepth < 11)
            {
                // DrawGradient(spriteBatch);
            }
            if (maxDepth >= 10 && minDepth < 10)
            {
                //  DrawBackClouds(spriteBatch);
            }
            if (maxDepth >= 7 && minDepth < 7)
            {
                DrawSky(spriteBatch);
            }
            if (maxDepth >= 5.2f && minDepth < 5.2f)
            {
                //   DrawFrontClouds(spriteBatch);
            }
        }


        private void DrawSky(SpriteBatch spriteBatch)
        {
            var texture = TextureRegistry.CloudNoise2;
            MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalSky"];

            eff.UseImage1(texture);
            eff.Shader.Parameters["uOpacity"].SetValue(0.5f);
            eff.Apply();
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, eff.Shader, Main.BackgroundViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * 0.3f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
        }
    }
}
